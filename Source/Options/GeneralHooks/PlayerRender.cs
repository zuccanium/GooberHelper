using System.Reflection;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Extensions;
using Celeste.Mod.GooberHelper.Options.Visuals;
using Celeste.Mod.GooberHelper.Settings.Root;
using Celeste.Mod.GooberHelper.Settings.Toggles;
using Celeste.Mod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.GeneralHooks {
    public static class PlayerRender {
        private static bool startedRendering = false;

        public static bool ShouldCustomRenderBody(Player player)
            => CustomSwimmingAnimation.ShouldDoAnimation(player) || GetOptionBool(Option.RotatePlayerToSpeed) || GetOptionEnum<PlayerShaderMaskValue>(Option.PlayerShaderMask) == PlayerShaderMaskValue.Cover;
        
        public static bool ShouldCustomRenderHair(Player player)
            => GetOptionEnum<PlayerShaderMaskValue>(Option.PlayerShaderMask) == PlayerShaderMaskValue.HairOnly;

        private static FieldInfo f_RasterizerState_CullNone = typeof(RasterizerState).GetField("CullNone");
        private static FieldInfo f_SpriteBatch_transformMatrix = typeof(SpriteBatch).GetField("transformMatrix", Utils.BindingFlagsAll);
        private static Matrix levelBatchMatrix;
        
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

            var shouldCancel = false;
            
            shouldCancel = shouldCancel
                || CustomSwimmingAnimation.OnUpdateSprite(self)
                || RotatePlayerToSpeed.OnUpdateSprite(self);
            
            if(!shouldCancel) {
                orig(self);
            } else {
                //these are important
                self.Sprite.Scale.X = Calc.Approach(self.Sprite.Scale.X, 1f, 1.75f * Engine.DeltaTime);
                self.Sprite.Scale.Y = Calc.Approach(self.Sprite.Scale.Y, 1f, 1.75f * Engine.DeltaTime);
            }

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

            var previousMatrix = (Matrix)f_SpriteBatch_transformMatrix.GetValue(Draw.SpriteBatch);

            rotateHairNodes(self, -1);

            //you
            if(!ShouldCustomRenderHair(self.Entity as Player) || !startedRendering || self.Entity is not Player player) {
                orig(self);

                rotateHairNodes(self, 1);

                return;
            }

            beforeRender(player, RenderSource.Hair);

            orig(self);
            
            afterRender();

            rotateHairNodes(self, 1);

            startedRendering = false;

            f_SpriteBatch_transformMatrix.SetValue(Draw.SpriteBatch, previousMatrix);
        }

        private static void rotateHairNodes(PlayerHair hair, int dir) {
            if(PlayerRotation == 0f)
                return;

            var center = hair.Nodes[0];

            for(var i = 0; i < hair.Nodes.Count; i++)
                hair.Nodes[i] = (hair.Nodes[i] - center).Rotate(PlayerRotation * dir) + center;
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

            levelBatchMatrix = (Matrix)f_SpriteBatch_transformMatrix.GetValue(Draw.SpriteBatch);

            beforeRender(self, RenderSource.Body);

            orig(self);

            afterRender();

            f_SpriteBatch_transformMatrix.SetValue(Draw.SpriteBatch, levelBatchMatrix);
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

            //should be equal to the camera position (but it accounts for translation like in mirror reflections)
            //dw about the negation
            var translation = -levelBatchMatrix.Translation;

            GameplayRenderer.End();

            var playerSpriteCenter = playerMaybe.Center - new Vector2(0, 2);

            Effect effect = null;
            var matrix = RotateMatrixToPlayerRotationAroundVector(levelBatchMatrix, new Vector2(translation.X, translation.Y) - playerSpriteCenter);

            PlayerShaderMask.BeforeRender(playerMaybe, level, source, ref effect, ref matrix);

            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, effect, matrix);
        }

        private static void afterRender() {
            Draw.SpriteBatch.End();
            GameplayRenderer.Begin();
        }

        private static void rotateTrailManagerMatrix() {
            var matrix = (Matrix)f_SpriteBatch_transformMatrix.GetValue(Draw.SpriteBatch);

            //256 = 512/2 = buffer.width/2 = buffer.height/2
            //the 8 is just a magic number to make the rotation around the player sprite work dw about it
            matrix = RotateMatrixToPlayerRotationAroundVector(matrix, -new Vector2(256, 256) + new Vector2(0, 8));

            f_SpriteBatch_transformMatrix.SetValue(Draw.SpriteBatch, matrix);
        }
    }
}