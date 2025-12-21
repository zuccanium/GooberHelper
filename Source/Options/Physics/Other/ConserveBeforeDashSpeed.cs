using Celeste.Mod.GooberHelper.Attributes;

namespace Celeste.Mod.GooberHelper.Options.Physics.Other {
    [GooberHelperOption]
    public class ConserveBeforeDashSpeed : AbstractOption {
        public override OptionGroup HeadGroup { get; set; } = OptionGroup.SpeedPreservation;
    }
}