using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Helpers;
using Celeste.Mod.GooberHelper.ModImports;
using Celeste.Mod.Helpers;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Jumping {
    [GooberHelperOption]
    public class AllDirectionHypersAndSupers : AbstractOption {
        public override OptionGroup HeadGroup { get; set; } = OptionGroup.AllowingThings;

        public enum Value {
            None,
            RequireGround,
            WorkWithCoyoteTimeAndDontRefill,
            WorkWithCoyoteTime
        }

        [ILHook(typeof(Player), "RedDashUpdate")]
        [ILHook(typeof(Player), "DashUpdate")]
        private static void allowAllDirectionHypersAndSupers(ILContext il) {
            //holy english essay double spacing

            var cursor = new ILCursor(il);

            var alwaysRefills = il.Method.Name == "Celeste.Player::RedDashUpdate";

            var superJumpLabel = cursor.DefineLabel();

            HookHelper.Begin(cursor, "implementing all direction hypers and supers");

            HookHelper.Move("finding where SuperJump is called", () => {
                cursor.GotoNextBestFit(MoveType.AfterLabel,
                    instr => instr.MatchLdarg0(),
                    instr => instr.MatchCallOrCallvirt<Player>("SuperJump"),
                    instr => instr.MatchLdcI4(0),
                    instr => instr.MatchRet()
                );
            });

            HookHelper.Do(() => {
                cursor.MarkLabel(superJumpLabel);
            });

            HookHelper.Move("finding where the method returns after calling SuperJump", () => {
                cursor.GotoNext(MoveType.After, instr => instr.MatchRet());
                cursor.MoveAfterLabels();
            });

            HookHelper.Do(() => {
                cursor.EmitLdarg0();
                cursor.EmitLdcI4(alwaysRefills ? 1 : 0); //why cant it just convert with (int)bool 😭???
                cursor.EmitDelegate(trySuperJump);

                cursor.EmitBrtrue(superJumpLabel);
            });

            HookHelper.End();
        }

        private static bool trySuperJump(Player player, bool alwaysRefills) {
            var allDirectionHypersAndSupersValue = GetOptionEnum<Value>(Option.AllDirectionHypersAndSupers);

            if(allDirectionHypersAndSupersValue == Value.None)
                return false;
            
            var extvarsJumpCount = ExtendedVariantMode.GetJumpCount?.Invoke() ?? 0;
            
            //inverse of original conditions
            if(!player.CanUnDuck || !Input.Jump.Pressed)
                return false;
            
            //real stuff
            var coyoteCondition =
                (player.jumpGraceTimer > 0f || extvarsJumpCount > 0) && 
                allDirectionHypersAndSupersValue != Value.RequireGround; //WorkWithCoyoteTime or WorkWithCoyoteTimeAndRefill

            var groundedCondition =
                (player.CollideCheck<JumpThru>(player.Position + Vector2.UnitY * player.Collider.Height) && player.CollideCheck<JumpThru>(player.Position + Vector2.UnitY)) ||
                player.CollideCheck<Solid>(player.Position + Vector2.UnitY);

            //dont take priority over normal wavedashes
            //this should check for when the player is just about to Collide with the ground but theyre on the pixel already
            if(coyoteCondition && groundedCondition && player.Speed.Y > 0)
                return false;

            if(!coyoteCondition && !groundedCondition)
                return false;

            //actual logic
            var canMaybeRefill = groundedCondition || allDirectionHypersAndSupersValue == Value.WorkWithCoyoteTime;

            if(alwaysRefills || canMaybeRefill && player.dashRefillCooldownTimer <= 0f && !player.Inventory.NoRefills)
                player.RefillDash();

            if(!groundedCondition && coyoteCondition && player.jumpGraceTimer <= 0f)
                ExtendedVariantMode.SetJumpCount?.Invoke(extvarsJumpCount - 1);

            return true;
        }
    }
}