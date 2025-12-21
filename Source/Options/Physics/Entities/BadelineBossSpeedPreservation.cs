using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Extensions;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Entities {
    [GooberHelperOption]
    public class BadelineBossSpeedPreservation : AbstractOption {
        [ILHook]
        private static void patch_Player_AttractBegin(ILContext il) {
            var cursor = new ILCursor(il);

            cursor.EmitLdarg0();
            cursor.EmitDelegate(setOriginalSpeed);
        }

        [OnHook]
        private static void patch_Player_FinalBossPushLaunch(On.Celeste.Player.orig_FinalBossPushLaunch orig, Player self, int dir) {
            orig(self, dir);

            if(GetOptionBool(Option.BadelineBossSpeedPreservation))
                self.Speed.X = dir * Math.Max(Math.Abs(self.Speed.X), self.GetExtensionFields().AttractSpeedPreserved.Length());
        }

        private static void setOriginalSpeed(Player player)
            => player.GetExtensionFields().AttractSpeedPreserved = player.GetConservedSpeed();
    }
}