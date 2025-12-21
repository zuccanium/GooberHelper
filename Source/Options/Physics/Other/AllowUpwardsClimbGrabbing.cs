using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Helpers;
using Celeste.Mod.Helpers;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Other {
    [GooberHelperOption]
    public class AllowUpwardsClimbGrabbing : AbstractOption {
        public enum Value {
            None,
            WhileHoldingUp,
            Always
        }

        private static bool legallyClimbjumping = false;

        [ILHook]
        private static void patch_Player_NormalUpdate(ILContext il) {
            var cursor = new ILCursor(il);

            ILLabel failedSpeedCheckLabel = null;
            
            HookHelper.Begin(cursor, "implementing allow upwards climb grabbing");

            HookHelper.Move("moving into vertical speed check", () => {
                cursor.GotoNext(MoveType.After, instr => instr.MatchEndfinally());
                
                cursor.GotoNextBestFit(MoveType.After,
                    instr => instr.MatchLdarg0(),
                    instr => instr.MatchLdflda<Player>("Speed"),
                    instr => instr.MatchLdfld<Vector2>("Y")
                );
            });

            HookHelper.Do(() => {
                cursor.EmitDelegate(overrideVerticalSpeedInCondition);
            });

            //add a CanUnDuck to make sure the player doesnt clip into the ceiling
            HookHelper.Move("moving after less than branch", () => {
                cursor.GotoNext(MoveType.After, instr => instr.MatchBltUn(out failedSpeedCheckLabel));
            });
            
            HookHelper.Do(() => {
                cursor.EmitLdarg0();
                cursor.EmitDelegate(canGrab);
                cursor.EmitBrfalse(failedSpeedCheckLabel);
            });

            HookHelper.End();
        }

        private static float overrideVerticalSpeedInCondition(float orig) {
            legallyClimbjumping = orig >= 0f;
            
            return GetOptionBool(Option.AllowUpwardsClimbGrabbing)
                ? float.MaxValue
                : orig;
        }

        private static bool canGrab(Player player)
            => GetOptionEnum<Value>(Option.AllowUpwardsClimbGrabbing) switch {
                Value.WhileHoldingUp => legallyClimbjumping || player.CanUnDuck && Input.MoveY < 0f,
                Value.Always => legallyClimbjumping || player.CanUnDuck,
                _ => true,
            };
    }
}