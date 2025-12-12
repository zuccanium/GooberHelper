using System;
using Celeste.Mod.GooberHelper.Attributes;

namespace Celeste.Mod.GooberHelper.Options.Physics.Other {
    [GooberHelperOption(Option.ReverseBackboosts)]
    public static class ReverseBackboosts {
        public static void AfterRelease(Holdable holdable, Vector2 force, ref Vector2 speed, Player player) {
            if(GetOptionBool(Option.ReverseBackboosts) && Math.Sign(force.X) == Math.Sign(player.Speed.X))
                player.Speed.X *= -1;
        }
    }
}