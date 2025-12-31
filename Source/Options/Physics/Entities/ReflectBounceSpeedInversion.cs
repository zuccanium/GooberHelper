using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Extensions;

namespace Celeste.Mod.GooberHelper.Options.Physics.Entities {
    [GooberHelperOption]
    public class ReflectBounceSpeedInversion : AbstractOption {
        [OnHook]
        private static void patch_Player_ReflectBounce(On.Celeste.Player.orig_ReflectBounce orig, Player self, Vector2 direction) {
            if(!GetOptionBool(Option.ReflectBounceSpeedInversion)) {
                orig(self, direction);

                return;
            }

            var originalSpeed = self.GetConservedSpeed();

            orig(self, direction);

            var sign = Utils.FirstSign(self.Speed.X, self.moveX, (int)self.Facing);
                        
            self.Speed.X = sign * Utils.UnsignedAbsMax(
                self.Speed.X,
                originalSpeed.X
            );
        }
    }
}