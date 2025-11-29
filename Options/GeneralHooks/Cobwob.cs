using System;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.GeneralHooks {
    public static class Cobwob {
        private static float originalSpeed;

        [ILHook]
        private static void patch_Player_Update(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(130f))) {
                cursor.EmitLdarg0();
                cursor.EmitDelegate(storeSpeed);
            }

            if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(27.5f))) {
                cursor.EmitLdarg0();
                cursor.EmitDelegate(overrideSpeed);
            }
        }

        private static void storeSpeed(Player player)
            => originalSpeed = player.Speed.X;

        private static float overrideSpeed(float orig, Player player) {
            if(
                GetOptionValue(Option.CobwobSpeedInversion) == (int)CobwobSpeedInversionValue.None &&
                !GetOptionBool(Option.WallboostSpeedIsOppositeSpeed)
            ) return orig;

            if(player == null)
                return orig;

            var dir = Math.Sign(player.Speed.X);
            var newAbsoluteSpeed = Math.Max(130f, Math.Abs(originalSpeed));

            if(
                GetOptionBool(Option.WallboostSpeedIsOppositeSpeed) &&
                !GetOptionBool(Option.WallboostDirectionIsOppositeSpeed) &&
                player.wallBoostDir == Math.Sign(originalSpeed - 11f * Math.Sign(originalSpeed))
            ) {
                dir = -Math.Sign(originalSpeed);
            }
            
            if(player.wallSpeedRetentionTimer > 0.0 && GetOptionValue(Option.CobwobSpeedInversion) == (int)CobwobSpeedInversionValue.WorkWithRetention) {
                var retainedSpeed = player.wallSpeedRetained;

                newAbsoluteSpeed = Math.Max(130f, Math.Abs(retainedSpeed));
            }

            player.Speed.X = dir * newAbsoluteSpeed;

            return orig;
        }
    }
}