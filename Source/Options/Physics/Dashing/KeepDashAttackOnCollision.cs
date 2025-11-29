using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Dashing {
    [GooberHelperOption(Option.KeepDashAttackOnCollision)]
    public static class KeepDashAttackOnCollision {
        [ILHook(typeof(Player), "OnCollideH")]
        [ILHook(typeof(Player), "OnCollideV")]
        private static void keepDashAttack(ILContext il) {
            var cursor = new ILCursor(il);

            while(cursor.TryGotoNext(MoveType.After,
                instr => instr.MatchLdarg0(),
                instr => instr.MatchLdcR4(0),
                instr => instr.MatchStfld<Player>("dashAttackTimer")
            )) {
                cursor.Index--;
                cursor.EmitLdarg0();
                cursor.EmitDelegate(overrideDashAttack);
            }
        }

        private static float overrideDashAttack(float orig, Player player)
            => GetOptionBool(Option.KeepDashAttackOnCollision)
                ? player.dashAttackTimer
                : orig;
    }
}