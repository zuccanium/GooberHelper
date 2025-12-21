using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;

namespace Celeste.Mod.GooberHelper.Options.Physics.Other {
    [GooberHelperOption]
    public class RemoveNormalEnd : AbstractOption {
        [OnHook]
        private static void patch_Player_NormalEnd(On.Celeste.Player.orig_NormalEnd orig, Player self) {
            var originalWallSpeedRetentionTimer = self.wallSpeedRetentionTimer;
            var originalWallBoostTimer = self.wallBoostTimer;

            orig(self);

            if(!GetOptionBool(Option.RemoveNormalEnd))
                return;

            self.wallSpeedRetentionTimer = originalWallSpeedRetentionTimer;
            self.wallBoostTimer = originalWallBoostTimer;
        }
    }
}