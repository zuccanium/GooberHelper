using System;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Extensions;
using Celeste.Mod.Helpers;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.GeneralHooks {
    public static class VerticalJumpSpeed {
        public enum Value {
            None = ReservedHybridEnumConstant + 0,
            DashSpeed = ReservedHybridEnumConstant + 1,
        }

        private static Vector2 originalSpeed;

        [ILHook(typeof(Player), "WallJump")]
        [ILHook(typeof(Player), "SuperJump")]
        [ILHook(typeof(Player), "Jump")]
        [ILHook(typeof(Player), "SuperWallJump")]
        private static void doStuff(ILContext il) {
            var cursor = new ILCursor(il);

            cursor.EmitLdarg0();
            cursor.EmitDelegate(setOriginalSpeed);

            if(cursor.TryGotoNextBestFit(MoveType.AfterLabel,
                instr => instr.MatchLdarg0(),
                instr => instr.MatchLdarg0(),
                instr => instr.MatchLdfld<Player>("Speed"),
                instr => instr.MatchLdarg0(),
                instr => instr.MatchCallOrCallvirt<Player>("get_LiftBoost")
            )) {
                cursor.EmitLdarg0();
                cursor.EmitLdcI4(il.Method.Name == "Celeste.Player::SuperJump" ? 1 : 0);
                cursor.EmitDelegate(setPlayerSpeed);
            }
        }

        private static void setPlayerSpeed(Player player, bool isSuperJump) {
            var downwardsOptionValue = GetOptionValue(Option.DownwardsJumpSpeedPreservationThreshold);
            var upwardsOptionValue = GetOptionValue(Option.UpwardsJumpSpeedPreservationThreshold);

            var doDownwardsStuff = Input.MoveY > 0 && originalSpeed.Y > 0 && (
                downwardsOptionValue == (int)Value.None
                    ? false

                : downwardsOptionValue == (int)Value.DashSpeed
                    ? player.StateMachine.state == Player.StDash

                : originalSpeed.Y >= downwardsOptionValue
            );

            var doUpwardsStuff = originalSpeed.Y < 0 && (
                upwardsOptionValue == (int)Value.None
                    ? false
                
                : upwardsOptionValue == (int)Value.DashSpeed
                    ? player.StateMachine.state == Player.StDash
                
                : originalSpeed.Y <= -upwardsOptionValue
            );

            if(isSuperJump && player.Ducking)
                player.Speed *= new Vector2(1.25f, 0.5f);

            //gaslighting my own mod
            //did you know that gaslighting was invented by john gas?
            if(doDownwardsStuff)
                originalSpeed.Y *= -1;

            if(GetOptionBool(Option.AdditiveVerticalJumpSpeed)) {
                player.Speed.Y = Math.Min(player.Speed.Y, player.Speed.Y + Math.Min(originalSpeed.Y, 0));
            } else {
                if(doDownwardsStuff || doUpwardsStuff)
                    player.Speed.Y = Math.Min(originalSpeed.Y, player.Speed.Y);
            }

            //soliddarking someone elses mod
            if(doDownwardsStuff)
                player.Speed.Y *= -1;
            
            if(isSuperJump && player.Ducking)
                player.Speed /= new Vector2(1.25f, 0.5f);
        }

        [OnHook]
        private static void patch_Player_NormalBegin(On.Celeste.Player.orig_NormalBegin orig, Player self) {
            var originalMaxFall = self.maxFall;
            
            orig(self);

            var downwardsOptionValue = GetOptionValue(Option.DownwardsJumpSpeedPreservationThreshold);

            if(
                downwardsOptionValue != (int)Value.None &&
                self.varJumpSpeed > 0f &&
                self.Speed.Y == self.varJumpSpeed &&
                self.varJumpTimer > 0f
            ) {
                self.maxFall = originalMaxFall;
            }
        }

        [ILHook]
        private static void patch_Player_NormalUpdate(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNextBestFit(MoveType.After,
                instr => instr.MatchLdarg0(),
                instr => instr.MatchLdfld<Player>("varJumpSpeed"),
                instr => instr.MatchCallOrCallvirt(out _) //math.min
            )) {
                cursor.EmitLdarg0();
                cursor.EmitDelegate(overrideVarJumpSpeed);
            }
        }
        
        private static void setOriginalSpeed(Player player)
            => originalSpeed = player.GetConservedSpeed();

        private static float overrideVarJumpSpeed(float orig, Player player) {
            if(GetOptionValue(Option.DownwardsJumpSpeedPreservationThreshold) == (int)Value.None)
                return orig;

            var varJumpSpeed = player.varJumpSpeed;

            return varJumpSpeed > 0 && !player.onGround
                ? varJumpSpeed
                : orig;
        }
    }
}