using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Helpers;
using Celeste.Mod.Helpers;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Other {
    [GooberHelperOption(Option.RemoveNormalEnd)]
    public static class RemoveNormalEnd {
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