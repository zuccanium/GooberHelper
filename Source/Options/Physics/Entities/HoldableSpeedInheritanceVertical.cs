using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Options.GeneralHooks;

namespace Celeste.Mod.GooberHelper.Options.Physics.Entities {
    [GooberHelperOption]
    public class HoldableSpeedInheritanceVertical : AbstractOption {
        public override OptionType Type { get; set; } = OptionType.Float;
        public override float? LeftMax { get; set; } = 0f;
        public override float? RightMin { get; set; } = 0f;
        public override float Step { get; set; } = 5f;
        public override bool SkipLeftMax { get; set; } = true;
        public override bool SkipRightMin { get; set; } = true;
        public override string Suffix { get; set; } = "%";
        
        public static void AfterRelease(Holdable holdable, Vector2 force, ref Vector2 holdableSpeed, Player player)
            => HoldableSpeedInheritance.InheritSpeed(holdable, force, ref holdableSpeed, player, Vector2.UnitY);
    }
}