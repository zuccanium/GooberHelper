using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Options.Physics.Jumping;

namespace Celeste.Mod.GooberHelper.Options.GeneralHooks {
    public static class Jump {
        [OnHook]
        private static void patch_Player_Jump(On.Celeste.Player.orig_Jump orig, Player self, bool particles, bool playSfx) {
            var isClimbjump = particles == false && playSfx == false;
            var originalSpeed = self.Speed;

            if(!isClimbjump)
                VerticalToHorizontalSpeedOnGroundJump.BeforeJump(self, originalSpeed);

            JumpInversion.BeforeJump(self, isClimbjump);

            orig(self, particles, playSfx);
        }
    }
}