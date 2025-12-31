using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Other {
    [GooberHelperOption]
    public class ClimbingSpeedPreservation : AbstractOption {
        [ILHook]
        private static void patch_Player_ClimbBegin(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(0f))) {
                cursor.EmitLdarg0();
                cursor.EmitDelegate(overrideHorizontalSpeedNullification);
            }

            if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(0.2f)))
                cursor.EmitDelegate(overrideVerticalSpeedMultiplier);
            
            if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(0.1f)))
                cursor.EmitDelegate(overrideClimbNoMoveTimer);
        }

        [ILHook]
        private static void patch_Player_ClimbUpdate(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(-45f))) {
                cursor.EmitLdarg0();
                cursor.EmitDelegate(overrideUpwardsSpeed);
            }

            if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(80f))) {
                cursor.EmitLdarg0();
                cursor.EmitDelegate(overrideDownwardsSpeed);
            }
        }

        //patch_Player_ClimbBegin
        private static float overrideHorizontalSpeedNullification(float orig, Player player)
            => GetOptionBool(Option.ClimbingSpeedPreservation) ? player.Speed.X : orig;

        private static float overrideVerticalSpeedMultiplier(float orig)
            => GetOptionBool(Option.ClimbingSpeedPreservation) ? 1f : orig;

        private static float overrideClimbNoMoveTimer(float orig)
            => GetOptionBool(Option.ClimbingSpeedPreservation) ? 0f : orig;

        
        //patch_Player_ClimbUpdate
        private static float overrideUpwardsSpeed(float orig, Player player)
            => GetOptionBool(Option.ClimbingSpeedPreservation) ? Math.Min(player.Speed.Y, orig) : orig;

        private static float overrideDownwardsSpeed(float orig, Player player)
            => GetOptionBool(Option.ClimbingSpeedPreservation) ? Math.Max(player.Speed.Y, orig) : orig;
    }
}