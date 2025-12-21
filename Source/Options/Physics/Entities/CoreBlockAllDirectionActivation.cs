using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.Helpers;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Entities {
    [GooberHelperOption(Option.CoreBlockAllDirectionActivation)]
    public static class CoreBlockAllDirectionActivation {
        [ILHook]
        private static void patch_BounceBlock_WindUpPlayerCheck(ILContext il) {
            var cursor = new ILCursor(il);

            for(var i = 0; i < 2; i++) {
                if(cursor.TryGotoNextBestFit(MoveType.After, instr => instr.MatchCallOrCallvirt<StateMachine>("get_State")))
                    cursor.EmitDelegate(overrideState);
            }
        }

        private static int overrideState(int orig)
            => GetOptionBool(Option.CoreBlockAllDirectionActivation)
                ? 1
                : orig;
    }
}