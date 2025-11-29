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

            ReverseBackboosts.Handle(self, force, player);
            HoldablesInheritSpeedWhenThrown.Handle(self, force, player);
        }
    }
}