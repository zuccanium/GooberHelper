using System;
using Celeste.Mod.GooberHelper.Attributes;

namespace Celeste.Mod.GooberHelper.Options.Physics.Other {
    [GooberHelperOption(Option.HoldablesInheritSpeedWhenThrown)]
    public static class HoldablesInheritSpeedWhenThrown {
        public static void AfterRelease(Holdable holdable, Vector2 force, Player player) {
            if(!GetOptionBool(Option.HoldablesInheritSpeedWhenThrown))
                return;

            var holdableSpeed = holdable.SpeedGetter.Invoke();
            var newLaunchSpeed = force.X * Math.Max(Math.Abs(holdableSpeed.X), Math.Abs(player.Speed.X) * 0.8f);

            holdable.SpeedSetter.Invoke(new Vector2(newLaunchSpeed, holdableSpeed.Y));
        }
    }
}