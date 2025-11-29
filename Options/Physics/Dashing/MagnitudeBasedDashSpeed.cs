using System;
using Celeste.Mod.GooberHelper.Attributes;

namespace Celeste.Mod.GooberHelper.Options.Physics.Dashing {
    [GooberHelperOption(Option.MagnitudeBasedDashSpeed)]
    public static class MagnitudeBasedDashSpeed {
        public static bool OverrideDashSpeed(Player player, Vector2 beforeDashSpeedConserved, Vector2 originalSpeed, ref Vector2 orig) {
            var magnitudeBasedDashSpeedValue = GetOptionEnum<MagnitudeBasedDashSpeedValue>(Option.MagnitudeBasedDashSpeed);
            var isCardinal = Vector2.Dot(beforeDashSpeedConserved, Vector2.UnitX) % 1 == 0;

            if(
                magnitudeBasedDashSpeedValue == MagnitudeBasedDashSpeedValue.All ||
                magnitudeBasedDashSpeedValue == MagnitudeBasedDashSpeedValue.OnlyCardinal && isCardinal
            ) {
                orig = beforeDashSpeedConserved.SafeNormalize() * Math.Max(originalSpeed.Length(), beforeDashSpeedConserved.Length());

                return true;
            }

            return false;
        }
    }
}