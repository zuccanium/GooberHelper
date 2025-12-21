using System;

namespace Celeste.Mod.GooberHelper.Options.GeneralHooks {
    public static class HoldableSpeedInheritance {
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
                ? Math.Sign(holdableSpeedComponent) * Math.Max(Math.Abs(holdableSpeedComponent), Math.Abs(playerSpeedComponent) * 0.8f)
                : holdableSpeedComponent + playerSpeedComponent * optionValue / 100f;

            holdableSpeed -= holdableSpeed * direction;
            holdableSpeed += newSpeedComponent * direction;
        }
    }
}