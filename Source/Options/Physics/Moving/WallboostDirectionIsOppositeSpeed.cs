using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Moving {
    [GooberHelperOption(Option.WallboostDirectionIsOppositeSpeed)]
    public static class WallboostDirectionIsOppositeSpeed {
        [ILHook]
        private static void patch_Player_ClimbJump(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNext(MoveType.Before, instr => instr.MatchStfld<Player>("wallBoostDir"))) {
                cursor.EmitLdarg0();
                cursor.EmitDelegate(overrideWallBoostDir);
            }
        } 

        private static int overrideWallBoostDir(int orig, Player player)
            => GetOptionBool(Option.WallboostDirectionIsOppositeSpeed)
                ? -Math.Sign(player.Speed.X)
                : orig;
        
        //partially implemented in GeneralHooks/Cobwob.cs
    }
}