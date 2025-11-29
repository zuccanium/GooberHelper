using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Options.GeneralHooks;
using Celeste.Mod.Helpers;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Other {
    [GooberHelperOption(Option.AllowClimbingInDashState)]
    public static class AllowClimbingInDashState {
        [ILHook]
        private static void patch_Player_DashUpdate(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNextBestFit(MoveType.AfterLabel, 
                instr => instr.MatchLdarg0(),
                instr => instr.MatchLdflda<Player>("DashDir"),
                instr => instr.MatchLdfld<Vector2>("Y"),
                instr => instr.MatchCallOrCallvirt(((Func<float, float>)Math.Abs).Method),
                instr => instr.MatchLdcR4(0.1f)
            )) {
                var afterReturnLabel = cursor.DefineLabel();

                cursor.EmitLdarg0();
                cursor.EmitDelegate(maybeRunNormalUpdateJustForClimbing);

                cursor.EmitBrfalse(afterReturnLabel);

                cursor.EmitLdcI4(1);
                cursor.EmitRet();

                cursor.MarkLabel(afterReturnLabel);
            }
        }

        //returns true if the player is now in stclimb
        private static bool maybeRunNormalUpdateJustForClimbing(Player player)
            => GetOptionBool(Option.AllowClimbingInDashState) && player.DashDir != Vector2.Zero
                ? NormalUpdate.RunNormalUpdateJustForClimbing(player) == 1
                : false;
    }
}