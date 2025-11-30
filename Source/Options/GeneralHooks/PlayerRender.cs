using System.Reflection;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Extensions;
using Celeste.Mod.GooberHelper.Options.Visuals;
using Celeste.Mod.GooberHelper.Settings.Toggles;
using Celeste.Mod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.GeneralHooks {
    public static class PlayerRender {
        private static bool startedRendering = false;

        public static bool ShouldCustomRenderBody(Player player)
            => CustomSwimmingAnimation.ShouldDoAnimation(player) || GetOptionEnum<PlayerShaderMaskValue>(Option.PlayerShaderMask) == PlayerShaderMaskValue.Cover;
        
        public static bool ShouldCustomRenderHair(Player player)
            => GetOptionEnum<PlayerShaderMaskValue>(Option.PlayerShaderMask) == PlayerShaderMaskValue.HairOnly;

        private static FieldInfo f_RasterizerState_CullNone = typeof(RasterizerState).GetField("CullNone");
        private static FieldInfo f_SpriteBatch_transformMatrix = typeof(SpriteBatch).GetField("transformMatrix", Utils.BindingFlagsAll);
        
        public enum RenderSource {
            Body,
            DeadBody,
            Hair
        }

        public static float PlayerRotation = 0f;
        public static float PlayerRotationTarget = 0f;

        public static Matrix RotateMatrixToPlayerRotationAroundVector(Matrix matrix, Vector2 vector) {
            var offset = new Vector3(vector, 0);

            return matrix
                * Matrix.CreateTranslation(offset)
                * Matrix.CreateRotationZ(PlayerRotation)
                * Matrix.CreateTranslation(-offset);
        }

        [OnHook]
        private static void patch_Player_UpdateSprite(On.Celeste.Player.orig_UpdateSprite orig, Player self) {
            var ext = self.GetExtensionFields();

            PlayerRotation = ext.PlayerRotation;
            PlayerRotationTarget = ext.PlayerRotationTarget;

            //this should probably go into the other one but im really lazy rn and i dont care 🧼😭
            if(self.StateMachine.State != Player.StSwim) {
                PlayerRotation = 0f;
                PlayerRotationTarget = 0f;
            }
            
            orig(self);

            PlayerRotation = Calc.AngleApproach(PlayerRotation, PlayerRotationTarget, 20f * Engine.DeltaTime);

            ext.PlayerRotation = PlayerRotation;
            ext.PlayerRotationTarget = PlayerRotationTarget;
        }

        [OnHook]
        private static void patch_PlayerHair_Render(On.Celeste.PlayerHair.orig_Render orig, PlayerHair self) {
            //i need the custom shader to not execute if the startedRendering boolean is false
            //this took way longer than it shouldve to figure out
            //there was a bug where the player trail would be offset from the player for some odd reason
            //i assumed it was a shader problem or more general thing for a while, but i eventually had the idea that it might be specific to the TrailManager (it was)
            //in TrailManager.BeforeRender(), it interrupts the spritebatch and renders specifically the PlayerHair with this method
            //that causes Something to mess up and somehow shift the player trail
            //the startedRendering boolean is set to true when the actual render method is called
            //that Should prevent this method from executing the custom shader code
            //i should document these things more often
            if(!ShouldCustomRenderHair(self.Entity as Player) || ShouldCustomRenderBody(self.Entity as Player) || !startedRendering || self.Entity is not Player player) {
                orig(self);

                return;
            }

            beforeRender(player, RenderSource.Hair);

            orig(self);
            
            afterRender();

            startedRendering = false;
        }

        [OnHook]
        private static void patch_PlayerDeadBody_Render(On.Celeste.PlayerDeadBody.orig_Render orig, PlayerDeadBody self) {
            if(!ShouldCustomRenderBody(null)) {
                orig(self);

                return;
            }

            beforeRender(self, RenderSource.DeadBody);

            orig(self);

            afterRender();
        }
        
        [OnHook]
        private static void patch_Player_Render(On.Celeste.Player.orig_Render orig, Player self) {
            startedRendering = true;

            if(!ShouldCustomRenderBody(self)) {
                orig(self);

                return;
            }

            PlayerShaderMask.SetMaskColor(self);

            beforeRender(self, RenderSource.Body);

            orig(self);

            afterRender();
        }

        [ILHook]
        private static void patch_TrailManager_BeforeRender(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNextBestFit(MoveType.After,
                instr => instr.MatchLdsfld(f_RasterizerState_CullNone),
                instr => instr.MatchCallOrCallvirt<SpriteBatch>("Begin")
            )) {
                cursor.EmitDelegate(rotateTrailManagerMatrix);
            }
        }

        private static void beforeRender(Entity playerMaybe, RenderSource source) {
            if(Engine.Scene is not Level level)
                return;

            GameplayRenderer.End();

            Effect effect = null;
            var matrix = RotateMatrixToPlayerRotationAroundVector(level.Camera.Matrix, level.Camera.Position - (playerMaybe.Center - new Vector2(0, 2)));

            PlayerShaderMask.BeforeRender(playerMaybe, source, ref effect, ref matrix);

            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, effect, matrix);
        }

        private static void afterRender() {
            Draw.SpriteBatch.End();
            GameplayRenderer.Begin();
        }

        private static void rotateTrailManagerMatrix() {
            var matrix = (Matrix)f_SpriteBatch_transformMatrix.GetValue(Draw.SpriteBatch);

            matrix = RotateMatrixToPlayerRotationAroundVector(matrix, -new Vector2(256, 256) + new Vector2(0, 8));

            f_SpriteBatch_transformMatrix.SetValue(Draw.SpriteBatch, matrix);
        }
    }
}