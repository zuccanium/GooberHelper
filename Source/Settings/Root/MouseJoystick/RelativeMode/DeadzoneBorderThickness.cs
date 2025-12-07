using Celeste.Mod.GooberHelper.Attributes;

namespace Celeste.Mod.GooberHelper.Settings.Root.MouseJoystick.RelativeMode {
    [GooberHelperSetting]
    public class DeadzoneBorderThickness : AbstractFloat {
        public override float Min { get; set; } = 0;
        public override float Max { get; set; } = 100;
        public override string Suffix { get; set; } = " px";
    }
}