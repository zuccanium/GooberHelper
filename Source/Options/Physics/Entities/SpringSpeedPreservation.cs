using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Extensions;

namespace Celeste.Mod.GooberHelper.Options.Physics.Entities {
    [GooberHelperOption]
    public class SpringSpeedPreservation : AbstractOption {
        public enum Value {
            None,
            Preserve,
            Invert
        }

        [OnHook]
        private static void patch_Player_SuperBounce(On.Celeste.Player.orig_SuperBounce orig, Player self, float fromY) {
            var springSpeedPreservationValue = GetOptionEnum<Value>(Option.SpringSpeedPreservation);

            if(springSpeedPreservationValue == Value.None) {
                orig(self, fromY);

                return;
            }

            var originalSpeed = self.Speed;

            orig(self, fromY);

            self.Speed.X = originalSpeed.X;

            if(springSpeedPreservationValue == Value.Invert && self.moveX == -Math.Sign(self.Speed.X))
                self.Speed.X *= -1;
        }

        [OnHook]
        private static bool patch_Player_SideBounce(On.Celeste.Player.orig_SideBounce orig, Player self, int dir, float fromX, float fromY) {
            if(!GetOptionBool(Option.SpringSpeedPreservation))
                return orig(self, dir, fromX, fromY);

            var originalSpeed = self.GetConservedSpeed();

            var res = orig(self, dir, fromX, fromY);
            
            self.Speed.X = Utils.SignedAbsMax(self.Speed.X, originalSpeed.X);

            return res;
        }
    }
}