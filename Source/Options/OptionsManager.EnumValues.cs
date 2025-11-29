namespace Celeste.Mod.GooberHelper.Options {
    public static partial class OptionsManager {
        public enum JumpInversionValue {
            None,
            GroundJumps,
            All
        }

        public enum WalljumpSpeedPreservationValue {
            None,
            FakeRCB,
            Preserve,
            Invert,
        }

        public enum VerticalJumpSpeedPreservationHybridValue {
            None = -1,
            DashSpeed = -2,
        }

        public enum AllDirectionHypersAndSupersValue {
            None,
            RequireGround,
            WorkWithCoyoteTimeAndDontRefill,
            WorkWithCoyoteTime
        }

        public enum VerticalToHorizontalSpeedOnGroundJumpValue {
            None,
            Vertical,
            Magnitude
        }

        public enum MagnitudeBasedDashSpeedValue {
            None,
            OnlyCardinal,
            All
        }

        public enum CobwobSpeedInversionValue {
            None,
            RequireSpeed,
            WorkWithRetention
        }

        public enum DreamBlockSpeedPreservationValue {
            None,
            Horizontal,
            Vertical,
            Both,
            Magnitude,
        }

        public enum SpringSpeedPreservationValue {
            None,
            Preserve,
            Invert
        }
        
        public enum CustomFeathersValue {
            None,
            KeepIntro,
            SkipIntro
        }

        public enum PlayerShaderMaskValue {
            None,
            HairOnly,
            Cover,
        }

        public enum DashesDontResetSpeedValue {
            None,
            Legacy,
            On,
        }

        public enum AllowWindWhileDashingValue {
            None,
            Velocity,
            Speed,
        }

        public enum ExplodeLaunchSpeedPreservationValue {
            None,
            Horizontal,
            Vertical,
            Both,
            Magnitude,
        }

        public enum AllowUpwardsClimbGrabbingValue {
            None,
            WhileHoldingUp,
            Always
        }
    }
}