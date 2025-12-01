using Celeste.Mod.GooberHelper.Attributes.Hooks;
using MonoMod.Utils;

namespace Celeste.Mod.GooberHelper.Extensions {
    public static class PlayerExtensions {
        public class PlayerExtensionFields {
            public Vector2 LenientAllDirectionRetentionSpeed = Vector2.Zero;
            public float LenientAllDirectionRetentionTimer = 0f;
            public Platform LenientAllDirectionRetentionPlatform;

            //i love naming variables
            public bool IsDolphin = false;

            public bool DashStickyRetentionExists = false;
            public Vector2 DashStickyRetentionSpeed = Vector2.Zero;
            
            public Vector2 BeforeDashSpeedConserved = Vector2.Zero;
            
            public Vector2 BoostSpeedPreserved = Vector2.Zero;
            public Vector2 StarFlySpeedPreserved = Vector2.Zero;
            public Vector2 AttractSpeedPreserved = Vector2.Zero;
            public Vector2 WallJumpSpeedPreserved = Vector2.Zero;
            
            public Vector2 PreservedDreamBlockSpeedMagnitude = Vector2.Zero;
            public Vector2 DashWindBoost = Vector2.Zero;
            
            public float HitboxCompression = 0f;

            public float PlayerRotationTarget = 0f;
            public float PlayerRotation = 0f;
        }

        private static readonly string f_Player_GooberHelperExtensionFields = nameof(f_Player_GooberHelperExtensionFields);

        public static PlayerExtensionFields GetExtensionFields(this Player player)
            => DynamicData.For(player).Get<PlayerExtensionFields>(f_Player_GooberHelperExtensionFields);

        [OnHook]
        private static void patch_Player_ctor(On.Celeste.Player.orig_ctor orig, Player self, Vector2 position, PlayerSpriteMode spriteMode) {
            orig(self, position, spriteMode);

            DynamicData.For(self).Set(f_Player_GooberHelperExtensionFields, new PlayerExtensionFields());
        }

        public static Vector2 GetConservedSpeed(this Player self) {
            var ext = self.GetExtensionFields();
            var conserveBeforeDashSpeed = self.StateMachine.State == Player.StDash && GetOptionBool(Option.ConserveBeforeDashSpeed);

            return new Vector2(
                Utils.SignedAbsMax(
                    self.Speed.X,
                    self.wallSpeedRetentionTimer > 0f
                        ? self.wallSpeedRetained
                        : 0f,
                    conserveBeforeDashSpeed
                        ? self.beforeDashSpeed.X
                        : 0f,
                    conserveBeforeDashSpeed && ext.DashStickyRetentionExists
                        ? ext.DashStickyRetentionSpeed.X
                        : 0f
                ),
                Utils.SignedAbsMax(
                    self.Speed.Y,
                    conserveBeforeDashSpeed
                        ? self.beforeDashSpeed.Y
                        : 0f,
                    conserveBeforeDashSpeed && ext.DashStickyRetentionExists
                        ? ext.DashStickyRetentionSpeed.Y
                        : 0f
                )
            );
        }
    }
}