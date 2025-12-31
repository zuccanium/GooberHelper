using Celeste.Mod.GooberHelper.Attributes;

namespace Celeste.Mod.GooberHelper.Options.Physics.Jumping {
    [GooberHelperOption]
    public class WallbounceSpeedPreservation : AbstractOption {
        public static void InSuperWallJump(Player player, Vector2 originalSpeed) {
            if(!GetOptionBool(Option.WallbounceSpeedPreservation))
                return;
            
            player.Speed.X = Utils.SignedAbsMax(
                player.Speed.X,
                player.beforeDashSpeed.X
            );
        }
    }
}