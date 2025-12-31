using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Extensions;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Entities {
    [GooberHelperOption]
    public class BadelineBossSpeedPreservation : AbstractOption {
        public enum Value {
            None,
            Preserve,
            Invert,
            DashAim,
            Aim,
        }

        [ILHook]
        private static void patch_Player_AttractBegin(ILContext il) {
            var cursor = new ILCursor(il);

            cursor.EmitLdarg0();
            cursor.EmitDelegate(setOriginalSpeed);
        }

        [OnHook]
        private static void patch_Player_FinalBossPushLaunch(On.Celeste.Player.orig_FinalBossPushLaunch orig, Player self, int dir) {
            orig(self, dir);

            var value = GetOptionEnum<Value>(Option.BadelineBossSpeedPreservation);

            if(value == Value.None)
                return;

            var originalLaunchSpeed = self.Speed;
            
            self.Speed.X = Utils.SignedAbsMax(self.Speed.X, self.GetExtensionFields().AttractSpeedPreserved.Length());

            if(value == Value.Invert && self.moveX == -Math.Sign(self.Speed.X))
                self.Speed.X *= -1;
            
            else if(value is Value.Aim or Value.DashAim) {
                var aim = Input.Aim.Value;

                if(aim == Vector2.Zero || value == Value.DashAim)
                    aim = Input.GetAimVector(self.Facing);

                self.Speed = aim * Math.Abs(self.Speed.X);

                // if(self.Speed.Y <= 0f)
                //     self.Speed.Y = Utils.SignedAbsMax(self.Speed.Y, originalLaunchSpeed.Y);
            }
        }

        private static void setOriginalSpeed(Player player) {
            var ext = player.GetExtensionFields();
            
            ext.AttractSpeedPreserved = player.GetConservedSpeed(ext);
        }
    }
}