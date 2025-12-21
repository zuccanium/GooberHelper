using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Extensions;

namespace Celeste.Mod.GooberHelper.Options.Physics.Entities {
    [GooberHelperOption]
    public class PointBounceSpeedPreservation : AbstractOption {
        [OnHook]
        private static void patch_Player_PointBounce(On.Celeste.Player.orig_PointBounce orig, Player self, Vector2 from) {
            if(!GetOptionBool(Option.PointBounceSpeedPreservation)) {
                orig(self, from);

                return;
            }

            var originalSpeed = self.GetConservedSpeed();

            orig(self, from);

            self.Speed = self.Speed.SafeNormalize() * Math.Max(originalSpeed.Length(), self.Speed.Length());
        }
    }
}