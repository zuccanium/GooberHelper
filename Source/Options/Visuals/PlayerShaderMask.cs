using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.ModImports;
using Celeste.Mod.GooberHelper.Options.GeneralHooks;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Mod.GooberHelper.Options.Visuals {
    [GooberHelperOption(Option.PlayerShaderMask)]
    public static class PlayerShaderMask {
        private static Color maskColor;

        public static void BeforeRender(Entity playerMaybe, Level level, PlayerRender.RenderSource source, ref Effect effect, ref Matrix matrix) {
            var playerShaderMaskValue = GetOptionEnum<PlayerShaderMaskValue>(Option.PlayerShaderMask);

            var ableToRender = playerShaderMaskValue switch {
                PlayerShaderMaskValue.Cover => true,
                PlayerShaderMaskValue.HairOnly => source == PlayerRender.RenderSource.Hair,
                _ => false
            };

            if(!ableToRender)
                return;

            effect = FrostHelper.GetEffectOrNull?.Invoke("GooberHelper/PlayerShaderMask/playerMask");

            if(effect is null)
                return;

            var tex = GFX.Game["GooberHelper/mask"].Texture.Texture;

            effect.CurrentTechnique = effect.Techniques["Grongle"];
            effect.Parameters["CamPos"].SetValue((Engine.Scene as Level).Camera.Position);
            effect.Parameters["HairColor"].SetValue(maskColor.ToVector4());
            effect.Parameters["TextureSize"].SetValue(new Vector2(tex.Width, tex.Height));
            effect.Parameters["Time"].SetValue(Engine.Scene.TimeActive);
            effect.Parameters["KeepOutlines"].SetValue(source == PlayerRender.RenderSource.Hair && playerShaderMaskValue == PlayerShaderMaskValue.HairOnly);
            Engine.Graphics.GraphicsDevice.Textures[1] = tex;
        }

        public static void SetMaskColor(Player player)
            => maskColor = new Color(new Vector4(player.Hair.Color.ToVector3() * player.Sprite.Color.ToVector3(), 1));
    }
}