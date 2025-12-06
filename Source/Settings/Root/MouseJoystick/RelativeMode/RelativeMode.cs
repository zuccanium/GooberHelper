namespace Celeste.Mod.GooberHelper.Settings.Root.MouseJoystick.RelativeMode {
    public static class RelativeMode {
        public static float OverrideAxis(On.Monocle.Binding.orig_Axis orig, Binding self, int gamepadIndex, float threshold) {
            return orig(self, gamepadIndex, threshold);

            // var settings = global::Celeste.Settings.Instance; //what the fuck??
    
            // var virtualPositionIsInRange = VirtualPosition.Length() >= MouseThreshold;

            // return 
            //     self == settings.Right
            //         ? virtualPositionIsInRange && VirtualPosition.X > threshold
            //             ? Math.Max(VirtualPosition.X, 0)
            //             : 0

            //     : self == settings.Left
            //         ? virtualPositionIsInRange && VirtualPosition.X < -threshold
            //             ? Math.Max(-VirtualPosition.X, 0)
            //             : 0
                
            //     : self == settings.Down
            //         ? virtualPositionIsInRange && VirtualPosition.Y > threshold
            //             ? Math.Max(VirtualPosition.Y, 0)
            //             : 0

            //     : self == settings.Up
            //         ? virtualPositionIsInRange && VirtualPosition.Y < -threshold
            //             ? Math.Max(-VirtualPosition.Y, 0)
            //             : 0
                
            //     : orig(self, gamepadIndex, threshold);
        }
    }
}