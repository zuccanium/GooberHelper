using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Other {
    [GooberHelperOption(Option.PickupSpeedInversion)]
    public static class PickupSpeedInversion {
        [ILHook]
        private static void patch_Player_Update(ILContext il) {
            var cursor = new ILCursor(il);

            cursor.EmitLdarg0();
            cursor.EmitDelegate(setFacingMaybe);
        }

        [ILHook]
        private static void patch_Player_PickupCoroutine(ILContext il) {
            var cursor = new ILCursor(il);

            cursor.TryGotoNext(MoveType.After, instr => instr.MatchStfld<Player>("Speed"));
            cursor.TryGotoNext(MoveType.Before, instr => instr.MatchStfld<Player>("Speed"));

            cursor.EmitDelegate(overrideSpeed);
        }

        private static Vector2 overrideSpeed(Vector2 orig)
            => GetOptionBool(Option.PickupSpeedInversion) && orig.X == -Input.MoveX
                ? orig * Utils.InvertX
                : orig;

        private static void setFacingMaybe(Player player) {
            if(player.StateMachine.State == Player.StPickup && Input.MoveX == -(int)player.Facing) 
                player.Facing = (Facings)(-(int)player.Facing);
        }
    }
}