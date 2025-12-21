using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Options.GeneralHooks;

namespace Celeste.Mod.GooberHelper.Options.Physics.Entities {
    [GooberHelperOption(Option.HoldableSpeedInheritanceHorizontal)]
    public static class HoldableSpeedInheritanceHorizontal {
        public static void AfterRelease(Holdable holdable, Vector2 force, ref Vector2 holdableSpeed, Player player)
            => HoldableSpeedInheritance.InheritSpeed(holdable, force, ref holdableSpeed, player, Vector2.UnitX);
    }
}