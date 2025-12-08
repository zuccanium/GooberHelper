using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Extensions;
using Celeste.Mod.GooberHelper.Options.GeneralHooks;

namespace Celeste.Mod.GooberHelper.Options {
    [GooberHelperOption()]
    public static class RotatePlayerToSpeed {
        public static bool OnUpdateSprite(Player player, PlayerExtensions.PlayerExtensionFields ext) {
            if(!GetOptionBool(Option.RotatePlayerToSpeed))
                return false;
            
            var conservedSpeed = player.GetConservedVisualSpeed(ext);

            ext.PlayerRotation = ext.PlayerRotationTarget = conservedSpeed == Vector2.Zero
                ? 0f
                : conservedSpeed.Angle() + MathF.PI / 2f;

            return false;
        }
    }
}