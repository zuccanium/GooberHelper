using System.Text.RegularExpressions;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Extensions;
using Celeste.Mod.GooberHelper.Options.Miscellaneous;

namespace Celeste.Mod.GooberHelper.Options.GeneralHooks {
    public static class RefillFreeze {
        private static Regex refillRoutineRegex = new("RefillRoutine");
        
        [OnHook]
        private static void patch_Celeste_Freeze(On.Celeste.Celeste.orig_Freeze orig, float time) {
            var refillFreezeLength = GetOptionValue(Option.RefillFreezeLength);
            var refillFreezeGameSuspension = GetOptionBool(Option.RefillFreezeGameSuspension);
            
            if(refillFreezeLength != 3f || refillFreezeGameSuspension) {
                //as long as all refill freeze freezeframe callers have "refillroutine" in their names and nothing else then this should work
                if(refillRoutineRegex.IsMatch(new System.Diagnostics.StackTrace().ToString())) {
                    if(refillFreezeLength != 3f)
                        time = refillFreezeLength / 60f;

                    if(GetOptionBool(Option.RefillFreezeGameSuspension)) {
                        var ext = Engine.Scene.GetExtensionFields();

                        ext.FreezeFrameFrozen = true;
                        ext.FreezeFrameFrozenInputs = new RefillFreezeGameSuspension.InputState();

                        return;
                    }
                }
            }

            orig(time);
        }
    }
}