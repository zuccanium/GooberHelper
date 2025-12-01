using System;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Extensions;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.GeneralHooks {
    public static class LenientAllDirectionRetention {
        [ILHook]
        private static void patch_Player_Update(ILContext il) {
            var cursor = new ILCursor(il);

            cursor.EmitLdarg0();
            cursor.EmitDelegate(updateRetention);
        }

        [ILHook(typeof(Player), "OnCollideH")]
        [ILHook(typeof(Player), "OnCollideV")]
        private static void patchCollisions(ILContext il) {
            var cursor = new ILCursor(il);

            cursor.EmitLdarg0();
            cursor.EmitLdarg1();
            cursor.EmitDelegate(handleCollision);
        }

        private static void updateRetention(Player player) {
            var ext = player.GetExtensionFields();

            if(ext.LenientAllDirectionRetentionTimer <= 0) {
                ext.LenientAllDirectionRetentionSpeed = Vector2.Zero;

                return;
            }
            
            Utils.Log($"[awesome retention info] speed: {ext.LenientAllDirectionRetentionSpeed}, time: {ext.LenientAllDirectionRetentionTimer}");

            ext.LenientAllDirectionRetentionTimer -= Engine.DeltaTime;
        }

        private static void handleCollision(Player player, CollisionData data) {
            var ext = player.GetExtensionFields();

            var newCollision = false;

            if(data.Direction.Y == 0 && Math.Abs(player.Speed.X) > Math.Abs(ext.LenientAllDirectionRetentionSpeed.X)) {
                ext.LenientAllDirectionRetentionSpeed.X = player.Speed.X;
                
                newCollision = true;
            }

            if(data.Direction.X == 0 && Math.Abs(player.Speed.Y) > Math.Abs(ext.LenientAllDirectionRetentionSpeed.Y)) {
                ext.LenientAllDirectionRetentionSpeed.Y = player.Speed.Y;
                
                newCollision = true;
            }

            if(newCollision) {
                ext.LenientAllDirectionRetentionTimer = 0.06f;
                ext.LenientAllDirectionRetentionPlatform = data.Hit;
            }
        }
    }
}