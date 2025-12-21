using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.Helpers;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Moving {
    [GooberHelperOption]
    public class IgnoreForcemove : AbstractOption {
        [ILHook]
        private static void patch_Player_Update(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNextBestFit(MoveType.After,
                instr => instr.MatchLdarg0(),
                instr => instr.MatchLdfld<Player>("forceMoveX")
            )) {
                cursor.EmitDelegate(overrideForceMoveX);
            }
        }

        private static int overrideForceMoveX(int orig)
            => GetOptionBool(Option.IgnoreForcemove)
                ? Input.MoveX
                : orig;
    }
}