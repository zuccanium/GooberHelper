using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Settings.Toggles;

namespace Celeste.Mod.GooberHelper.Settings.Root.MouseJoystick {
    [GooberHelperSetting]
    public class Mode : AbstractEnum {
        public enum ModeValue {
            None,
            Relative,
            Absolute,
        }

        public abstract class MouseJoystickModeHandler {
            public virtual Vector2 JoystickPosition { get; set; }
            public abstract void Update();
            public abstract void Render();
        }

        public override Type EnumType { get; set; } = typeof(ModeValue);
    }
}