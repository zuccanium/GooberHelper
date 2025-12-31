using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Extensions;
using Celeste.Mod.Helpers;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Other {
    [GooberHelperOption]
    public class StackableUltras : AbstractOption {
        [ILHook]
        private static void patch_Player_OnCollideV(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNextBestFit(MoveType.After,
                instr => instr.MatchLdcR4(1.2f),
                instr => instr.MatchMul()
            )) {
                cursor.Index--;

                cursor.EmitLdarg0();
                cursor.EmitDelegate(overrideUltraMultiplier);
            }
        }

        [ILHook]
        private static void patch_Player_DashEnd(ILContext il) {
            var cursor = new ILCursor(il);

            cursor.EmitLdarg0();
            cursor.EmitDelegate(incrementUltraCounter);
        }

        private static void incrementUltraCounter(Player player) {
            var ext = player.GetExtensionFields();

            //pattern matching is the best thing in any language
            //this is actually just so hot
            //hot probably isnt the best word for this
            //but i cant think of anything that describes my sheer attraction to this feature
            //its so good
            ext.StackedUltras = player.DashDir is {Y: > 0, X: not 0}
                ? ext.StackedUltras + 1
                : 0;
        }

        private static float overrideUltraMultiplier(float orig, Player player) {
            if(!GetOptionBool(Option.StackableUltras))
                return orig;
            
            var exponent = player.GetExtensionFields().StackedUltras;
            
            if(player.StateMachine.State == Player.StDash)
                exponent++;
            
            var result = MathF.Pow(orig, exponent);

            Utils.Log($"ultra multiplier is {result} with exponent {exponent}");

            return result;
        }
    }
}