using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Options.GeneralHooks;

namespace Celeste.Mod.GooberHelper.Options {
    [GooberHelperOption()]
    public static class RotatePlayerToSpeed {
        public static bool OnUpdateSprite(Player player) {
            if(GetOptionBool(Option.RotatePlayerToSpeed))
                PlayerRender.PlayerRotation = PlayerRender.PlayerRotationTarget = player.Speed == Vector2.Zero
                    ? 0f
                    : player.Speed.Angle() + MathF.PI / 2f;

            return false;
        }
    }
}