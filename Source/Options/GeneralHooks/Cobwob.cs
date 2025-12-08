using System;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.GeneralHooks {
    public static class Cobwob {
        private static float originalSpeed;
        private static float originalCobwobSpeed;

        [ILHook]
        private static void patch_Player_Update(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(130f))) {
                cursor.EmitLdarg0();
                cursor.EmitDelegate(storeSpeeds);
            }

            if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(27.5f))) {
                cursor.EmitLdarg0();
                cursor.EmitDelegate(overrideSpeed);
            }
        }

        private static float storeSpeeds(float orig, Player player) {
            originalSpeed = player.Speed.X;
            originalCobwobSpeed = orig;

            return orig;
        }

        private static float overrideSpeed(float orig, Player player) {
            var cobwobSpeedInversionValue = GetOptionEnum<CobwobSpeedInversionValue>(Option.CobwobSpeedInversion);

            if(
                cobwobSpeedInversionValue == CobwobSpeedInversionValue.None &&
                !GetOptionBool(Option.WallboostSpeedIsOppositeSpeed)
            ) return orig;

            if(player == null)
                return orig;

            var dir = Math.Sign(player.Speed.X);
            var newAbsoluteSpeed = Math.Max(originalCobwobSpeed, Math.Abs(originalSpeed));

            if(
                GetOptionBool(Option.WallboostSpeedIsOppositeSpeed) &&
                !GetOptionBool(Option.WallboostDirectionIsOppositeSpeed) &&
                player.wallBoostDir == Math.Sign(originalSpeed - 11f * Math.Sign(originalSpeed))
            ) {
                dir = -Math.Sign(originalSpeed);
            }
            
            if(player.wallSpeedRetentionTimer > 0.0 && cobwobSpeedInversionValue == CobwobSpeedInversionValue.WorkWithRetention) {
                var retainedSpeed = player.wallSpeedRetained;

                newAbsoluteSpeed = Math.Max(originalCobwobSpeed, Math.Abs(retainedSpeed));
            }

            player.Speed.X = dir * newAbsoluteSpeed;

            return orig;
        }
    }
}