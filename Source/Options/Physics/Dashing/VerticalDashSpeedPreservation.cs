using System;
using Celeste.Mod.GooberHelper.Attributes;

namespace Celeste.Mod.GooberHelper.Options.Physics.Dashing {
    [GooberHelperOption]
    public class VerticalDashSpeedPreservation : AbstractOption {
        public static void OverrideDashSpeed(Player player, Vector2 originalDashSpeed, Vector2 originalSpeed, ref Vector2 orig) {
            if(GetOptionBool(Option.VerticalDashSpeedPreservation) && Math.Sign(originalDashSpeed.Y) == Math.Sign(originalSpeed.Y) && Math.Abs(originalSpeed.Y) > Math.Abs(originalDashSpeed.Y))
                orig.Y = originalSpeed.Y;
        }
    }
}