using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Settings.Toggles;

namespace Celeste.Mod.GooberHelper.Settings.Root.MouseJoystick {
    [GooberHelperSetting]
    public class Mode : AbstractEnum {
        public enum MouseJoystickMode {
            None,
            Relative,
            Absolute,
        }

        public override Type EnumType { get; set; } = typeof(MouseJoystickMode);

        public override void CreateEntry(object container, bool inGame)
            => base.CreateEntry(container, inGame);
    }
}