using System.Collections.Generic;

namespace Celeste.Mod.GooberHelper.Options {
    public static partial class OptionsManager {
        //the order within categories is
        //- speed preservation
        //- new thing
        //- allowing things that are prevented in vanilla
        //these subcategories are sorted roughly by creation order or however i want 😭
        //important things can be pinned to the top
        
        //important terminology definitions:
        //preservation = it preserves speed
        //inversion -> it preserves speed AND the player can decide which direction to go 

        public enum OptionCategory {
            Jumping,
            Dashing,
            Moving,
            Other,
            Visuals,
            Miscellaneous,
            General
        }

        public static readonly Dictionary<OptionCategory, List<OptionData>> Categories = new() {
            { OptionCategory.Jumping, [
                //goodbye buhbu 💗 i will love you forever
                // new OptionData(Option.buhbu, OptionType.Float, 0) { min = 0, max = 10, growthFactor = 10, suffix = " frames" },
                // new OptionData(Option.zonmgle),
                // new OptionData(Option.zingle)
                new OptionData(Option.JumpInversion, typeof(JumpInversionValue), JumpInversionValue.None),
                new OptionData(Option.WalljumpSpeedPreservation, typeof(WalljumpSpeedPreservationValue), WalljumpSpeedPreservationValue.None),
                new OptionData(Option.WallbounceSpeedPreservation),
                new OptionData(Option.HyperAndSuperSpeedPreservation),
                new OptionData(Option.UpwardsJumpSpeedPreservationThreshold, typeof(VerticalJumpSpeedPreservationHybridValue), OptionType.Float, (int)VerticalJumpSpeedPreservationHybridValue.None) { RightMin = 0, RightMax = 240, Step = 10, Suffix = "px/s" },
                new OptionData(Option.DownwardsJumpSpeedPreservationThreshold, typeof(VerticalJumpSpeedPreservationHybridValue), OptionType.Float, (int)VerticalJumpSpeedPreservationHybridValue.None) { RightMin = 0, RightMax = 240, Step = 10, Suffix = "px/s" },
                new OptionData(Option.BounceHelperBounceSpeedPreservation),

                new OptionData(Option.GetClimbjumpSpeedInRetention),
                new OptionData(Option.AdditiveVerticalJumpSpeed),
                new OptionData(Option.SwapHorizontalAndVerticalSpeedOnWalljump),
                new OptionData(Option.VerticalToHorizontalSpeedOnGroundJump, typeof(VerticalToHorizontalSpeedOnGroundJumpValue), VerticalToHorizontalSpeedOnGroundJumpValue.None),
                new OptionData(Option.CornerboostBlocksEverywhere),
                
                new OptionData(Option.AllDirectionHypersAndSupers, typeof(AllDirectionHypersAndSupersValue), AllDirectionHypersAndSupersValue.None),
                new OptionData(Option.AllowUpwardsCoyote),
                new OptionData(Option.AllDirectionDreamJumps),
                new OptionData(Option.AllowHoldableClimbjumping),
            ]},
            { OptionCategory.Dashing, [
                new OptionData(Option.VerticalDashSpeedPreservation),
                new OptionData(Option.ReverseDashSpeedPreservation),

                new OptionData(Option.MagnitudeBasedDashSpeed, typeof(MagnitudeBasedDashSpeedValue), MagnitudeBasedDashSpeedValue.None),
                
                new OptionData(Option.DashesDontResetSpeed, typeof(DashesDontResetSpeedValue), DashesDontResetSpeedValue.None),
                new OptionData(Option.KeepDashAttackOnCollision),
                new OptionData(Option.DownDemoDashing),
            ]},
            { OptionCategory.Moving, [
                new OptionData(Option.CobwobSpeedInversion, typeof(CobwobSpeedInversionValue), CobwobSpeedInversionValue.None),
                
                new OptionData(Option.WallboostDirectionIsOppositeSpeed),
                new OptionData(Option.WallboostSpeedIsOppositeSpeed),
                new OptionData(Option.HorizontalTurningSpeedInversion),
                new OptionData(Option.VerticalTurningSpeedInversion),
                new OptionData(Option.DownwardsAirFrictionBehavior),
                new OptionData(Option.IgnoreForcemove),

                new OptionData(Option.UpwardsTransitionSpeedPreservation),
            ]},
            { OptionCategory.Other, [
                new OptionData(Option.RefillFreezeLength, OptionType.Float, 3) { RightMin = 0, Step = 1, Suffix = "f" },
                new OptionData(Option.RetentionLength, OptionType.Float, 4) { RightMin = 0, Step = 1, Suffix = "f" },
                
                new OptionData(Option.ConserveBeforeDashSpeed),
                new OptionData(Option.DreamBlockSpeedPreservation, typeof(DreamBlockSpeedPreservationValue), DreamBlockSpeedPreservationValue.None),
                new OptionData(Option.SpringSpeedPreservation, typeof(SpringSpeedPreservationValue), SpringSpeedPreservationValue.None),
                new OptionData(Option.ReboundSpeedPreservation),
                new OptionData(Option.PointBounceSpeedPreservation),
                new OptionData(Option.ReflectBounceSpeedPreservation),
                new OptionData(Option.ExplodeLaunchSpeedPreservation, typeof(ExplodeLaunchSpeedPreservationValue), ExplodeLaunchSpeedPreservationValue.None),
                new OptionData(Option.PickupSpeedInversion),
                new OptionData(Option.BubbleSpeedPreservation),
                new OptionData(Option.FeatherEndSpeedPreservation),
                new OptionData(Option.BadelineBossSpeedPreservation),

                new OptionData(Option.CustomFeathers, typeof(CustomFeathersValue), CustomFeathersValue.None),
                new OptionData(Option.CustomSwimming),
                new OptionData(Option.RemoveNormalEnd),
                new OptionData(Option.LenientStunning),
                new OptionData(Option.HoldableSpeedInheritanceHorizontal, typeof(HoldableSpeedInheritanceHybridValue), OptionType.Float, (int)HoldableSpeedInheritanceHybridValue.None) { LeftMax = 0, RightMin = 0, Step = 5, SkipLeftMax = true, SkipRightMin = true, Suffix = "%" },
                new OptionData(Option.HoldableSpeedInheritanceVertical, typeof(HoldableSpeedInheritanceHybridValue), OptionType.Float, (int)HoldableSpeedInheritanceHybridValue.None) { LeftMax = 0, RightMin = 0, Step = 5, SkipLeftMax = true, SkipRightMin = true, Suffix = "%" },
                new OptionData(Option.FastFallHitboxSquish, OptionType.Float, 0) { RightMin = 0, RightMax = 100, Step = 5, Suffix = "%" },

                new OptionData(Option.AllowCrouchedHoldableGrabbing),
                new OptionData(Option.AllowUpwardsClimbGrabbing, typeof(AllowUpwardsClimbGrabbingValue), AllowUpwardsClimbGrabbingValue.None),
                new OptionData(Option.AllowCrouchedClimbGrabbing),
                new OptionData(Option.ClimbingSpeedPreservation),
                new OptionData(Option.AllowClimbingInDashState),
                new OptionData(Option.CoreBlockAllDirectionActivation),
                new OptionData(Option.AllowWindWhileDashing, typeof(AllowWindWhileDashingValue), AllowWindWhileDashingValue.None),
                new OptionData(Option.LiftboostAdditionHorizontal, OptionType.Float, 0) { LeftMax = 0, RightMin = 0, Step = 5, Suffix = "px/s" },
                new OptionData(Option.LiftboostAdditionVertical, OptionType.Float, 0) { LeftMax = 0, RightMin = 0, Step = 5, Suffix = "px/s" },
                new OptionData(Option.AdvantageousLiftBoost),
                new OptionData(Option.ReverseBackboosts),
            ]},
            { OptionCategory.Visuals, [
                new OptionData(Option.PlayerShaderMask, typeof(PlayerShaderMaskValue), PlayerShaderMaskValue.None),
                new OptionData(Option.TheoNuclearReactor),
                new OptionData(Option.RotatePlayerToSpeed),
            ]},
            { OptionCategory.Miscellaneous, [
                new OptionData(Option.AlwaysExplodeSpinners),
                new OptionData(Option.GoldenBlocksAlwaysLoad),
                new OptionData(Option.RefillFreezeGameSuspension),
                new OptionData(Option.BufferDelayVisualization),
                new OptionData(Option.Ant),
            ]},
            { OptionCategory.General, [
                new OptionData(Option.ShowActiveOptions),
            ]},
        };
    }
}