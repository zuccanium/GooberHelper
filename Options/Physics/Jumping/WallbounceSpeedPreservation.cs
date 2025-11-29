using Celeste.Mod.GooberHelper.Attributes;

namespace Celeste.Mod.GooberHelper.Options.Physics.Jumping {
    [GooberHelperOption(Option.WallbounceSpeedPreservation)]
    public static class WallbounceSpeedPreservation {
        public static void SetSpeed(Player player, Vector2 originalSpeed) {
            if(!GetOptionBool(Option.WallbounceSpeedPreservation))
                return;
            
            player.Speed.X = Utils.SignedAbsMax(
                player.Speed.X,
                player.beforeDashSpeed.X
            );
        }
    }
}