using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Extensions;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Jumping {
    [GooberHelperOption(Option.VerticalToHorizontalSpeedOnGroundJump)]
    public static class VerticalToHorizontalSpeedOnGroundJump {
        public static void BeforeJump(Player player, Vector2 originalSpeed) {
            var verticalToHorizontalSpeedOnGroundJumpValue = GetOptionEnum<VerticalToHorizontalSpeedOnGroundJumpValue>(Option.VerticalToHorizontalSpeedOnGroundJump);
            
            if(verticalToHorizontalSpeedOnGroundJumpValue != VerticalToHorizontalSpeedOnGroundJumpValue.None) {
                var ext = player.GetExtensionFields();

                var dir = Utils.FirstNonZero(
                    Math.Sign(player.Speed.X),
                    player.moveX,
                    (int)player.Facing
                );

                var speedToConvert = Utils.UnsignedAbsMax(
                    originalSpeed.Y,
                    ext.VerticalRetentionTimer > 0
                        ? Math.Abs(ext.VerticalRetentionSpeed)
                        : 0
                );
                
                if(verticalToHorizontalSpeedOnGroundJumpValue == VerticalToHorizontalSpeedOnGroundJumpValue.Magnitude)
                    speedToConvert = new Vector2(speedToConvert, originalSpeed.X).Length();

                player.Speed.X = dir * Math.Max(speedToConvert, Math.Abs(player.Speed.X));
            }
        }
    }
}