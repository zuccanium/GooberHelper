using System;
using Celeste.Mod.GooberHelper.Attributes;

namespace Celeste.Mod.GooberHelper.Options.Physics.Jumping {
    [GooberHelperOption(Option.WalljumpSpeedPreservation)]
    public static class WalljumpSpeedPreservation {
        public static void SetSpeed(Player player, Vector2 originalSpeed, int dir) {
            var wallJumpSpeedPreservationValue = GetOptionEnum<WalljumpSpeedPreservationValue>(Option.WalljumpSpeedPreservation);

            if(wallJumpSpeedPreservationValue == WalljumpSpeedPreservationValue.None)
                return;

            var result = wallJumpSpeedPreservationValue switch {
                WalljumpSpeedPreservationValue.Invert => Math.Abs(originalSpeed.X),
                WalljumpSpeedPreservationValue.None => 0f,
                
                //FakeRCB and Preserve (theyre practically the same)
                _ => Math.Sign(originalSpeed.X) == dir ? Math.Abs(originalSpeed.X) : 0f,
            };

            if(wallJumpSpeedPreservationValue == WalljumpSpeedPreservationValue.FakeRCB && player.moveX != 0)
                result -= 40f;

            player.Speed.X = Utils.SignedAbsMax(
                player.Speed.X,
                result
            );
        }
    }
}