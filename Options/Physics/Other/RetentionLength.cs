using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.Helpers;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Other {
    [GooberHelperOption(Option.RetentionLength)]
    public static class RetentionLength {
        [ILHook]
        private static void patch_Player_OnCollideH(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNextBestFit(MoveType.After, instr => instr.MatchLdcR4(0.06f))) {
                cursor.EmitDelegate(overrideRetentionLength);
            }
        }

        private static float overrideRetentionLength(float orig) {
            var retentionLength = GetOptionValue(Option.RetentionLength);

            return retentionLength != 4
                ? retentionLength / 60f
                : orig;
        }
    }
}