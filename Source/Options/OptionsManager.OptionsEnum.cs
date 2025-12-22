//this file is literally just for the enum
//lmao

namespace Celeste.Mod.GooberHelper.Options {
    public static partial class OptionsManager {
        public enum Option {
            //jumping
            JumpInversion,
            WalljumpSpeedPreservation,
            WallbounceSpeedPreservation,
            HyperAndSuperSpeedPreservation,
            UpwardsJumpSpeedPreservationThreshold,
            DownwardsJumpSpeedPreservationThreshold,
            BounceHelperBounceSpeedPreservation,

            GetClimbjumpSpeedInRetention,
            AdditiveVerticalJumpSpeed,
            SwapHorizontalAndVerticalSpeedOnWalljump,
            VerticalToHorizontalSpeedOnGroundJump,
            CornerboostBlocksEverywhere,

            AllDirectionHypersAndSupers,
            AllowUpwardsCoyote,
            AllDirectionDreamJumps,
            AllowHoldableClimbjumping,

            //dashing
            VerticalDashSpeedPreservation,
            ReverseDashSpeedPreservation,

            MagnitudeBasedDashSpeed,

            DashesDontResetSpeed,
            KeepDashAttackOnCollision,
            DownDemoDashing,

            //moving
            CobwobSpeedInversion,
            HorizontalTurningSpeedInversion,
            VerticalTurningSpeedInversion,
            UpwardsTransitionSpeedPreservation,

            WallboostDirectionIsOppositeSpeed,
            WallboostSpeedIsOppositeSpeed,
            DownwardsAirFrictionBehavior,
            IgnoreForcemove,

            //entities
            RefillFreezeLength,

            DreamBlockSpeedPreservation,
            SpringSpeedPreservation,
            ReboundSpeedPreservation,
            PointBounceSpeedPreservation,
            ReflectBounceSpeedPreservation,
            ExplodeLaunchSpeedPreservation,
            PickupSpeedInversion,
            BubbleSpeedPreservation,
            FeatherEndSpeedPreservation,
            BadelineBossSpeedPreservation,

            CustomFeathers,
            CustomSwimming,
            LenientStunning,
            HoldableSpeedInheritanceHorizontal,
            HoldableSpeedInheritanceVertical,
            ReverseBackboosts,
            
            AllowCrouchedHoldableGrabbing,
            CoreBlockAllDirectionActivation,

            //other
            RetentionLength,

            ConserveBeforeDashSpeed,
            ClimbingSpeedPreservation,

            FastFallHitboxSquish,
            LiftboostAdditionHorizontal,
            LiftboostAdditionVertical,
            AdvantageousLiftboost,

            AllowUpwardsClimbGrabbing,
            AllowCrouchedClimbGrabbing,
            AllowClimbingInDashState,
            AllowWindWhileDashing,
            RemoveNormalEnd,

            //visuals
            PlayerShaderMask,
            TheoNuclearReactor,
            RotatePlayerToSpeed,

            //miscellaneous
            AlwaysExplodeSpinners,
            GoldenBlocksAlwaysLoad,
            RefillFreezeGameSuspension,
            BufferDelayVisualization,
            Ant,

            //general
            ShowActiveOptions,
        }
    }
}