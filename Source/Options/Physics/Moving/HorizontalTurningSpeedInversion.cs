using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Moving {
    [GooberHelperOption(Option.HorizontalTurningSpeedInversion)]
    public static class HorizontalTurningSpeedInversion {
        [ILHook]
        private static void patch_Player_Update(ILContext il) {
            var cursor = new ILCursor(il);
            
            cursor.EmitLdarg0();
            cursor.EmitDelegate(setSpeed);
        }

        private static void setSpeed(Player player) {
            if(GetOptionBool(Option.HorizontalTurningSpeedInversion) && player.moveX == -Math.Sign(player.Speed.X))
                player.Speed.X *= -1;
        }
    }
}