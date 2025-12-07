using Celeste.Mod.GooberHelper.Attributes;

namespace Celeste.Mod.GooberHelper.Settings.Root.MouseJoystick {
    [GooberHelperSetting]
    public class Mode : AbstractEnum<Mode.ModeValue> {
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
    }
}