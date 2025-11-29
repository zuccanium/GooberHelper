using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Moving {
    [GooberHelperOption(Option.VerticalTurningSpeedInversion)]
    public static class VerticalTurningSpeedInversion {
        [ILHook]
        private static void patch_Player_Update(ILContext il) {
            var cursor = new ILCursor(il);
            
            cursor.EmitLdarg0();
            cursor.EmitDelegate(setSpeed);
        }

        private static void setSpeed(Player player) {
            //weird as hell
            if(GetOptionBool(Option.VerticalTurningSpeedInversion) && Input.MoveY != -Math.Sign(player.Speed.Y)) {
                if(player.varJumpTimer > 0 && player.Speed.Y < 0f)
                    player.varJumpTimer = 0f;

                player.Speed.Y *= -1;
            }
        }
    }
}