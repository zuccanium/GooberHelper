using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Extensions;
using Celeste.Mod.GooberHelper.Options.Physics.Jumping;
using Celeste.Mod.Helpers;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.GeneralHooks {
    public static class Wallbounce {
        private static Vector2 originalSpeed;

        [ILHook]
        private static void patch_Player_SuperWallJump(ILContext il) {
            var cursor = new ILCursor(il);

            cursor.EmitLdarg0();
            cursor.EmitDelegate(setOriginalSpeed);

            if(cursor.TryGotoNextBestFit(MoveType.AfterLabel,
                instr => instr.MatchLdarg0(),
                instr => instr.MatchLdfld<Player>("Speed"),
                instr => instr.MatchLdarg0(),
                instr => instr.MatchCallOrCallvirt<Player>("get_LiftBoost")
            )) {
                cursor.EmitLdarg0();
                cursor.EmitDelegate(setPlayerSpeed);
            }
        }

        private static void setOriginalSpeed(Player player)
            => originalSpeed = player.GetConservedSpeed();

        private static void setPlayerSpeed(Player player) {
            WallbounceSpeedPreservation.InSuperWallJump(player, originalSpeed);
        }
    }
}