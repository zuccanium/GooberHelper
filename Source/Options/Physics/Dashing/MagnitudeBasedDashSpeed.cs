using System;
using Celeste.Mod.GooberHelper.Attributes;

namespace Celeste.Mod.GooberHelper.Options.Physics.Dashing {
    [GooberHelperOption]
    public class MagnitudeBasedDashSpeed : AbstractOption {
        public enum Value {
            None,
            OnlyCardinal,
            All
        }

        public static bool OverrideDashSpeed(Player player, Vector2 beforeDashSpeedConserved, Vector2 originalSpeed, ref Vector2 orig) {
            var magnitudeBasedDashSpeedValue = GetOptionEnum<Value>(Option.MagnitudeBasedDashSpeed);
            var isCardinal = Vector2.Dot(beforeDashSpeedConserved, Vector2.UnitX) % 1 == 0;

            if(
                magnitudeBasedDashSpeedValue == Value.All ||
                magnitudeBasedDashSpeedValue == Value.OnlyCardinal && isCardinal
            ) {
                orig = beforeDashSpeedConserved.SafeNormalize() * Math.Max(originalSpeed.Length(), beforeDashSpeedConserved.Length());

                return true;
            }

            return false;
        }
    }
}