using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.ModImports;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Mod.GooberHelper.Options.Visuals {
    [GooberHelperOption(Option.PlayerShaderMask)]
    public static class PlayerShaderMask {
        private static bool startedRendering = false;
        private static Color lastPlayerHairColor = Player.NormalHairColor;

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
            if(GetOptionValue(Option.PlayerShaderMask) != (int)PlayerShaderMaskValue.HairOnly || !startedRendering) {
                orig(self);

                return;
            }

            if(self.Entity is not Player) return;

            doPlayerMaskStuffBefore(new Vector4((self.Entity as Player).Hair.Color.ToVector3() * (self.Entity as Player).Sprite.Color.ToVector3(), 1), true);

            orig(self);

            doPlayerMaskStuffAfter();

            startedRendering = false;
        }

        [OnHook]
        private static void patch_PlayerDeadBody_Render(On.Celeste.PlayerDeadBody.orig_Render orig, PlayerDeadBody self) {
            if(GetOptionValue(Option.PlayerShaderMask) != (int)PlayerShaderMaskValue.Cover) {
                orig(self);

                return;
            }

            doPlayerMaskStuffBefore(lastPlayerHairColor.ToVector4());

            orig(self);

            doPlayerMaskStuffAfter();
        }
        
        [OnHook]
        private static void patch_Player_Render(On.Celeste.Player.orig_Render orig, Player self) {            
            startedRendering = true;

            if(GetOptionValue(Option.PlayerShaderMask) != (int)PlayerShaderMaskValue.Cover) {
                orig(self);

                return;
            }

            lastPlayerHairColor = self.Hair.Color;

            doPlayerMaskStuffBefore(new Vector4(self.Hair.Color.ToVector3() * self.Sprite.Color.ToVector3(), 1));

            orig(self);

            doPlayerMaskStuffAfter();
        }

        private static void doPlayerMaskStuffBefore(Vector4 color, bool keepOutlines = false) {
            var playerMaskEffect = FrostHelper.GetEffectOrNull?.Invoke("GooberHelper/PlayerShaderMask/playerMask");

            if(playerMaskEffect == null || (Engine.Scene as Level) == null)
                return;

            GameplayRenderer.End();

            var tex = GFX.Game["GooberHelper/mask"].Texture.Texture;
            
            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, playerMaskEffect, (Engine.Scene as Level).GameplayRenderer.Camera.Matrix);
            playerMaskEffect.CurrentTechnique = playerMaskEffect.Techniques["Grongle"];
            playerMaskEffect.Parameters["CamPos"].SetValue((Engine.Scene as Level).Camera.Position);
            playerMaskEffect.Parameters["HairColor"].SetValue(color);
            playerMaskEffect.Parameters["TextureSize"].SetValue(new Vector2(tex.Width, tex.Height));
            playerMaskEffect.Parameters["Time"].SetValue(Engine.Scene.TimeActive);
            playerMaskEffect.Parameters["KeepOutlines"].SetValue(keepOutlines);
            Engine.Graphics.GraphicsDevice.Textures[1] = tex;
        }

        private static void doPlayerMaskStuffAfter() {
            Draw.SpriteBatch.End();
            GameplayRenderer.Begin();
        }
    }
}