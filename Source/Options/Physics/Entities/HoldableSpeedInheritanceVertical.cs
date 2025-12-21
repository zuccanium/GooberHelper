using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Options.GeneralHooks;

namespace Celeste.Mod.GooberHelper.Options.Physics.Entities {
    [GooberHelperOption(Option.HoldableSpeedInheritanceVertical)]
    public static class HoldableSpeedInheritanceVertical {
        public static void AfterRelease(Holdable holdable, Vector2 force, ref Vector2 holdableSpeed, Player player)
            => HoldableSpeedInheritance.InheritSpeed(holdable, force, ref holdableSpeed, player, Vector2.UnitY);
    }
}