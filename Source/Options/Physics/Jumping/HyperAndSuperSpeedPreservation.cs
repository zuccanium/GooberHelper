using Celeste.Mod.GooberHelper.Attributes;

namespace Celeste.Mod.GooberHelper.Options.Physics.Jumping {
    [GooberHelperOption(Option.HyperAndSuperSpeedPreservation)]
    public static class HyperAndSuperSpeedPreservation {
        public static void SetSpeed(Player player, Vector2 originalSpeed) {
            var hyperAndSuperSpeedPreservationValue = 0f;

            if(GetOptionBool(Option.HyperAndSuperSpeedPreservation))
                hyperAndSuperSpeedPreservationValue = originalSpeed.X;

            player.Speed.X = Utils.SignedAbsMax(
                player.Speed.X,
                hyperAndSuperSpeedPreservationValue
            );
        }
    }
}