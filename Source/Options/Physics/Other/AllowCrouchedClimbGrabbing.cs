using System;
using System.Collections.Generic;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Helpers;
using Celeste.Mod.GooberHelper.Options.GeneralHooks;
using Celeste.Mod.Helpers;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace Celeste.Mod.GooberHelper.Options.Physics.Other {
    [GooberHelperOption]
    public class AllowCrouchedClimbGrabbing : AbstractOption {
        [ILHook]
        private static void patch_Player_NormalUpdate(ILContext il) {
            var cursor = new ILCursor(il);

            var afterClimbGrabbingLabel = cursor.DefineLabel();
            var afterSettingDuckingToFalseLabel = cursor.DefineLabel();

            HookHelper.Begin(cursor, "implementing allow crouched climb grabbing");

            HookHelper.Move("going before the check", () => {
                cursor.GotoNext(MoveType.After, instr => instr.MatchEndfinally());
                cursor.GotoNext(MoveType.AfterLabel,
                    instr => instr.MatchLdarg0(),
                    instr => instr.MatchLdflda<Player>("Speed"),
                    instr => instr.MatchLdfld<Vector2>("Y")
                );
            });

            HookHelper.Do(() => {
                cursor.EmitDelegate(doClimbGrabbingStuffCondition);
                cursor.EmitBrfalse(afterClimbGrabbingLabel);
            });

            //make it not set your ducking to false when it tries to grab
            HookHelper.Move("going before it sets your ducking to false", () => {
                cursor.GotoNextBestFit(MoveType.Before,
                    instr => instr.MatchLdarg0(),
                    instr => instr.MatchLdcI4(0),
                    instr => instr.MatchCallOrCallvirt<Player>("set_Ducking")
                );
            });

            HookHelper.Do(() => {
                cursor.EmitDelegate(getOptionValue);
                cursor.EmitBrtrue(afterSettingDuckingToFalseLabel);

                //this one is guaranteed to work if this code is even running; the previous move was satisfied
                cursor.GotoNext(MoveType.After, instr => instr.MatchCallOrCallvirt<Player>("set_Ducking"));
                cursor.MarkLabel(afterSettingDuckingToFalseLabel);
            });

            HookHelper.Move("going after the check", () => {
                cursor.GotoNextBestFit(MoveType.AfterLabel,
                    instr => instr.MatchLdarg0(),
                    instr => instr.MatchCallOrCallvirt<Player>("get_CanDash")
                    // instr => instr.MatchBrfalse(out var _)
                );
            });

            HookHelper.Do(() => {
                cursor.MarkLabel(afterClimbGrabbingLabel);
            });

            HookHelper.End();
        }

        private static bool doClimbGrabbingStuffCondition()
            => GetOptionBool(Option.AllowCrouchedClimbGrabbing) || NormalUpdate.EnteredNotDuckingAreaLegally;

        private static bool getOptionValue()
            => GetOptionBool(Option.AllowCrouchedClimbGrabbing);
    }
}