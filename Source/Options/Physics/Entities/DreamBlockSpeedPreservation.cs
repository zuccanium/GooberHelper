using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Extensions;
using MonoMod.Utils;

namespace Celeste.Mod.GooberHelper.Options.Physics.Entities {
    [GooberHelperOption]
    public class DreamBlockSpeedPreservation : AbstractOption {
        public override OptionGroup HeadGroup { get; set; } = OptionGroup.SpeedPreservation;
        
        public enum Value {
            None,
            Horizontal,
            Vertical,
            Both,
            Magnitude,
        }

        [OnHook]
        private static void patch_Player_DreamDashBegin(On.Celeste.Player.orig_DreamDashBegin orig, Player self) {
            var originalSpeed = self.GetConservedSpeed();

            orig(self);

            var optionValue = GetOptionEnum<Value>(Option.DreamBlockSpeedPreservation);

            if(optionValue != Value.None) {
                var componentMax = self.Speed.Sign() * Vector2.Max(self.Speed.Abs(), originalSpeed.Abs());

                object _ = optionValue switch {
                    Value.Horizontal => self.Speed.X = componentMax.X,
                    Value.Vertical => self.Speed.Y = componentMax.Y,
                    Value.Both => self.Speed = componentMax,
                    Value.Magnitude => self.Speed = self.Speed.SafeNormalize() * Math.Max(originalSpeed.Length(), self.Speed.Length()),
                    _ => null
                };
                
                self.GetExtensionFields().PreservedDreamBlockSpeedMagnitude = self.Speed;
            }
        }

        [OnHook]
        private static int patch_Player_DreamDashUpdate(On.Celeste.Player.orig_DreamDashUpdate orig, Player self) {
            if(!GetOptionBool(Option.DreamBlockSpeedPreservation))
                return orig(self);

            var correctSpeed = self.GetExtensionFields().PreservedDreamBlockSpeedMagnitude;

            if(self.Speed.X == -correctSpeed.X && Math.Abs(self.Speed.X) > 0f)
                correctSpeed.X *= -1f;
            
            else if(self.Speed.Y == -correctSpeed.Y && Math.Abs(self.Speed.Y) > 0f)
                correctSpeed.Y *= -1f;
            
            else {
                var dreamBlockType = self.dreamBlock.GetType().Name;
                var data = DynamicData.For(self.dreamBlock);

                //i know this is evil but also putting code to update the player speed to anything constant is evil too so it cancels out and its fine
                self.Speed = dreamBlockType == "ConnectedDreamBlock" && data.Get<bool>("FeatherMode")
                    ? self.Speed.SafeNormalize() * correctSpeed.Length()
                    : correctSpeed;
            }

            return orig(self);
        }
    }
}