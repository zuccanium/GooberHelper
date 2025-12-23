using System;

namespace Celeste.Mod.GooberHelper.Options.GeneralHooks {
    public class HoldableSpeedInheritance : AbstractOption {
        public override OptionType Type { get; set; } = OptionType.Float;
        public override Type EnumType { get; set; } = typeof(Value);
        public override float DefaultValue { get; set; } = (float)Value.None;
        public override float? LeftMax { get; set; } = 0f;
        public override float? RightMin { get; set; } = 0f;
        public override float Step { get; set; } = 5f;
        public override bool SkipLeftMax { get; set; } = true;
        public override bool SkipRightMin { get; set; } = true;
        public override string Suffix { get; set; } = "%";

        public enum Value {
            None = ReservedHybridEnumConstant + 0,
            MatchPlayer = ReservedHybridEnumConstant + 1
        }

        public static void InheritSpeed(Holdable holdable, Vector2 force, ref Vector2 holdableSpeed, Player player, Vector2 direction) {
            var option = direction == Vector2.UnitX
                ? Option.HoldableSpeedInheritanceHorizontal
                : Option.HoldableSpeedInheritanceVertical;
            
            var optionValue = GetOptionValue(option);

            if(optionValue == 0f || optionValue == (int)Value.None)  
                return;

            var holdableSpeedComponent = Vector2.Dot(holdableSpeed, direction);
            var playerSpeedComponent = Vector2.Dot(player.Speed, direction);

            var newSpeedComponent = optionValue == (int)Value.MatchPlayer
                ? playerSpeedComponent - 80f * Math.Sign(playerSpeedComponent) //80f for the speed lost while throwing (i know this isnt a perfect solution but it works honestly)
                : holdableSpeedComponent + playerSpeedComponent * optionValue / 100f;

            holdableSpeed -= holdableSpeed * direction;
            holdableSpeed += newSpeedComponent * direction;
        }
    }
}