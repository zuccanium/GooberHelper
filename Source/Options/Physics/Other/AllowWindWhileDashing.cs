using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Extensions;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Other {
    [GooberHelperOption]
    public class AllowWindWhileDashing : AbstractOption {
        public enum Value {
            None,
            Velocity,
            Speed,
        }

        public static void OverrideDashSpeed(Player player, Vector2 originalDashSpeed, Vector2 originalSpeed, ref Vector2 orig) {
            var ext = player.GetExtensionFields();

            if(ext.DashWindBoost != Vector2.Zero) {
                orig.X += ext.DashWindBoost.X;
                
                if(!player.onGround)
                    orig.X += ext.DashWindBoost.Y;

                ext.DashWindBoost = Vector2.Zero;
            }
        }

        [ILHook]
        private static void patch_Player_WindMove(ILContext il) {
            var cursor = new ILCursor(il);

            cursor.EmitLdarg0();
            cursor.EmitLdarg1();
            cursor.EmitDelegate(setWindBoost);

            if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcI4(Player.StDash)))
                cursor.EmitDelegate(overrideStateCondition);
        }

        private static void setWindBoost(Player player, Vector2 move) {
            if(
                GetOptionEnum<Value>(Option.AllowWindWhileDashing) == Value.Speed &&
                player.DashDir == Vector2.Zero &&
                player.StateMachine.State == 2
            ) {
                player.GetExtensionFields().DashWindBoost = move / Engine.DeltaTime;
            }
        }

        private static int overrideStateCondition(int orig)
            => GetOptionEnum<Value>(Option.AllowWindWhileDashing) == Value.Velocity
                ? -1
                : orig;
    }
}

