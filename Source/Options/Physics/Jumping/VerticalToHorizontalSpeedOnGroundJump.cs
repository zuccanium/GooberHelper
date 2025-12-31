using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Extensions;

namespace Celeste.Mod.GooberHelper.Options.Physics.Jumping {
    [GooberHelperOption]
    public class VerticalToHorizontalSpeedOnGroundJump : AbstractOption {
        public enum Value {
            None,
            Vertical,
            Magnitude
        }

        public static void BeforeJump(Player player, Vector2 originalSpeed) {
            var verticalToHorizontalSpeedOnGroundJumpValue = GetOptionEnum<Value>(Option.VerticalToHorizontalSpeedOnGroundJump);
            
            if(verticalToHorizontalSpeedOnGroundJumpValue != Value.None) {
                var ext = player.GetExtensionFields();

                var dir = Utils.FirstSign(
                    player.Speed.X,
                    player.moveX,
                    (int)player.Facing
                );

                var speedToConvert = Utils.UnsignedAbsMax(
                    originalSpeed.Y,
                    ext.VerticalRetentionTimer > 0
                        ? Math.Abs(ext.VerticalRetentionSpeed)
                        : 0
                );
                
                if(verticalToHorizontalSpeedOnGroundJumpValue == Value.Magnitude)
                    speedToConvert = new Vector2(speedToConvert, originalSpeed.X).Length();

                player.Speed.X = dir * Math.Max(speedToConvert, Math.Abs(player.Speed.X));
            }
        }
    }
}