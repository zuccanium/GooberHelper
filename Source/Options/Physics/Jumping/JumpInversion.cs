using System;
using Celeste.Mod.GooberHelper.Attributes;

namespace Celeste.Mod.GooberHelper.Options.Physics.Jumping {
    [GooberHelperOption(Option.JumpInversion)]
    public static class JumpInversion {
        public static void BeforeJump(Player player, bool isClimbjump) {
            var jumpInversionValue = GetOptionEnum<JumpInversionValue>(Option.JumpInversion);

            if(
                player.moveX == -Math.Sign(player.Speed.X) &&
                (
                    jumpInversionValue == JumpInversionValue.All ||
                    !isClimbjump && jumpInversionValue == JumpInversionValue.GroundJumps
                )
            ) {
                player.Speed.X *= -1;
            }
        }
    }
}