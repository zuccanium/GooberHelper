using Celeste.Mod.GooberHelper.Attributes;

namespace Celeste.Mod.GooberHelper.Settings.Root.MouseJoystick.RelativeMode {
    [GooberHelperSetting]
    public class DeadzoneRadius : AbstractFloat {
        public override float Min { get; set; } = 0;
        public override float Max { get; set; } = 100;
        public override float Step { get; set; } = 2;
        public override string Suffix { get; set; } = " %";
    }
}