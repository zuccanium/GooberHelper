using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Extensions;

namespace Celeste.Mod.GooberHelper.Options.Physics.Jumping {
    [GooberHelperOption(Option.VerticalToHorizontalSpeedOnGroundJump)]
    public static class VerticalToHorizontalSpeedOnGroundJump {
        public static void HandleVerticalSpeedToHorizontal(Player player, Vector2 originalSpeed) {
            // var verticalToHorizontalSpeedOnGroundJumpValue = GetOptionEnum<VerticalToHorizontalSpeedOnGroundJumpValue>(Option.VerticalToHorizontalSpeedOnGroundJump);
            
            // if(verticalToHorizontalSpeedOnGroundJumpValue != VerticalToHorizontalSpeedOnGroundJumpValue.None) {
            //     var ext = player.GetExtensionFields();

            //     var retainedVerticalSpeed = !ext.AwesomeRetentionWasInWater && ext.LenientAllDirectionRetentionTimer > 0 && ext.AwesomeRetentionDirection.X == 0
            //         ? Math.Abs(ext.LenientAllDirectionRetentionSpeed.Y)
            //         : 0;

            //     float dir = Math.Sign(player.Speed.X);

            //     if(dir == 0)
            //         dir = player.moveX;
                
            //     if(dir == 0)
            //         dir = (int)player.Facing;

            //     var speedToConvert = Math.Max(Math.Abs(originalSpeed.Y), retainedVerticalSpeed);
                
            //     if(verticalToHorizontalSpeedOnGroundJumpValue == VerticalToHorizontalSpeedOnGroundJumpValue.Magnitude)
            //         speedToConvert = new Vector2(speedToConvert, originalSpeed.X).Length();

            //     player.Speed.X = dir * Math.Max(speedToConvert, Math.Abs(player.Speed.X));
            // }
        }
    }
}