using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.ModImports;
using Celeste.Mod.GooberHelper.Options.GeneralHooks;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Mod.GooberHelper.Options.Visuals {
    [GooberHelperOption(Option.PlayerShaderMask)]
    public static class PlayerShaderMask {
        public static void BeforeRender(Entity playerMaybe, PlayerRender.RenderSource source, ref Effect effect, ref Matrix matrix) {
            var playerShaderMaskValue = GetOptionEnum<PlayerShaderMaskValue>(Option.PlayerShaderMask);

            var ableToRender = playerShaderMaskValue switch {
                PlayerShaderMaskValue.Cover => true,
                PlayerShaderMaskValue.HairOnly => source == PlayerRender.RenderSource.Hair,
                _ => false
            };

            if(!ableToRender || Engine.Scene is not Level level)
                return;

            effect = FrostHelper.GetEffectOrNull?.Invoke("GooberHelper/PlayerShaderMask/playerMask");

            if(effect is null)
                return;

            var tex = GFX.Game["GooberHelper/mask"].Texture.Texture;

            effect.CurrentTechnique = effect.Techniques["Grongle"];
            effect.Parameters["CamPos"].SetValue((Engine.Scene as Level).Camera.Position);
            effect.Parameters["HairColor"].SetValue(PlayerRender.LastPlayerHairColor.ToVector4());
            effect.Parameters["TextureSize"].SetValue(new Vector2(tex.Width, tex.Height));
            effect.Parameters["Time"].SetValue(Engine.Scene.TimeActive);
            effect.Parameters["KeepOutlines"].SetValue(source == PlayerRender.RenderSource.Hair && playerShaderMaskValue == PlayerShaderMaskValue.HairOnly);
            Engine.Graphics.GraphicsDevice.Textures[1] = tex;
        }
    }
}