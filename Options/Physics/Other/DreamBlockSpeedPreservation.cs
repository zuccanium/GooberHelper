using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Extensions;
using MonoMod.Utils;

namespace Celeste.Mod.GooberHelper.Options.Physics.Other {
    [GooberHelperOption(Option.DreamBlockSpeedPreservation)]
    public static class DreamBlockSpeedPreservation {
        [OnHook]
        private static void patch_Player_DreamDashBegin(On.Celeste.Player.orig_DreamDashBegin orig, Player self) {
            var originalSpeed = self.GetConservedSpeed();

            orig(self);

            var optionValue = GetOptionEnum<DreamBlockSpeedPreservationValue>(Option.DreamBlockSpeedPreservation);

            if(optionValue != DreamBlockSpeedPreservationValue.None) {
                var componentMax = self.Speed.Sign() * Vector2.Max(self.Speed.Abs(), originalSpeed.Abs());

                object _ = optionValue switch {
                    DreamBlockSpeedPreservationValue.Horizontal => self.Speed.X = componentMax.X,
                    DreamBlockSpeedPreservationValue.Vertical => self.Speed.Y = componentMax.Y,
                    DreamBlockSpeedPreservationValue.Both => self.Speed = componentMax,
                    DreamBlockSpeedPreservationValue.Magnitude => self.Speed = self.Speed.SafeNormalize() * Math.Max(originalSpeed.Length(), self.Speed.Length()),
                    _ => null
                };
                
                self.GetExtensionFields().PreservedDreamBlockSpeedMagnitude = self.Speed;
            }
        }

        [OnHook]
        private static int patch_Player_DreamDashUpdate(On.Celeste.Player.orig_DreamDashUpdate orig, Player self) {
            if(GetOptionBool(Option.DreamBlockSpeedPreservation)) {
                var correctSpeed = self.GetExtensionFields().PreservedDreamBlockSpeedMagnitude;

                if(self.Speed.X == -correctSpeed.X && Math.Abs(self.Speed.X) > 0) correctSpeed.X *= -1; else 
                if(self.Speed.Y == -correctSpeed.Y && Math.Abs(self.Speed.Y) > 0) correctSpeed.Y *= -1; else
                {
                    var dreamBlockType = self.dreamBlock.GetType().Name;
                    var data = DynamicData.For(self.dreamBlock);

                    //i know this is evil but also putting code to update the player speed to anything constant is evil too so it cancels out and its fine
                    self.Speed = dreamBlockType == "ConnectedDreamBlock" && data.Get<bool>("FeatherMode")
                        ? self.Speed.SafeNormalize() * correctSpeed.Length()
                        : correctSpeed;
                }
            }

            return orig(self);
        }
    }
}