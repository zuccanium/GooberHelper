using System.Reflection;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.Helpers;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.GeneralHooks {
    public static class NormalUpdate {
        public static bool EnteredNotDuckingAreaLegally = false;
        public static bool RunningNormalUpdateJustForClimbing = false;

        private static FieldInfo f_NormalUpdate_RunningNormalUpdateJustForClimbing = typeof(NormalUpdate).GetField(nameof(RunningNormalUpdateJustForClimbing));

        public static int RunNormalUpdateJustForClimbing(Player player) {  
            RunningNormalUpdateJustForClimbing = true;

            var result = player.NormalUpdate();

            RunningNormalUpdateJustForClimbing = false;

            return result;
        }
        

        [ILHook]
        private static void patch_Player_NormalUpdate(ILContext il) {
            guardNormalUpdateForOnlyClimbing(new ILCursor(il));

            var cursor = new ILCursor(il);

            if(cursor.TryGotoNext(MoveType.After, 
                instr => instr.MatchLdarg0(),
                instr => instr.MatchCallOrCallvirt<Player>("get_Ducking"),
                instr => instr.MatchBrtrue(out var _)
            )) {
                cursor.Index--;
                cursor.EmitDelegate(setWasDucking);

                cursor.GotoNext(MoveType.After, instr => instr.MatchBrtrue(out var _));
            }
        }

        private static bool guardNormalUpdateForOnlyClimbing(ILCursor cursor) {            
            var climbingStuffStartLabel = cursor.DefineLabel();
            var afterHoldingCheckLabel = cursor.DefineLabel();
            var afterGrabCheckLabel = cursor.DefineLabel();
            var afterClimbCheckLabel = cursor.DefineLabel();

            if(!cursor.TryGotoNextBestFit(MoveType.Before,
                instr => instr.MatchStfld<Vector2>("Y"),
                instr => instr.MatchLdarg0(),
                instr => instr.MatchCallOrCallvirt<Player>("get_Holding")
            )) {
                return false;
            }

            cursor.GotoNext(MoveType.AfterLabel, instr => instr.MatchLdarg0());
            cursor.MarkLabel(climbingStuffStartLabel);

            if(!cursor.TryGotoNextBestFit(MoveType.After,
                instr => instr.MatchCallOrCallvirt<Player>("get_Holding"),
                instr => instr.MatchBrtrue(out afterHoldingCheckLabel)
            )) {
                return false;
            }

            if(!cursor.TryGotoNextBestFit(MoveType.After,
                instr => instr.MatchCallOrCallvirt<Player>("get_IsTired"), //yes i know this isnt grabcheck but i didnt want to do weird reflection to get the getter
                instr => instr.MatchBrtrue(out afterGrabCheckLabel)
            )) {
                return false;
            }

            if(!cursor.TryGotoNextBestFit(MoveType.After,
                instr => instr.MatchCallOrCallvirt<Player>("ClimbCheck"),
                instr => instr.MatchBrfalse(out afterClimbCheckLabel)
            )) {
                return false;
            }

            //skip past the first stuff
            cursor.Index = 0;
            cursor.EmitLdsfld(f_NormalUpdate_RunningNormalUpdateJustForClimbing);
            cursor.EmitBrtrue(climbingStuffStartLabel);

            ILLabel[] labels = [afterHoldingCheckLabel, afterGrabCheckLabel, afterClimbCheckLabel];

            foreach(var label in labels) {
                cursor.GotoLabel(label);
                
                var afterReturnLabel = cursor.DefineLabel();

                //if the guard is in place, return -1
                cursor.EmitLdsfld(f_NormalUpdate_RunningNormalUpdateJustForClimbing);
                cursor.EmitBrfalse(afterReturnLabel);
                cursor.EmitLdcI4(-1);
                cursor.EmitRet();
                cursor.MarkLabel(afterReturnLabel);
            }

            return true;
        }

        private static bool setWasDucking(bool ducking) {
            EnteredNotDuckingAreaLegally = !ducking;

            return GetOptionBool(Option.AllowCrouchedClimbGrabbing) || GetOptionBool(Option.AllowCrouchedHoldableGrabbing)
                ? false :
                ducking;
        }
    }
}