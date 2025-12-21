using System;
using Celeste.Mod.GooberHelper.Attributes;

namespace Celeste.Mod.GooberHelper.Options.Physics.Jumping {
    [GooberHelperOption]
    public class WalljumpSpeedPreservation : AbstractOption {
        public enum Value {
            None,
            FakeRCB,
            Preserve,
            Invert,
        }

        public static void InWallJump(Player player, Vector2 originalSpeed, int dir) {
            var wallJumpSpeedPreservationValue = GetOptionEnum<Value>(Option.WalljumpSpeedPreservation);

            if(wallJumpSpeedPreservationValue == Value.None)
                return;

            var res = wallJumpSpeedPreservationValue switch {
                Value.Invert => Math.Abs(originalSpeed.X),
                Value.None => 0f,
                
                //FakeRCB and Preserve (theyre practically the same)
                _ => Math.Sign(originalSpeed.X) == dir ? Math.Abs(originalSpeed.X) : 0f,
            };

            if(wallJumpSpeedPreservationValue == Value.FakeRCB && player.moveX != 0)
                res -= 40f; //this should really support modification from other sources (e.g. extvars), but i really dont see a realistic way of doing that which isnt terrible

            player.Speed.X = Utils.SignedAbsMax(
                player.Speed.X,
                res
            );
        }
    }
}