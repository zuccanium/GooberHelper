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

            WallboostDirectionIsOppositeSpeed,
            WallboostSpeedIsOppositeSpeed,
            HorizontalTurningSpeedInversion,
            VerticalTurningSpeedInversion,
            DownwardsAirFrictionBehavior,

            UpwardsTransitionSpeedPreservation,

            //other
            RefillFreezeLength,
            RetentionLength,

            ConserveBeforeDashSpeed,
            DreamBlockSpeedPreservation,
            SpringSpeedPreservation,
            ReboundSpeedPreservation,
            ExplodeLaunchSpeedPreservation,
            PickupSpeedInversion,
            BubbleSpeedPreservation,
            FeatherEndSpeedPreservation,
            BadelineBossSpeedPreservation,

            CustomFeathers,
            CustomSwimming,
            RemoveNormalEnd,
            LenientStunning,
            HoldablesInheritSpeedWhenThrown,
            FastFallHitboxSquish,

            AllowCrouchedHoldableGrabbing,
            AllowUpwardsClimbGrabbing,
            AllowCrouchedClimbGrabbing,
            ClimbingSpeedPreservation,
            AllowClimbingInDashState,
            CoreBlockAllDirectionActivation,
            AllowWindWhileDashing,
            LiftboostAdditionHorizontal,
            LiftboostAdditionVertical,
            AdvantageousLiftBoost,
            ReverseBackboosts,

            //visuals
            PlayerShaderMask,
            TheoNuclearReactor,

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