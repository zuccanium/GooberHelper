using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Moving {
    [GooberHelperOption(Option.DownwardsAirFrictionBehavior)]
    public static class DownwardsAirFrictionBehavior {
        [ILHook]
        private static void patch_Player_NormalUpdate(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(900f))) {
                cursor.EmitLdarg0();
                cursor.EmitLdloc(8);
                cursor.EmitDelegate(overrideFastFallSpeed);
            }
        }

        //400f is default movement-direction-aligned air friction
        //0.65f is the default multiplier on horizontal air friction while midair
        //they call me the magic number documenter
        private static float overrideFastFallSpeed(float value, Player player, float fastfallSpeed)
            => GetOptionBool(Option.DownwardsAirFrictionBehavior) && Math.Abs(player.Speed.Y) > fastfallSpeed && Math.Sign(player.Speed.Y) == Input.MoveY
                ? 400f * 0.65f
                : value;
    }
}