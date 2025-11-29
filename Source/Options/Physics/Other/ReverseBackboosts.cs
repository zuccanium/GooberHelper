using System;
using Celeste.Mod.GooberHelper.Attributes;

namespace Celeste.Mod.GooberHelper.Options.Physics.Other {
    [GooberHelperOption(Option.ReverseBackboosts)]
    public static class ReverseBackboosts {
        public static void Handle(Holdable holdable, Vector2 force, Player player) {
            if(GetOptionBool(Option.ReverseBackboosts) && Math.Sign(force.X) == Math.Sign(player.Speed.X))
                player.Speed.X *= -1;
        }
    }
}