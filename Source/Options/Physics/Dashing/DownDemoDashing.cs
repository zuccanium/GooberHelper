using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.Helpers;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Dashing {
    [GooberHelperOption(Option.DownDemoDashing)]
    public static class DownDemoDashing {
        [ILHook]
        private static void patch_Player_Update(ILContext il) {
            var cursor = new ILCursor(il);

            var afterDuckingLabel = cursor.DefineLabel();

            if(cursor.TryGotoNextBestFit(MoveType.After,
                instr => instr.MatchLdarg0(),
                instr => instr.MatchLdfld<Player>("jumpGraceTimer"),
                instr => instr.MatchLdcR4(0),
                instr => instr.MatchBgtUn(out afterDuckingLabel)
            )) {
                cursor.EmitLdarg0();
                cursor.EmitDelegate(shouldNotDuck);
                cursor.EmitBrtrue(afterDuckingLabel);
            }
        }

        private static bool shouldNotDuck(Player player)
            => GetOptionBool(Option.DownDemoDashing) && player.demoDashed;
    }
}