using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.ModImports;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Mod.GooberHelper.Options.Visuals {
    [GooberHelperOption]
    public class TheoNuclearReactor : AbstractOption {
        [Tracked(false)]
        public class NuclearReactorComponent : Component {
            public NuclearReactorComponent() : base(true, true) {}

            public override void Update() {
                base.Update();

                if(!GetOptionBool(Option.TheoNuclearReactor))
                    return;

                if(Scene.OnInterval(0.02f))
                    (Scene as Level).Displacement.AddBurst(
                        Entity.Center + Vector2.One.Rotate(Random.Shared.NextAngle()) * 10f,
                        Random.Shared.Range(0.5f, 1f),
                        Random.Shared.Range(0f, 100f),
                        Random.Shared.Range(300f, 1000f),
                        Random.Shared.Range(0.2f, 0.3f)
                    );
            }

            public override void Render() {
                base.Render();

                if(!GetOptionBool(Option.TheoNuclearReactor))
                    return;

                var nuclearReactorEffect = FrostHelper.GetEffectOrNull?.Invoke("GooberHelper/NuclearReactor/nuclearReactor");
                
                if(nuclearReactorEffect == null || Engine.Scene is not Level)
                    return;

                var noiseTexture = GFX.Game["GooberHelper/noise"];

                GameplayRenderer.End();

                Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, nuclearReactorEffect, (Engine.Scene as Level).Camera.Matrix);
                nuclearReactorEffect.CurrentTechnique = nuclearReactorEffect.Techniques["Grongle"];
                nuclearReactorEffect.Parameters["time"]?.SetValue(Engine.Scene.TimeActive);
                Engine.Graphics.GraphicsDevice.Textures[1] = GFX.Game["GooberHelper/theoidle00"].Texture.Texture; //using the actual texture is terrible because of the whole texture atlas thing

                noiseTexture.DrawCentered(Entity.Center, Color.White, 8f);

                Draw.SpriteBatch.End();
                GameplayRenderer.Begin();
            }
        }

        [OnHook]
        private static void patch_TheoCrystal_ctor(On.Celeste.TheoCrystal.orig_ctor_Vector2 orig, TheoCrystal self, Vector2 position) {
            orig(self, position);

            self.Add(new NuclearReactorComponent());
        }
    }
}