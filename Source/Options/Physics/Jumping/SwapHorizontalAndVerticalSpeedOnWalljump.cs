using Celeste.Mod.GooberHelper.Attributes;

namespace Celeste.Mod.GooberHelper.Options.Physics.Jumping {
    [GooberHelperOption(Option.SwapHorizontalAndVerticalSpeedOnWalljump)]
    public static class SwapHorizontalAndVerticalSpeedOnWalljump {
        public static void SetSpeed(Player player, Vector2 originalSpeed, int dir) {
            if(!GetOptionBool(Option.SwapHorizontalAndVerticalSpeedOnWalljump))
                return;

            player.Speed.X = Utils.SignedAbsMax(
                player.Speed.X,
                originalSpeed.Y
            );
        }
    }
}