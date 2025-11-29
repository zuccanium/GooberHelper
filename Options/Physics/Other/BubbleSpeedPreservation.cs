using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Extensions;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Other {
    [GooberHelperOption(Option.BubbleSpeedPreservation)]
    public static class BubbleSpeedPreservation {
        [ILHook]
        private static void modifyDashSpeedThing(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(240f))) {
                cursor.EmitLdarg0();
                cursor.EmitDelegate(overrideSpeed);
            }
        }

        [ILHook(typeof(Player), "Boost")]
        [ILHook(typeof(Player), "RedBoost")]
        private static void boostin(ILContext il) {
            var cursor = new ILCursor(il);

            cursor.EmitLdarg0();
            cursor.EmitDelegate(preserveBoostSpeed);
        }

        private static void preserveBoostSpeed(Player player)
            => player.GetExtensionFields().BoostSpeedPreserved = player.Speed;

        private static float overrideSpeed(float orig, Player player) {
            if(!GetOptionBool(Option.BubbleSpeedPreservation))
                return orig;

            var ext = player.GetExtensionFields();

            orig = Math.Max(ext.BoostSpeedPreserved.Length(), orig);

            ext.BoostSpeedPreserved = Vector2.Zero;

            return orig;
        }
    }
}