using Celeste.Mod.GooberHelper.Attributes;

namespace Celeste.Mod.GooberHelper.Options.Physics.Jumping {
    [GooberHelperOption]
    public class UpwardsJumpSpeedPreservationThreshold : AbstractOption {
        public override OptionType Type { get; set; } = OptionType.Float;
        public override float? RightMin { get; set; } = 0f;
        public override float? RightMax { get; set; } = 240f;
        public override float Step { get; set; } = 10f;
        public override string Suffix { get; set; } = "px/s";
        
        //implemented in GeneralHooks/VerticalJumpSpeed.cs
    }
}