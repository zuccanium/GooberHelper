using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Helpers;
using Celeste.Mod.Helpers;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Other {
    [GooberHelperOption(Option.RemoveNormalEnd)]
    public static class RemoveNormalEnd {
        private static void patch_Player_NormalEnd(ILContext il) {
            var cursor = new ILCursor(il);

            HookHelper.Begin(cursor, "implementing remove normal end");
            
            var afterEverythingLabel = cursor.DefineLabel();

            HookHelper.Move("going before the actual code", () => {
                cursor.GotoNextBestFit(MoveType.AfterLabel,
                    instr => instr.MatchLdarg0(),
                    instr => instr.MatchLdcI4(0),
                    instr => instr.MatchStfld<Player>("wallBoostTimer")
                );
            });
            
            HookHelper.Do(() => {
                cursor.EmitDelegate(getOptionBool);
                cursor.EmitBrtrue(afterEverythingLabel);
            });

            HookHelper.Move("going after the actual code", () => {
                cursor.GotoNextBestFit(MoveType.After,
                    instr => instr.MatchLdarg0(),
                    instr => instr.MatchLdcI4(0),
                    instr => instr.MatchStfld<Player>("hopWaitX")
                );
            });

            HookHelper.Do(() => {
                cursor.MarkLabel(afterEverythingLabel);
            });

            HookHelper.End();
        }

        private static bool getOptionBool()
            => GetOptionBool(Option.RemoveNormalEnd);
    }
}