using System;
using Celeste.Mod.GooberHelper.Attributes;

namespace Celeste.Mod.GooberHelper.Options.Physics.Jumping {
    [GooberHelperOption]
    public class JumpInversion : AbstractOption {
        public enum Value {
            None,
            GroundJumps,
            All
        }

        public static void BeforeJump(Player player, bool isClimbjump) {
            var jumpInversionValue = GetOptionEnum<Value>(Option.JumpInversion);

            if(
                player.moveX == -Math.Sign(player.Speed.X) &&
                (
                    jumpInversionValue == Value.All ||
                    !isClimbjump && jumpInversionValue == Value.GroundJumps
                )
            ) {
                player.Speed.X *= -1;
            }
        }
    }
}