using System;
using Celeste.Mod.GooberHelper.Attributes;

namespace Celeste.Mod.GooberHelper.Settings.Root.MouseJoystick.RelativeMode {
    [GooberHelperSetting]
    public class ClampBehavior : AbstractEnum<ClampBehavior.ClampBehaviorValue> {
        public enum ClampBehaviorValue {
            None,
            Screen,
            Circle,
        }

        public static void ModifyVirtualMousePosition(Monocle.Circle circle, ref bool needsReset, ref Vector2 fromCircle) {
            var mode = GooberHelperModule.Settings.MouseJoystick.RelativeMode.ClampBehavior;
            
            switch(mode) {
                case ClampBehaviorValue.Circle:
                    if(fromCircle.Length() > circle.Radius) {
                        fromCircle = fromCircle.SafeNormalize() * circle.Radius;

                        needsReset = true;
                    }

                break;
                case ClampBehaviorValue.Screen:
                    var screenPos = fromCircle + circle.Center;
                    
                    var clampedScreenPos = new Vector2(
                        Math.Clamp(screenPos.X, -Engine.Viewport.Width / 2, Engine.Viewport.Width / 2),
                        Math.Clamp(screenPos.Y, -Engine.Viewport.Height / 2, Engine.Viewport.Height / 2)
                    );

                    if(screenPos != clampedScreenPos) {
                        fromCircle = clampedScreenPos - circle.Center;

                        needsReset = true;
                    }

                break;
                case ClampBehaviorValue.None: break;
            }            
        }
    }
}