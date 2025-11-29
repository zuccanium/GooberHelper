using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Extensions;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.GeneralHooks {
    public static class AwesomeRetention {
        public static bool UseAwesomeRetention
            => GetOptionBool(Option.CustomSwimming)
            || GetOptionBool(Option.VerticalToHorizontalSpeedOnGroundJump);

        [ILHook]
        private static void patch_Player_Update(ILContext il) {
            var cursor = new ILCursor(il);

            cursor.EmitLdarg0();
            cursor.EmitDelegate(updateAwesomeRetention);
        }

        [ILHook]
        private static void patch_Player_OnCollideH(ILContext il) {
            var cursor = new ILCursor(il);

            cursor.EmitLdarg0();
            cursor.EmitLdarg1();
            cursor.EmitDelegate(handleAwesomeRetentionHorizontal);
        }

        [ILHook]
        private static void patch_Player_OnCollideV(ILContext il) {
            var cursor = new ILCursor(il);

            cursor.EmitLdarg0();
            cursor.EmitLdarg1();
            cursor.EmitDelegate(handleAwesomeRetentionVertical);
        }

        private static void updateAwesomeRetention(Player player) {
            if(UseAwesomeRetention) {
                var ext = player.GetExtensionFields();

                if(ext.AwesomeRetentionTimer > 0) {
                    Utils.Log($"[awesome retention info] speed: {ext.AwesomeRetentionSpeed}, dir: {ext.AwesomeRetentionDirection}, time: {ext.AwesomeRetentionTimer}");

                    ext.AwesomeRetentionTimer -= Engine.DeltaTime;
                } else {
                    ext.AwesomeRetentionDirection = Vector2.Zero;
                }
            }
        }

        private static void handleAwesomeRetentionHorizontal(Player player, CollisionData data) {
            if(UseAwesomeRetention) {
                var ext = player.GetExtensionFields();

                if(ext.AwesomeRetentionTimer <= 0.0f) {
                    ext.AwesomeRetentionSpeed.X = player.Speed.X;
                    ext.AwesomeRetentionTimer = 0.06f;
                    ext.AwesomeRetentionWasInWater = player.CollideCheck<Water>();
                    ext.AwesomeRetentionPlatform = data.Hit;

                    ext.AwesomeRetentionDirection = new Vector2(data.Direction.X, ext.AwesomeRetentionDirection.Y);
                }
            }
        }

        private static void handleAwesomeRetentionVertical(Player player, CollisionData data) {
            if(UseAwesomeRetention) {
                var ext = player.GetExtensionFields();

                if(ext.AwesomeRetentionTimer <= 0.0f) {
                    ext.AwesomeRetentionSpeed.Y = player.Speed.Y;
                    ext.AwesomeRetentionTimer = 0.06f;
                    ext.AwesomeRetentionWasInWater = player.CollideCheck<Water>();
                    ext.AwesomeRetentionPlatform = data.Hit;

                    ext.AwesomeRetentionDirection = new Vector2(ext.AwesomeRetentionDirection.X, data.Direction.Y);
                }
            }
        }
    }
}