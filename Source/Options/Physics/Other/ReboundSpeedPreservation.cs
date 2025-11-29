using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Extensions;

namespace Celeste.Mod.GooberHelper.Options.Physics.Other {
    [GooberHelperOption(Option.ReboundSpeedPreservation)]
    public static class ReboundSpeedPreservation {
        [OnHook]
        private static void patch_Player_ReflectBounce(On.Celeste.Player.orig_ReflectBounce orig, Player self, Vector2 direction) {
            if(!GetOptionBool(Option.ReboundSpeedPreservation)) {
                orig(self, direction);
                
                return;
            }

            var originalSpeed = self.GetConservedSpeed();

            orig(self, direction);
            
            var sign = direction.X == 0
                ? Utils.FirstSign(self.moveX, self.Speed.X)
                : Utils.FirstSign(self.Speed.X);

            self.Speed.X = sign * Utils.UnsignedAbsMax(
                self.Speed.X,
                originalSpeed.X
            );
        }

        [OnHook]
        private static void patch_Player_Rebound(On.Celeste.Player.orig_Rebound orig, Player self, int direction = 0) {
            if(!GetOptionBool(Option.ReboundSpeedPreservation)) {
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
        
        [OnHook]
        private static void patch_Player_PointBounce(On.Celeste.Player.orig_PointBounce orig, Player self, Vector2 from) {
            if(!GetOptionBool(Option.ReboundSpeedPreservation)) {
                orig(self, from);

                return;
            }

            var originalSpeed = self.GetConservedSpeed();

            orig(self, from);

            self.Speed = self.Speed.SafeNormalize() * Math.Max(originalSpeed.Length(), self.Speed.Length());
        }
    }
}