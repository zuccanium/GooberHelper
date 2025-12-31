using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;

namespace Celeste.Mod.GooberHelper.Options.Physics.Moving {
    [GooberHelperOption]
    public class UpwardsTransitionSpeedPreservation : AbstractOption {
        [OnHook]
        private static void patch_Player_BeforeUpTransition(On.Celeste.Player.orig_BeforeUpTransition orig, Player self) {
            if(!GetOptionBool(Option.UpwardsTransitionSpeedPreservation)) {
                orig(self);

                return;
            }

            var varJumpTimer = self.varJumpTimer;
            var varJumpSpeed = self.varJumpSpeed;
            var speed = self.Speed;
            var dashCooldownTimer = self.dashCooldownTimer;

            orig(self);

            self.varJumpTimer = varJumpTimer;
            self.varJumpSpeed = varJumpSpeed;
            self.Speed = speed;
            self.dashCooldownTimer = dashCooldownTimer;
        }
    }
}