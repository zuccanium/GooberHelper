using Celeste.Mod.Entities;
using System;
using System.Linq;
using System.Collections.Generic;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Mod.GooberHelper.Entities {
    [CustomEntity("GooberHelper/Waterfall")]
    [TrackedAs(typeof(Water))]
    public class Waterfall : Water {
        public class Splash {
            public float Position;
            public Vector2 Direction;
            public float Offset;

            public Splash(float position) {
                Position = position;
                
                Direction = Calc.AngleToVector(Random.Shared.NextAngle(), 1);
                Offset = Random.Shared.NextFloat();
            }
        }

        private bool nonCollidable = false;
        
        private float speed;

        private Color backgroundWaterColor;
        private float backgroundWaterOpacity;

        private Color waterColor;
        private List<Texture2D> waterTextureLayers;
        private float waterLayerDistance;
        private float waterSpeed;
        private int waterPadding = 3;

        private Color splashColor;
        private List<MTexture> splashTextures;
        private float splashSpeed;
        private float splashSize;
        private float splashOpacity;
        private float splashDensity;
        private float splashDistance;

        private float time = 0f;
        private Player playerInside;
        private List<Splash> splashes = [];

        public Waterfall(EntityData data, Vector2 offset) : base(data.Position + offset, false, false, data.Width, data.Height) {
            Depth = data.Int("depth", -9999);
            
            speed = data.Float("speed", 200f);

            backgroundWaterColor = data.HexColor("backgroundWaterColor", Color.LightSkyBlue);
            backgroundWaterOpacity = data.Float("backgroundWaterOpacity", 0.3f);
            
            waterColor = data.HexColor("waterColor", Color.White);
            waterTextureLayers = [..
                data.Attr("waterTextureLayers", "objects/waterfall/GooberHelper/water")
                    .Split(",")
                    .Where(path => path != "")
                    .Select(path => GFX.Game[path].Texture.Texture)
            ];
            waterPadding = data.Int("padding", 3);
            waterSpeed = data.Float("waterSpeed", 192f);
            waterLayerDistance = data.Float("waterLayerDistance", 0f);

            splashColor = data.HexColor("splashColor", Color.White);
            splashTextures = [.. 
                data.Attr("splashTextures", "objects/waterfall/GooberHelper/splash")
                    .Split(",")
                    .Where(path => path != "")
                    .Select(path => GFX.Game[path])
            ];
            splashSpeed = data.Float("splashSpeed", 96f);
            splashSize = data.Float("splashSize", 0.75f);
            splashOpacity = data.Float("splashOpacity", 0.75f);
            splashDensity = data.Float("splashDensity", 0.1f);
            splashDistance = data.Float("splashDistance", 48f);

            nonCollidable = data.Bool("nonCollidable", false);

            if(nonCollidable) {
                Collidable = false;

                return;
            }
        }

        public override void Added(Scene scene) {
            base.Added(scene);

            if(splashDensity == 0 || Width == 0)
                return;

            for(var i = 0f; i < 1f; i += 1f / (splashDensity * Width))
                splashes.Add(new Splash(i));

            FillColor = backgroundWaterColor * backgroundWaterOpacity;
        }

        public override void Update() {
            base.Update();
            
            time += Engine.DeltaTime;

            if(nonCollidable)
                return;
            
            var player = CollideFirst<Player>();

            //player just exited the waterfall
            if(player is null && playerInside is not null)
                playerInside.Speed.Y += speed;
            
            player?.MoveV(speed * Engine.DeltaTime);

            playerInside = player;
        }

        public override void Render() {
            base.Render();

            var scroll = time * waterSpeed;

            //this solution is so much better than manually tiling the water holy

            GameplayRenderer.End();
            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, (Scene as Level).Camera.Matrix);

            for(var i = waterTextureLayers.Count - 1; i >= 0; i--) {
                var layer = waterTextureLayers[i];

                var layerScroll = 1f / (i * waterLayerDistance + 1f);
                var camPosMult = 1f - layerScroll;
                var camPos = (Scene as Level).Camera.Position;

                Draw.SpriteBatch.Draw(
                    layer,
                    Position,
                    new Rectangle(
                        (int)(-camPos.X * camPosMult + Position.X),
                        (int)(-camPos.Y * camPosMult + Position.Y - scroll * layerScroll),
                        (int)Width,
                        (int)Height + waterPadding
                    ),
                    waterColor
                );
            }

            Draw.SpriteBatch.End();
            GameplayRenderer.Begin();

            foreach(var splash in splashes) {
                var worldPosition = BottomLeft + new Vector2(splash.Position * Width, 0);
                var pathLerp = (time * splashSpeed / splashDistance + splash.Offset) % 1f;

                if(pathLerp > 1f - Engine.DeltaTime * splashSpeed / splashDistance)
                    splash.Position = Random.Shared.NextFloat();

                worldPosition += splash.Direction * pathLerp * splashDistance;

                var alpha = (1f - pathLerp) * splashOpacity;

                //completely arbitrary constant that acts kinda like hashing go
                splashTextures[(int)MathF.Floor(splash.Offset * 82935.235f % splashTextures.Count)].DrawCentered(worldPosition, splashColor * alpha, splashSize);
            }
        }
    }
}