using System;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Extensions;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace Celeste.Mod.GooberHelper.Options.GeneralHooks {
    public static class DashStickyRetention {
        [ILHook(typeof(Player), "OnCollideH")]
        [ILHook(typeof(Player), "OnCollideV")]
        private static void setOnCollide(ILContext il) {
            var cursor = new ILCursor(il);

            cursor.EmitLdarg0();
            cursor.EmitLdarg1();
            cursor.EmitDelegate(setDashStickyRetentionOnCollide);
        }


        [ILHook]
        private static void patch_Player_DashEnd(ILContext il) {
            var cursor = new ILCursor(il);

            cursor.EmitLdarg0();
            cursor.EmitDelegate(clearDashStickyRetention);
        }

        [ILHook]
        private static void patch_Player_NormalEnd(ILContext il) {
            var cursor = new ILCursor(il);

            cursor.EmitLdarg0();
            cursor.EmitDelegate(setDashStickyRetentionOnNormalEnd);
        }

        private static void setDashStickyRetentionOnNormalEnd(Player player) {
            if(player.StateMachine.State == 2 && player.wallSpeedRetentionTimer > 0) {
                var ext = player.GetExtensionFields();
                
                ext.DashStickyRetentionExists = true;
                ext.DashStickyRetentionDirection = new Vector2(Math.Sign(player.wallSpeedRetained), 0);
                ext.DashStickyRetentionSpeed = new Vector2(player.wallSpeedRetained, 0);
            }
        }

        private static void setDashStickyRetentionOnCollide(Player player, CollisionData data) {
            if(player.StateMachine.State == 2) {
                var ext = player.GetExtensionFields();
                
                if(!ext.DashStickyRetentionExists) {
                    ext.DashStickyRetentionExists = true;
                    ext.DashStickyRetentionDirection = data.Direction;
                    ext.DashStickyRetentionSpeed = player.Speed;
                }
            }
        }

        private static void clearDashStickyRetention(Player player)
            => player.GetExtensionFields().DashStickyRetentionExists = false;
    }
}