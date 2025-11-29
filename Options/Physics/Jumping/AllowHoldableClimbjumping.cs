using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.Helpers;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Jumping {
    [GooberHelperOption(Option.AllowHoldableClimbjumping)]
    public static class AllowHoldableClimbjumping {
        [ILHook(typeof(Player), "RedDashUpdate")]
        [ILHook(typeof(Player), "HitSquashUpdate")]
        [ILHook(typeof(Player), "NormalUpdate")]
        [ILHook(typeof(Player), "DashUpdate")]
        private static void allowHoldableClimbjumping(ILContext il) {
            var cursor = new ILCursor(il);

            for(var i = 0; i < 2; i++) {
                if(
                    cursor.TryGotoNextBestFit(MoveType.After,
                        instr => instr.MatchLdfld<Player>("Stamina"),
                        instr => instr.MatchLdcR4(0),
                        instr => instr.MatchBleUn(out var _),
                        instr => instr.MatchLdarg(0),
                        instr => instr.MatchCallvirt<Player>("get_Holding"),
                        instr => instr.MatchBrtrue(out var _)
                    )
                ) {
                    cursor.Index--;
                    cursor.EmitDelegate(overrideHoldable);
                }
            }
        }

        private static Holdable overrideHoldable(Holdable orig)
            => GetOptionBool(Option.AllowHoldableClimbjumping)
                ? null
                : orig;
    }
}