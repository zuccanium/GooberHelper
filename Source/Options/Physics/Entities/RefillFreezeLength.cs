using Celeste.Mod.GooberHelper.Attributes;

namespace Celeste.Mod.GooberHelper.Options.Physics.Entities {
    [GooberHelperOption]
    public class RefillFreezeLength : AbstractOption {
        public override OptionGroup HeadGroup { get; set; } = OptionGroup.Special;
        
        public override OptionType Type { get; set; } = OptionType.Float;
        public override float DefaultValue { get; set; } = 3f;
        public override float? RightMin { get; set; } = 0f;
        public override float Step { get; set; } = 1f;
        public override string Suffix { get; set; } = "f";

        //implemented in GeneralHooks/RefillFreeze
    }
}