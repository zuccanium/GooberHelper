using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Extensions;

namespace Celeste.Mod.GooberHelper.Options.Physics.Other {
    [GooberHelperOption(Option.ReflectBounceSpeedPreservation)]
    public static class ReflectBounceSpeedPreservation {
        [OnHook]
        private static void patch_Player_Rebound(On.Celeste.Player.orig_Rebound orig, Player self, int direction = 0) {
            if(!GetOptionBool(Option.ReflectBounceSpeedPreservation)) {
                orig(self, direction);

                return;
            }

            var originalSpeed = self.GetConservedSpeed();

            orig(self, direction);

            var sign = Utils.FirstSign(self.Speed.X, self.moveX, originalSpeed.X);
                        
            self.Speed.X = sign * Utils.UnsignedAbsMax(
                self.Speed.X,
                originalSpeed.X
            );
        }
    }
}