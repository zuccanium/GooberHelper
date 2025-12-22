using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Options.Physics.Entities;

namespace Celeste.Mod.GooberHelper.Options.GeneralHooks {
    public static class HoldableRelease {
        [OnHook]
        private static void patch_Holdable_Release(On.Celeste.Holdable.orig_Release orig, Holdable self, Vector2 force) {
            var holder = self.Holder as Entity;
                        
            orig(self, force);
            
            if(holder is not Player player)
                return;
            
            var holdableSpeedMaybe = self.SpeedGetter?.Invoke();
            var holdableSpeed = holdableSpeedMaybe ?? Vector2.Zero;

            ReverseBackboosts.AfterRelease(self, force, ref holdableSpeed, player);
            HoldableSpeedInheritanceHorizontal.AfterRelease(self, force, ref holdableSpeed, player);
            HoldableSpeedInheritanceVertical.AfterRelease(self, force, ref holdableSpeed, player);

            //dont set the speed if there wasnt a speed to begin with
            //the only reason it doesnt return earlier is because there might be other important code in the invoked methods that doesnt just change the holdable speed
            if(holdableSpeedMaybe is not Vector2)
                return;

            self.SpeedSetter?.Invoke(holdableSpeed);
        }
    }
}