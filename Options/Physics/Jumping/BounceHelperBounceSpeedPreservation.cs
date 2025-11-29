using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Extensions;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace Celeste.Mod.GooberHelper.Options.Physics.Jumping {
    [GooberHelperOption(Option.BounceHelperBounceSpeedPreservation)]
    public static class BounceHelperBounceSpeedPreservation {
        private static Vector2 beforeUpdateSpeed;

        [ILHook("BounceHelper", "Celeste.Mod.BounceHelper.BounceHelperModule", "bounce")]
        private static void boyoyoyoing(ILContext il) {
            var cursor = new ILCursor(il);

            cursor.EmitLdarg(1);
            cursor.EmitLdarg(2);
            cursor.EmitDelegate(overrideBounceSpeed);
            cursor.EmitStarg(2);
        }
        
        [ILHook]
        private static void patch_Player_Update(ILContext il) {
            var cursor = new ILCursor(il);

            cursor.EmitLdarg0();
            cursor.EmitDelegate(setOriginalSpeed);
        }

        private static Vector2 overrideBounceSpeed(Player player, Vector2 orig) {
            if(!GetOptionBool(Option.BounceHelperBounceSpeedPreservation))
                return orig;

            var sign = orig.Sign();
            var ext = player.GetExtensionFields();

            orig = orig.Abs();

            //down and vertical bounces
            if(orig.Y == 210 || orig.Y == 200)
                orig.Y = Math.Max(
                    Math.Max(
                        Math.Abs(ext.DashStickyRetentionExists ? ext.DashStickyRetentionSpeed.Y : 0f),
                        Math.Abs(beforeUpdateSpeed.Y)
                    ),
                    orig.Y
                );

            //horizontal bounces
            else if(orig.X == 320)
                orig.X = Math.Max(
                    Math.Max(
                        Math.Abs(ext.DashStickyRetentionExists ? ext.DashStickyRetentionSpeed.X : 0f),
                        Math.Abs(beforeUpdateSpeed.X)
                    ),
                    orig.X
                );
            
            //diagonal bounces
            else
                orig = orig.SafeNormalize() * Math.Max(
                    new Vector2(
                        Math.Max(
                            Math.Abs(ext.DashStickyRetentionExists ? ext.DashStickyRetentionSpeed.X : 0f),
                            Math.Abs(beforeUpdateSpeed.X)
                        ),
                        Math.Max(
                            Math.Abs(ext.DashStickyRetentionExists ? ext.DashStickyRetentionSpeed.Y : 0f),
                            Math.Abs(beforeUpdateSpeed.Y)
                        )
                    ).Length(),
                    orig.Length()
                );

            return orig * sign;
        }

        private static void setOriginalSpeed(Player player)
            => beforeUpdateSpeed = player.GetConservedSpeed();
    }
}