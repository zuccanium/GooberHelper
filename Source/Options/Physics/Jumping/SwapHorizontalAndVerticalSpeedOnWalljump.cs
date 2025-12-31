using Celeste.Mod.GooberHelper.Attributes;

namespace Celeste.Mod.GooberHelper.Options.Physics.Jumping {
    [GooberHelperOption]
    public class SwapHorizontalAndVerticalSpeedOnWalljump : AbstractOption {
        public static void InWallJump(Player player, Vector2 originalSpeed, int dir) {
            if(!GetOptionBool(Option.SwapHorizontalAndVerticalSpeedOnWalljump))
                return;

            player.Speed.X = Utils.SignedAbsMax(
                player.Speed.X,
                originalSpeed.Y
            );

            player.Speed.Y = Utils.SignedAbsMax(
                player.Speed.Y,
                originalSpeed.X
            );
        }
    }
}