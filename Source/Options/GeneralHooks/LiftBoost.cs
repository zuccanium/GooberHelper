using System;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.GeneralHooks {
    public static class LiftBoost {
        [ILHook]
        private static void patch_Player_get_LiftBoost(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNext(MoveType.AfterLabel, instr => instr.MatchRet())) {
                cursor.EmitLdarg0();
                cursor.EmitDelegate(overrideLiftboost);
            }
        }

        private static Vector2 overrideLiftboost(Vector2 liftboost, Player player) {
            liftboost += new Vector2(GetOptionValue(Option.LiftboostAdditionHorizontal), GetOptionValue(Option.LiftboostAdditionVertical));

            if(GetOptionBool(Option.AdvantageousLiftboost)) {
                if(Math.Abs(player.Speed.X - liftboost.X) > Math.Abs(player.Speed.X + liftboost.X))
                    liftboost.X *= -1;

                if(Math.Abs(player.Speed.Y - liftboost.Y) > Math.Abs(player.Speed.Y + liftboost.Y))
                    liftboost.Y *= -1;
            }

            return liftboost;
        }
    }
}