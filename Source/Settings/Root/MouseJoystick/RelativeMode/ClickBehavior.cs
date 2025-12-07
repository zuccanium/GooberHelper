using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Settings.Toggles;

namespace Celeste.Mod.GooberHelper.Settings.Root.MouseJoystick.RelativeMode {
    [GooberHelperSetting]
    public class ClickBehavior : AbstractEnum {
        public enum ClickBehaviorValue {
            None,
            ResetOnClick,
            RequireToMove,
        }

        public override Type EnumType { get; set; } = typeof(ClickBehaviorValue);

        public static void ModifyVirtualMousePosition(Monocle.Circle circle, ref bool needsReset, ref Vector2 fromCircle) {
            var mode = GooberHelperModule.Settings.MouseJoystick.RelativeMode.ClickBehavior;
            
            switch(mode) {
                case ClickBehaviorValue.ResetOnClick:
                    if(MInput.Mouse.PressedLeftButton) {
                        fromCircle = Vector2.Zero;

                        needsReset = true;
                    }

                    break;
                
                case ClickBehaviorValue.RequireToMove:
                    if(!MInput.Mouse.CheckLeftButton) {
                        fromCircle = Vector2.Zero;

                        needsReset = true;
                    }

                    break;
                
                case ClickBehaviorValue.None: break;
            }            
        }
    }
}