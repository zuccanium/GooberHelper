using System;
using Celeste.Mod.GooberHelper.Entities;
using Celeste.Mod.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using MonoMod.Cil;
using MonoMod.Utils;

namespace Celeste.Mod.GooberHelper.UI {
    public class HighResolutionBulletRenderer : Renderer {
        public static VirtualRenderTarget Buffer => Mod.UI.SubHudRenderer.Buffer;

        public static bool DrawToBuffer {
            get {
                return Buffer != null && (Engine.ViewWidth < 1920 || Engine.ViewHeight < 1080);
            }
        }

        public static bool DontRender = false;
        private static Type? celesteTasHitboxToggleType;

        public static string RenderState_Effect = "";
        public static bool RenderState_Additive = false;

        public static void Load() {
            IL.Celeste.LevelLoader.LoadingThread += modifyLevelLoadingThread;
            IL.Celeste.Level.Render += modifyLevelRender;
        }

        public static void Unload() {
            IL.Celeste.LevelLoader.LoadingThread -= modifyLevelLoadingThread;
            IL.Celeste.Level.Render -= modifyLevelRender;
        }

        public static void modifyLevelLoadingThread(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNextBestFit(MoveType.AfterLabel,
                instr => instr.MatchLdarg0(),
                instr => instr.MatchCallOrCallvirt<LevelLoader>("get_Level"),
                instr => instr.MatchLdarg0(),
                instr => instr.MatchCallOrCallvirt<LevelLoader>("get_Level"),
                instr => instr.MatchNewobj<HudRenderer>()
            )) {
                Console.WriteLine("found the thing");

                cursor.EmitLdarg0();
                cursor.EmitDelegate((LevelLoader loader) => {
                    var renderer = new HighResolutionBulletRenderer();
                    Console.WriteLine("setting renderer");

                    loader.Level.Add(renderer);
                    DynamicData.For(loader.Level).Set("HighResolutionBulletRenderer", renderer);
                    Console.WriteLine("set renderer");

                    if(Everest.Loader.DependencyLoaded(new EverestModuleMetadata() {
                        Name = "CelesteTAS",
                        Version = new Version(3, 0, 0)
                    })) {
                        celesteTasHitboxToggleType = Type.GetType("TAS.EverestInterop.Hitboxes.HitboxToggle, CelesteTAS-EverestInterop, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")!;
                    }
                });
            }
        }

        public static void modifyLevelRender(ILContext il) {
            var cursor = new ILCursor(il);

            //dont let the gameplay renderer render high resolution bullets
            cursor.EmitDelegate(() => { DontRender = true; });

            if(cursor.TryGotoNext(MoveType.AfterLabel,
                instr => instr.MatchLdarg0(),
                instr => instr.MatchLdfld<Scene>("Paused"),
                instr => instr.MatchBrfalse(out var _)
            )) {
                Console.WriteLine("efjiief");

                cursor.EmitLdarg0();
                cursor.EmitDelegate((Level level) => {
                    DynamicData.For(level).Get<HighResolutionBulletRenderer>("HighResolutionBulletRenderer")?.Render(level);
                });
            }
        }

        public static void BeginRender(BlendState blend = null, SamplerState sampler = null) {
            Draw.SpriteBatch.Begin(
                SpriteSortMode.Deferred,
                blend ?? BlendState.AlphaBlend,
                sampler ?? SamplerState.LinearClamp,
                DepthStencilState.Default,
                RasterizerState.CullNone,
                null,
                DrawToBuffer ? Matrix.Identity : Engine.ScreenMatrix
            );
        }

        public static void EndRender() {
            Draw.SpriteBatch.End();
        }

        public override void BeforeRender(Scene scene) {
            if (!DrawToBuffer)
                return;

            RenderContent(scene);
        }

        public override void Render(Scene scene) {
            if (!DrawToBuffer) {
                RenderContent(scene);
                return;
            }

            Draw.SpriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.LinearClamp,
                DepthStencilState.Default,
                RasterizerState.CullNone,
                null,
                Engine.ScreenMatrix
            );
            Draw.SpriteBatch.Draw(Buffer, new Vector2(-1f, -1f), Color.White);
            Draw.SpriteBatch.End();
        }

        public void RenderContent(Scene scene) {
            BeginRender();
            DontRender = false;

            RenderState_Effect = "string that wont ever be equal to a real shader";
            RenderState_Additive = false;

            foreach(Bullet entity in scene.Tracker.GetEntities<Bullet>()) {
                if(!entity.LowResolution) {
                    if(entity.Visible)
                        entity.Render();
                } 
            }

            Bullet.EndRender(false);
            Bullet.BeginRender(false, "", false);

            bool debugRender = (bool)(celesteTasHitboxToggleType?.GetProperty("DrawHitboxes")?.GetValue(null) ?? false) || Engine.Commands.Open;

            if(debugRender) {
                foreach(Bullet entity in scene.Tracker.GetEntities<Bullet>()) {
                    if(!entity.LowResolution)
                        entity.DebugRender((scene as Level)!.Camera);
                }
            }

            Bullet.EndRender(false);

            EndRender();
        }
    }
}