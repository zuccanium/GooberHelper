using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Extensions;
using Celeste.Mod.Helpers;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Entities {
    [GooberHelperOption]
    public class ExplodeLaunchSpeedPreservation : AbstractOption {
        public enum Value {
            None,
            Horizontal,
            Vertical,
            Both,
            Magnitude,
        }

        private static Vector2 originalSpeed;

        [ILHook]
        private static void patch_Player_ExplodeLaunch_Vector2_bool_bool(ILContext il) {
            var cursor = new ILCursor(il);

            cursor.EmitLdarg0();
            cursor.EmitDelegate(setOriginalSpeed);

            //right before it does the logic for bumper boosting
            if(cursor.TryGotoNextBestFit(MoveType.AfterLabel,
                instr => instr.MatchLdarg0(),
                instr => instr.MatchLdflda<Player>("Speed"),
                instr => instr.MatchLdfld<Vector2>("X"),
                instr => instr.MatchLdcR4(0)
            )) {
                cursor.EmitLdarg0();
                cursor.EmitDelegate(doStuff);
            }
        }

        private static void setOriginalSpeed(Player player)
            => originalSpeed = player.GetConservedSpeed();

        //they call me john goodatnamingstuff
        //i need to stop saying john everywhere
        private static void doStuff(Player player) {
            var explodeLaunchSpeedPreservationValue = GetOptionEnum<Value>(Option.ExplodeLaunchSpeedPreservation);
            
            if(explodeLaunchSpeedPreservationValue == Value.None)
                return;

            var componentMax = player.Speed.Sign() * Vector2.Max(player.Speed.Abs(), originalSpeed.Abs());

            object _ = explodeLaunchSpeedPreservationValue switch {
                Value.Horizontal => player.Speed.X = componentMax.X,
                Value.Vertical => player.Speed.Y = componentMax.Y,
                Value.Both => player.Speed = componentMax,
                Value.Magnitude => player.Speed = player.Speed.SafeNormalize() * Math.Max(originalSpeed.Length(), player.Speed.Length()),
                _ => null
            };

            //i need to leave this in
            //even in the rewrite
            if(player.level.Session.Area.SID == "alex21/Dashless+/1A Dashless but Spikier" && player.level.Session.Level == "b-06") {
                player.Speed.X = 0;
                player.Speed.Y = -330;
            }
        }
    }
}