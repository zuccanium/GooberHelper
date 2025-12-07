using System;
using Celeste.Mod.GooberHelper.Attributes;

namespace Celeste.Mod.GooberHelper.Options.Physics.Dashing {
    [GooberHelperOption(Option.ReverseDashSpeedPreservation)]
    public static class ReverseDashSpeedPreservation {
        public static void OverrideDashSpeed(Player player, Vector2 originalDashSpeed, Vector2 originalSpeed, ref Vector2 orig) {
            var enabled = GetOptionBool(Option.ReverseDashSpeedPreservation);

            if(enabled && Math.Sign(originalSpeed.X) == -Math.Sign(originalDashSpeed.X) && Math.Abs(originalSpeed.X) > Math.Abs(originalDashSpeed.X))
                orig.X = -originalSpeed.X;

            if(!GetOptionBool(Option.VerticalDashSpeedPreservation))
                return;

            if(enabled && Math.Sign(originalSpeed.Y) == -Math.Sign(originalDashSpeed.Y) && Math.Abs(originalSpeed.Y) > Math.Abs(originalDashSpeed.Y))
                orig.Y = -originalSpeed.Y;
        }
    }
}