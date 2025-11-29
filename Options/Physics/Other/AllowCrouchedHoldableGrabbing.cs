using System;
using System.Collections.Generic;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Helpers;
using Celeste.Mod.GooberHelper.Options.GeneralHooks;
using Celeste.Mod.Helpers;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Other {
    [GooberHelperOption(Option.AllowCrouchedHoldableGrabbing)]
    public static class AllowCrouchedHoldableGrabbing {
        [ILHook]
        private static void patch_Player_Pickup(ILContext il) {
            var cursor = new ILCursor(il);

            var endLabel = cursor.DefineLabel();

            HookHelper.Begin(cursor, "allowing crouched holdable grabbing");

            HookHelper.Move("moving before set_Ducking", () => {
                cursor.GotoNext(MoveType.AfterLabel,
                    instr => instr.MatchLdarg0(),
                    instr => instr.MatchLdcI4(0),
                    instr => instr.MatchCallOrCallvirt<Player>("set_Ducking")
                );
            });

            HookHelper.Do(() => {
                cursor.EmitDelegate(getOptionBool);
                cursor.EmitBrtrue(endLabel);
            });

            HookHelper.Move("setting label after set_Ducking", () => {
                cursor.GotoNext(MoveType.After, instr => instr.MatchCallOrCallvirt<Player>("set_Ducking"));
            });

            HookHelper.Do(() => {
                cursor.MarkLabel(endLabel);
            });

            HookHelper.End();
        }

        [ILHook]
        private static void patch_Player_LaunchUpdate(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNext(MoveType.After,
                instr => instr.MatchCallOrCallvirt<Player>("get_Ducking"),
                instr => instr.MatchBrtrue(out var _)
            )) {
                cursor.Index--;

                cursor.EmitDelegate(overrideGetDucking);
            }
        }

        [ILHook]
        private static void patch_Player_NormalUpdate(ILContext il) {
            var cursor = new ILCursor(il);
            var afterHoldableGrabbingLabel = cursor.DefineLabel();

            HookHelper.Begin(cursor, "implementing allow crouched holdable grabbing");

            HookHelper.Move("going before the loop", () => {
                cursor.GotoNextBestFit(MoveType.AfterLabel,
                    instr => instr.MatchLdarg0(),
                    instr => instr.MatchCallOrCallvirt<Entity>("get_Scene"),
                    instr => instr.MatchCallOrCallvirt<Scene>("get_Tracker"),
                    instr => instr.MatchCallOrCallvirt<Tracker>("GetComponents"),
                    instr => instr.MatchCallOrCallvirt<List<Component>>("GetEnumerator")
                );
            });

            HookHelper.Do(() => {
                cursor.EmitDelegate(doHoldableGrabbingStuffCondition);
                cursor.EmitBrfalse(afterHoldableGrabbingLabel);
            });

            HookHelper.Move("going after the loop", () => {
                cursor.GotoNextBestFit(MoveType.After, instr => instr.MatchEndfinally());
                cursor.MoveAfterLabels();
            });

            HookHelper.Do(() => {
                cursor.MarkLabel(afterHoldableGrabbingLabel);
            });

            HookHelper.Move("going after the get ducking check", () => {
                cursor.GotoNextBestFit(MoveType.After,
                    instr => instr.MatchLdarg0(),
                    instr => instr.MatchLdfld<Player>("onGround"),
                    instr => instr.MatchBrfalse(out var _),
                    instr => instr.MatchLdarg0(),
                    instr => instr.MatchCallOrCallvirt<Player>("get_Ducking")
                );
            });

            HookHelper.Do(() => {
                cursor.EmitDelegate(overrideGetDucking);
            });

            HookHelper.End();
        }

        private static bool doHoldableGrabbingStuffCondition()
            => GetOptionBool(Option.AllowCrouchedHoldableGrabbing) || NormalUpdate.EnteredNotDuckingAreaLegally;

        private static bool getOptionBool()
            => GetOptionBool(Option.AllowCrouchedHoldableGrabbing);
        
        private static bool overrideGetDucking(bool orig)
            => GetOptionBool(Option.AllowCrouchedHoldableGrabbing)
                ? false
                : orig;
    }
}