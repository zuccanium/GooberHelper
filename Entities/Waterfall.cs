using Celeste.Mod.Entities;
using System;
using System.Linq;
using System.Collections.Generic;
using Celeste.Mod.GooberHelper.Attributes.Hooks;

namespace Celeste.Mod.GooberHelper.Entities {

    [CustomEntity("GooberHelper/Waterfall")]
    [TrackedAs(typeof(Water))]
    public class Waterfall : Water {
        private MTexture splashTexture = GFX.Game["objs/waterfall/GooberHelper/fade"];
        private MTexture noiseTexture;

        private bool nonCollidable = false;

        private bool playerInside = false;
        private float speed = 200f;

        List<Vector4> splashes = [];

        public Waterfall(EntityData data, Vector2 offset) : base(data.Position + offset, false, false, data.Width, data.Height) {
            nonCollidable = data.Bool("nonCollidable", false);
            Depth = -9999;

            Add(new PlayerCollider(onPlayer, null, Collider));
        }

        public override void Added(Scene scene) {
            for(var i = 0; i < Width / 2; i++) {
                var angle = Random.Shared.NextAngle();

                splashes.Add(new Vector4(i * 2f, MathF.Cos(angle), MathF.Sin(angle), Random.Shared.NextFloat()));
            }

            base.Added(scene);
        }

        private void onPlayer(Player player) {
            if(nonCollidable)
                return;

            player.MoveV(speed * Engine.DeltaTime);
        }

        public override void Update() {
            base.Update();

            if(nonCollidable)
                return;

            var player = CollideFirst<Player>();

            if(player is null && playerInside)
                player.Speed.Y += speed;

            playerInside = player is not null;
        }

        public override void Render() {
            base.Render();

            var scroll = 128 - ((int)(Scene.TimeActive * 96) % 128);
            var scrollOverlay = 128 - ((int)(Scene.TimeActive * 192) % 128);

            var padding = 3;

            for(var i = 0; i < Math.Ceiling(Height/128) + 1; i++) {
                noiseTexture = new MTexture(GFX.Game["objs/waterfall/GooberHelper/noiseOverlay"], null, new Rectangle(0, scrollOverlay - i * 128, 128, (int)Height + padding), new Vector2(0, 0), 128, (int)Height + padding);

                for(var j = 0; j < Math.Ceiling(Width/128); j++) {
                    if(j == Math.Floor(Width/128))
                        noiseTexture = new MTexture(GFX.Game["objs/waterfall/GooberHelper/noiseOverlay"], null, new Rectangle(0, scrollOverlay - i * 128, (int)Width % 128, (int)Height + padding), new Vector2(0, 0), (int)Width % 128, (int)Height + padding);

                    noiseTexture.DrawJustified(Position + new Vector2(j * 128, i * 128 - (i > 0 ? scrollOverlay : 0)), Vector2.Zero);
                }
            }

            foreach(var splash in splashes) {
                var basePos = Position + new Vector2(splash.X, Height);
                var len = 1.5f;
                var fac = (Scene.TimeActive * 4f + splash.W * len) % len;

                var offset = new Vector2(splash.Y, splash.Z) * 32 * fac;

                var a = (len - fac) * 0.5f;

                splashTexture.DrawCentered(basePos + offset, new Color(a,a,a,a), 0.75f);
            }
        }

        //todo: REFACTOR THESE TO NOT EXIST; just make the entity noncollidable through the base class
        [OnHook]
        private static bool patch_Player_SwimCheck(On.Celeste.Player.orig_SwimCheck orig, Player self) {
            if(self.CollideAll<Water>().Any(water => water is Waterfall && (water as Waterfall).nonCollidable))
                return false;
            
            return orig(self);
        }

        [OnHook]
        private static bool patch_Player_SwimCheck(On.Celeste.Player.orig_SwimJumpCheck orig, Player self) {
            if(self.CollideAll<Water>().Any(water => water is Waterfall && (water as Waterfall).nonCollidable))
                return false;
            
            return orig(self);
        }

        [OnHook]
        private static bool patch_Player_UnderwaterMusicCheck(On.Celeste.Player.orig_UnderwaterMusicCheck orig, Player self) {
            if(self.CollideAll<Water>().Any(water => water is Waterfall && (water as Waterfall).nonCollidable))
                return false;
            
            return orig(self);
        }
    }
}