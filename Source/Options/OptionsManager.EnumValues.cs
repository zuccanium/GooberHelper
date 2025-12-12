namespace Celeste.Mod.GooberHelper.Options {
    public static partial class OptionsManager {
        //jank as hell
        //i need nobody to ever use this as an actual value
        public const int ReservedHybridEnumConstant = -899405; //thank you sparky
        public const int ReservedHybridEnumSize = 10; //minimize the chances of a collision

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
            None = ReservedHybridEnumConstant + 0,
            DashSpeed = ReservedHybridEnumConstant + 1,
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

        public enum HoldableSpeedInheritanceHybridValue {
            None = ReservedHybridEnumConstant + 0,
            MatchPlayer = ReservedHybridEnumConstant + 1
        }
    }
}