using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Extensions;

namespace Celeste.Mod.GooberHelper.Options.Visuals {
    [GooberHelperOption]
    public class RotatePlayerToSpeed : AbstractOption {        
        public static bool OnUpdateSprite(Player player, PlayerExtensions.PlayerExtensionFields ext, ref bool somethingActive) {
            if(!GetOptionBool(Option.RotatePlayerToSpeed))
                return false;

            somethingActive = true;
            
            var conservedSpeed = player.GetConservedVisualSpeed(ext);

            ext.PlayerRotation = ext.PlayerRotationTarget = conservedSpeed == Vector2.Zero
                ? 0f
                : conservedSpeed.Angle() + MathF.PI / 2f;

            return false;
        }
    }
}