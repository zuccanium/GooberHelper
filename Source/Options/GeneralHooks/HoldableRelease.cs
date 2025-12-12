using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Options.Physics.Other;

namespace Celeste.Mod.GooberHelper.Options.GeneralHooks {
    public static class HoldableRelease {
        [OnHook]
        private static void patch_Holdable_Release(On.Celeste.Holdable.orig_Release orig, Holdable self, Vector2 force) {
            var holder = self.Holder as Entity;
                        
            orig(self, force);
            
            if(holder is not Player player)
                return;
            
            var holdableSpeed = self.SpeedGetter();

            ReverseBackboosts.AfterRelease(self, force, ref holdableSpeed, player);
            HoldableSpeedInheritanceHorizontal.AfterRelease(self, force, ref holdableSpeed, player);
            HoldableSpeedInheritanceVertical.AfterRelease(self, force, ref holdableSpeed, player);

            self.SpeedSetter.Invoke(holdableSpeed);
        }
    }
}