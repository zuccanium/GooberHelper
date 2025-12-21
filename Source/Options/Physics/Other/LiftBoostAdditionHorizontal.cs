using Celeste.Mod.GooberHelper.Attributes;

namespace Celeste.Mod.GooberHelper.Options.Physics.Other {
    [GooberHelperOption]
    public class LiftboostAdditionHorizontal : AbstractOption {
        public override OptionType Type { get; set; } = OptionType.Float;
        public override float? LeftMax { get; set; } = 0f;
        public override float? RightMin { get; set; } = 0f;
        public override float Step { get; set; } = 5f;
        public override string Suffix { get; set; } = "px/s";

        //implemented in in GeneralHooks/LiftBoost.cs
    }
}