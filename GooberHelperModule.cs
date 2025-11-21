using System;
using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using Monocle;
using Microsoft.Xna.Framework;
using System.Text.RegularExpressions;
using Celeste.Mod.GooberHelper.Entities;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Celeste.Mod.Helpers;
using Celeste.Mod.GooberHelper.Components;
using System.Collections.Generic;
using Celeste.Mod.GooberHelper.UI;
using static Celeste.Mod.GooberHelper.OptionsManager;
using Celeste.Mod.Entities;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Helpers;

namespace Celeste.Mod.GooberHelper {
    public class GooberHelperModule : EverestModule {
        public static GooberHelperModule Instance { get; private set; }


        public override Type SettingsType => typeof(GooberHelperModuleSettings);
        public static GooberHelperModuleSettings Settings => (GooberHelperModuleSettings) Instance._Settings;

        public override Type SessionType => typeof(GooberHelperModuleSession);
        public static GooberHelperModuleSession Session => (GooberHelperModuleSession) Instance._Session;

        private static ILHook playerWindMoveHook;
        private static ILHook playerUpdateHook;
        private static ILHook playerStarFlyCoroutineHook;
        private static ILHook playerDashCoroutineHook;
        private static ILHook playerPickupCoroutineHook;
        private static ILHook playerRedDashCoroutineHook;
        private static ILHook playerDashCoroutineHook2;
        private static ILHook postcardEaseInHook;
        private static ILHook postcardEaseButtinInHook;
        private static ILHook postcardEaseOutHook;
        private static ILHook postcardDisplayRoutineHook;
        private static ILHook playerGetLiftBoostHook;
        private static ILHook silverBlockAwakeHook;
        private static ILHook platinumBlockAwakeHook;
        private static ILHook bounceHelperBounceHook;
        private static ILHook playerWallJumpHook;
        private static ILHook playerPickupHook;

        private static Color lastPlayerHairColor = Player.NormalHairColor;

        private static Regex refillRoutineRegex = new Regex("RefillRoutine");

        private static Effect playerMaskEffect = null;
        private static bool startedRendering = false;
        private static Vector2 beforeUpdateSpeed;

        private static bool runningNormalUpdateJustForClimbing = false;
        private static bool UseAwesomeRetention {
            get => GetOptionBool(Option.CustomSwimming);
        }

        public GooberHelperModule() {
            Instance = this;
#if DEBUG
            // debug builds use verbose logging
            Logger.SetLogLevel(nameof(GooberHelperModule), LogLevel.Verbose);
#else
            // release builds use info logging to reduce spam in log files
            Logger.SetLogLevel(nameof(GooberHelperModule), LogLevel.Info);
#endif
        }

        public override void Load() {
            OnLoadAttribute.Load();
            
            ModIntegration.FrostHelperAPI.Load();
            ModIntegration.ExtendedVariantModeAPI.Load();

            FluidSimulation.Load();
            AbstractTrigger<GooberPhysicsOptions>.Load();
            AbstractTrigger<GooberMiscellaneousOptions>.Load();
            AbstractTrigger<RefillFreezeLength>.Load();
            AbstractTrigger<RetentionFrames>.Load();

            GooberHelperOptions.Load();

            BufferOffsetIndicator.Load();

            DebugMapThings.Load();

            Everest.Events.Level.OnCreatePauseMenuButtons += createPauseMenuButton;
            Everest.Events.Input.OnInitialize += UpdateFastMenuing;

            playerUpdateHook = new ILHook(typeof(Player).GetMethod("orig_Update"), modifyPlayerUpdate);
            playerStarFlyCoroutineHook = new ILHook(typeof(Player).GetMethod("StarFlyCoroutine", BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(), modifyPlayerStarFlyCoroutine);
            playerDashCoroutineHook = new ILHook(typeof(Player).GetMethod("DashCoroutine", BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(), modifyPlayerDashCoroutine);
            playerPickupCoroutineHook = new ILHook(typeof(Player).GetMethod("PickupCoroutine", BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(), modifyPlayerPickupCoroutine);
            playerRedDashCoroutineHook = new ILHook(typeof(Player).GetMethod("RedDashCoroutine", BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(), modifyDashSpeedThing);
            playerDashCoroutineHook2 = new ILHook(typeof(Player).GetMethod("DashCoroutine", BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(), modifyDashSpeedThing);
            postcardEaseInHook = new ILHook(typeof(Postcard).GetMethod("EaseIn").GetStateMachineTarget(), modifyPostCardEaseIn);
            postcardEaseButtinInHook = new ILHook(typeof(Postcard).GetMethod("EaseButtinIn", BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(), modifyPostCardEaseButtinIn);
            postcardEaseOutHook = new ILHook(typeof(Postcard).GetMethod("EaseOut").GetStateMachineTarget(), modifyPostCardEaseOut);
            postcardDisplayRoutineHook = new ILHook(typeof(Postcard).GetMethod("DisplayRoutine").GetStateMachineTarget(), modifyPostCardDisplayRoutine);
            playerGetLiftBoostHook = new ILHook(typeof(Player).GetMethod("get_LiftBoost", BindingFlags.NonPublic | BindingFlags.Instance), modifyPlayerGetLiftBoost);
            playerWindMoveHook = new ILHook(typeof(Player).GetMethod("orig_WindMove", BindingFlags.NonPublic | BindingFlags.Instance), modifyPlayerWindMove);
            playerWallJumpHook = new ILHook(typeof(Player).GetMethod("orig_WallJump", BindingFlags.NonPublic | BindingFlags.Instance), modifyPlayerWallJump);
            playerPickupHook = new ILHook(typeof(Player).GetMethod("orig_Pickup", BindingFlags.NonPublic | BindingFlags.Instance), modifyPlayerPickup);

            if(Everest.Loader.DependencyLoaded(new EverestModuleMetadata() { Name = "CollabUtils2", Version = new Version(1, 10, 0) })) {
                //feel free to opp PLEASE i need to learn a better way of doing this
                var type = Type.GetType("Celeste.Mod.CollabUtils2.Entities.SilverBlock, CollabUtils2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");

                silverBlockAwakeHook = new ILHook(type.GetMethod("Awake"), makeGoldenBlocksOrSimilarEntitiesAlwaysLoad);
            }

            if(Everest.Loader.DependencyLoaded(new EverestModuleMetadata() { Name = "PlatinumStrawberry", Version = new Version(1, 0, 0) })) {
                var type = Type.GetType("Celeste.Mod.PlatinumStrawberry.Entities.PlatinumBlock, PlatinumStrawberry, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");

                platinumBlockAwakeHook = new ILHook(type.GetMethod("Awake"), makeGoldenBlocksOrSimilarEntitiesAlwaysLoad);
            }

            if(Everest.Loader.DependencyLoaded(new EverestModuleMetadata() { Name = "BounceHelper", Version = new Version(1, 0, 0) })) {
                var type = Type.GetType("Celeste.Mod.BounceHelper.BounceHelperModule, BounceHelper, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");

                bounceHelperBounceHook = new ILHook(type.GetMethod("bounce", BindingFlags.NonPublic | BindingFlags.Instance), modifyBounceHelperBounce);
            }

            IL.Celeste.GoldenBlock.Awake += makeGoldenBlocksOrSimilarEntitiesAlwaysLoad;

            IL.Celeste.Player.OnCollideH += modifyPlayerOnCollideH;
            IL.Celeste.Player.OnCollideV += modifyPlayerOnCollideV;
            IL.Celeste.Player.StarFlyUpdate += modifyPlayerStarFlyUpdate;
            IL.Celeste.Player.DashUpdate += modifyPlayerDashUpdate;
            IL.Celeste.Player.NormalUpdate += modifyPlayerNormalUpdate;
            IL.Celeste.Player.RedDashUpdate += modifyPlayerRedDashUpdate;
            IL.Celeste.Player.HitSquashUpdate += modifyPlayerHitSquashUpdate;
            IL.Celeste.Player.DreamDashEnd += modifyPlayerDreamDashEnd;
            IL.Celeste.Player.DreamDashUpdate += modifyPlayerDreamDashUpdate;
            IL.Celeste.Player.LaunchUpdate += modifyPlayerLaunchUpdate;
            IL.Celeste.Player.WallJumpCheck += modifyPlayerWallJumpCheck;
            IL.Celeste.Player.SwimUpdate += modifyPlayerSwimUpdate;
            IL.Celeste.Player.ClimbBegin += modifyPlayerClimbBegin;
            IL.Celeste.Player.ClimbUpdate += modifyPlayerClimbUpdate;
            IL.Celeste.Player.SuperWallJump += modifyPlayerSuperWallJump;
            IL.Celeste.Player.SuperJump += modifyPlayerSuperJump;
            IL.Celeste.Player.ExplodeLaunch_Vector2_bool_bool += modifyPlayerExplodeLaunch;
            IL.Celeste.BounceBlock.WindUpPlayerCheck += modifyBounceBlockWindUpPlayerCheck;
            
            On.Celeste.Player.Update += modPlayerUpdate;
            On.Celeste.Player.Jump += modPlayerJump;
            On.Celeste.Player.Rebound += modPlayerRebound;
            On.Celeste.Player.ReflectBounce += modPlayerReflectBounce;
            On.Celeste.Player.PointBounce += modPlayerPointBounce;
            On.Celeste.Player.OnCollideH += modPlayerOnCollideH;
            On.Celeste.Player.OnCollideV += modPlayerOnCollideV;
            On.Celeste.Player.SideBounce += modPlayerSideBounce;
            On.Celeste.Player.SuperBounce += modPlayerSuperBounce;
            On.Celeste.Player.WallJump += modPlayerWallJump;
            On.Celeste.Player.ClimbJump += modPlayerClimbJump;
            On.Celeste.Player.StarFlyBegin += modPlayerStarFlyBegin;
            On.Celeste.Player.FinalBossPushLaunch += modPlayerFinalBossPushLaunch;
            On.Celeste.Player.AttractBegin += modPlayerAttractBegin;
            On.Celeste.Player.SwimBegin += modPlayerSwimBegin;
            On.Celeste.Player.WallJumpCheck += modPlayerWallJumpCheck;
            On.Celeste.Player.DashBegin += modPlayerDashBegin;
            On.Celeste.Player.NormalEnd += modPlayerNormalEnd;
            On.Celeste.Player.SuperJump += modPlayerSuperJump;
            On.Celeste.Player.ctor += modPlayerCtor;
            On.Celeste.Player.BeforeUpTransition += modPlayerBeforeUpTransition;
            On.Celeste.Player.Boost += modPlayerBoost;
            On.Celeste.Player.RedBoost += modPlayerRedBoost;
            On.Celeste.Player.Render += modPlayerRender;
            On.Celeste.Player.SwimCheck += modPlayerSwimCheck;
            On.Celeste.Player.SwimJumpCheck += modPlayerSwimJumpCheck;
            On.Celeste.Player.UnderwaterMusicCheck += modPlayerUnderwaterMusicCheck;
            On.Celeste.Player.NormalBegin += modPlayerNormalBegin;
            On.Celeste.Player.DashEnd += modPlayerDashEnd;

            On.Celeste.CrystalStaticSpinner.OnPlayer += modCrystalStaticSpinnerOnPlayer;

            On.Celeste.Celeste.Freeze += modCelesteFreeze;

            On.Celeste.Level.LoadLevel += modLevelLevelLoad;
            On.Celeste.Level.Pause += modLevelPause;
            On.Celeste.Level.Update += modLevelUpdate;
            On.Monocle.Scene.BeforeUpdate += modSceneBeforeUpdate;

            On.Celeste.PlayerDeadBody.Render += modPlayerDeadBodyRender;

            On.Celeste.Holdable.Release += modHoldableRelease;

            On.Celeste.TheoCrystal.ctor_Vector2 += modTheoCrystalCtor;

            // //code adapted from https://github.com/0x0ade/CelesteNet/blob/405a7e5e4d78727cd35ee679a730400b0a46667a/CelesteNet.Client/Components/CelesteNetMainComponent.cs#L71-L75 (thank you snip for posting this link 8 months ago)
            // using (new DetourConfigContext(new DetourConfig(
            //     "GooberHelper",
            //     int.MinValue  // this simulates before: "*"
            // )).Use()) {
            //     On.Celeste.Player.SwimUpdate += modPlayerSwimUpdate;
            // }


            using (new DetourConfigContext(new DetourConfig(
                "GooberHelper",
                int.MaxValue
            )).Use()) {
                On.Celeste.PlayerHair.Render += modPlayerHairRender;
                On.Celeste.Player.DreamDashBegin += modPlayerDreamDashBegin;
            }

            using (new DetourConfigContext(new DetourConfig(
                "GooberHelper",
                int.MinValue
            )).Use()) {
                On.Celeste.Player.DreamDashUpdate += modPlayerDreamDashUpdate;
            }
        }

        public override void Unload() {
            OnUnloadAttribute.Unload();

            FluidSimulation.Unload();
            AbstractTrigger<GooberPhysicsOptions>.Unload();
            AbstractTrigger<GooberMiscellaneousOptions>.Unload();
            AbstractTrigger<RefillFreezeLength>.Unload();
            AbstractTrigger<RetentionFrames>.Unload();

            GooberHelperOptions.Unload();

            BufferOffsetIndicator.Unload();
            
            DebugMapThings.Unload();

            Everest.Events.Level.OnCreatePauseMenuButtons -= createPauseMenuButton;
            Everest.Events.Input.OnInitialize -= UpdateFastMenuing;

            playerUpdateHook.Dispose();
            playerStarFlyCoroutineHook.Dispose();
            playerDashCoroutineHook.Dispose();
            playerPickupCoroutineHook.Dispose();
            playerRedDashCoroutineHook.Dispose();
            playerDashCoroutineHook2.Dispose();
            postcardEaseInHook.Dispose();
            postcardEaseButtinInHook.Dispose();
            postcardEaseOutHook.Dispose();
            postcardDisplayRoutineHook.Dispose();
            playerGetLiftBoostHook.Dispose();
            playerWindMoveHook.Dispose();
            playerWallJumpHook.Dispose();
            playerPickupHook.Dispose();

            IL.Celeste.Player.OnCollideH -= modifyPlayerOnCollideH;
            IL.Celeste.Player.OnCollideV -= modifyPlayerOnCollideV;
            IL.Celeste.Player.StarFlyUpdate -= modifyPlayerStarFlyUpdate;
            IL.Celeste.Player.DashUpdate -= modifyPlayerDashUpdate;
            IL.Celeste.Player.NormalUpdate -= modifyPlayerNormalUpdate;
            IL.Celeste.Player.RedDashUpdate -= modifyPlayerRedDashUpdate;
            IL.Celeste.Player.HitSquashUpdate -= modifyPlayerHitSquashUpdate;
            IL.Celeste.Player.DreamDashEnd -= modifyPlayerDreamDashEnd;
            IL.Celeste.Player.DreamDashUpdate -= modifyPlayerDreamDashUpdate;
            IL.Celeste.Player.LaunchUpdate -= modifyPlayerLaunchUpdate;
            IL.Celeste.Player.WallJumpCheck -= modifyPlayerWallJumpCheck;
            IL.Celeste.Player.SwimUpdate -= modifyPlayerSwimUpdate;
            IL.Celeste.Player.ClimbBegin -= modifyPlayerClimbBegin;
            IL.Celeste.Player.ClimbUpdate -= modifyPlayerClimbUpdate;
            IL.Celeste.Player.SuperWallJump -= modifyPlayerSuperWallJump;
            IL.Celeste.Player.SuperJump -= modifyPlayerSuperJump;
            IL.Celeste.Player.ExplodeLaunch_Vector2_bool_bool -= modifyPlayerExplodeLaunch;
            IL.Celeste.BounceBlock.WindUpPlayerCheck -= modifyBounceBlockWindUpPlayerCheck;

            IL.Celeste.GoldenBlock.Awake -= makeGoldenBlocksOrSimilarEntitiesAlwaysLoad;

            silverBlockAwakeHook?.Dispose();
            platinumBlockAwakeHook?.Dispose();
            bounceHelperBounceHook?.Dispose();

            On.Celeste.Player.Update -= modPlayerUpdate;
            On.Celeste.Player.Jump -= modPlayerJump;
            On.Celeste.Player.Rebound -= modPlayerRebound;
            On.Celeste.Player.ReflectBounce -= modPlayerReflectBounce;
            On.Celeste.Player.PointBounce -= modPlayerPointBounce;
            On.Celeste.Player.OnCollideH -= modPlayerOnCollideH;
            On.Celeste.Player.OnCollideV -= modPlayerOnCollideV;
            On.Celeste.Player.SideBounce -= modPlayerSideBounce;
            On.Celeste.Player.SuperBounce -= modPlayerSuperBounce;
            On.Celeste.Player.WallJump -= modPlayerWallJump;
            On.Celeste.Player.ClimbJump -= modPlayerClimbJump;
            On.Celeste.Player.StarFlyBegin -= modPlayerStarFlyBegin;
            On.Celeste.Player.FinalBossPushLaunch -= modPlayerFinalBossPushLaunch;
            On.Celeste.Player.AttractBegin -= modPlayerAttractBegin;
            On.Celeste.Player.SwimBegin -= modPlayerSwimBegin;
            On.Celeste.Player.WallJumpCheck -= modPlayerWallJumpCheck;
            On.Celeste.Player.DashBegin -= modPlayerDashBegin;
            On.Celeste.Player.NormalEnd -= modPlayerNormalEnd;
            On.Celeste.Player.SuperJump -= modPlayerSuperJump;
            On.Celeste.Player.ctor -= modPlayerCtor;
            On.Celeste.Player.BeforeUpTransition -= modPlayerBeforeUpTransition;
            On.Celeste.Player.Boost -= modPlayerBoost;
            On.Celeste.Player.RedBoost -= modPlayerRedBoost;
            On.Celeste.Player.Render -= modPlayerRender;
            On.Celeste.Player.SwimCheck -= modPlayerSwimCheck;
            On.Celeste.Player.SwimJumpCheck -= modPlayerSwimJumpCheck;
            On.Celeste.Player.UnderwaterMusicCheck -= modPlayerUnderwaterMusicCheck;
            On.Celeste.Player.NormalBegin -= modPlayerNormalBegin;
            On.Celeste.Player.DashEnd -= modPlayerDashEnd;

            On.Celeste.CrystalStaticSpinner.OnPlayer -= modCrystalStaticSpinnerOnPlayer;

            On.Celeste.Celeste.Freeze -= modCelesteFreeze;

            On.Celeste.Level.LoadLevel -= modLevelLevelLoad;
            On.Celeste.Level.Pause -= modLevelPause;
            On.Celeste.Level.Update -= modLevelUpdate;
            On.Monocle.Scene.BeforeUpdate -= modSceneBeforeUpdate;

            On.Celeste.PlayerDeadBody.Render -= modPlayerDeadBodyRender;

            On.Celeste.Holdable.Release -= modHoldableRelease;
            
            On.Celeste.TheoCrystal.ctor_Vector2 -= modTheoCrystalCtor;


            // //code adapted from https://github.com/0x0ade/CelesteNet/blob/405a7e5e4d78727cd35ee679a730400b0a46667a/CelesteNet.Client/Components/CelesteNetMainComponent.cs#L71-L75 (thank you snip for posting this link 8 months ago)
            // using (new DetourConfigContext(new DetourConfig(
            //     "GooberHelper",
            //     int.MinValue  // this simulates before: "*"
            // )).Use()) {
            //     On.Celeste.Player.SwimUpdate -= modPlayerSwimUpdate;
            // }

            using (new DetourConfigContext(new DetourConfig(
                "GooberHelper",
                int.MaxValue
            )).Use()) {
                On.Celeste.PlayerHair.Render -= modPlayerHairRender;
                On.Celeste.Player.DreamDashBegin -= modPlayerDreamDashBegin;
            }

            using (new DetourConfigContext(new DetourConfig(
                "GooberHelper",
                int.MinValue
            )).Use()) {
                On.Celeste.Player.DreamDashUpdate -= modPlayerDreamDashUpdate;
            }
        }

        public static void UpdateFastMenuing() {
            if(Input.MenuLeft == null) return;

            var firstRepeatTime = Settings.FastMenuing ? 0.2f : 0.4f;
            var multiRepeatTime = Settings.FastMenuing ? 0.05f : 0.1f;

            Input.MenuLeft.SetRepeat(firstRepeatTime, multiRepeatTime);
            Input.MenuRight.SetRepeat(firstRepeatTime, multiRepeatTime);
            Input.MenuUp.SetRepeat(firstRepeatTime, multiRepeatTime);
            Input.MenuDown.SetRepeat(firstRepeatTime, multiRepeatTime);
        }

        //thank you everest!!! i stole a lot of your mod options code here; i hope you dont mind
        private void createPauseMenuButton(Level level, TextMenu menu, bool minimal) {
            if(!Settings.ShowOptionsInGame) return;

            int index = menu.items.FindIndex(item => item is TextMenu.Button && (item as TextMenu.Button).Label == Dialog.Clean("menu_pause_options"));
            menu.Insert(index, OuiGooberHelperOptions.CreateOptionsButton(menu, true));
        }

        private void modifyPostCardEaseIn(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(0.8f)))
                cursor.EmitDelegate((float orig) => Settings.FastMenuing ? 1.6f : orig);
        }

        //this isnt a typo on my part; the method is actually just called that 😭
        private void modifyPostCardEaseButtinIn(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(0.75f)))
                cursor.EmitDelegate((float orig) => Settings.FastMenuing ? 0f : orig);

            if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(2f)))
                cursor.EmitDelegate((float orig) => Settings.FastMenuing ? 4f : orig);
        }

        private void modifyPostCardEaseOut(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchCallOrCallvirt<Engine>("get_DeltaTime")))
                cursor.EmitDelegate((float orig) => Settings.FastMenuing ? orig * 2f : orig);
        }

        private void modifyPostCardDisplayRoutine(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(0.75f)))
                cursor.EmitDelegate((float orig) => Settings.FastMenuing ? 0f : orig);

            if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(1.2f)))
                cursor.EmitDelegate((float orig) => Settings.FastMenuing ? 0f : orig);
        }

        private void modifyPlayerGetLiftBoost(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNext(MoveType.AfterLabel, instr => instr.MatchRet())) {
                cursor.EmitLdarg0();
                cursor.EmitDelegate((Vector2 liftboost, Player player) => {
                    liftboost += new Vector2(GetOptionValue(Option.LiftBoostAdditionHorizontal), GetOptionValue(Option.LiftBoostAdditionVertical));

                    if(Math.Abs(player.Speed.X - liftboost.X) > Math.Abs(player.Speed.X + liftboost.X))
                        liftboost.X *= -1;

                    if(Math.Abs(player.Speed.Y - liftboost.Y) > Math.Abs(player.Speed.Y + liftboost.Y))
                        liftboost.Y *= -1;

                    return liftboost;
                });
            }
        }

        private void modifyPlayerClimbBegin(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(0f))) {
                cursor.EmitLdarg0();
                cursor.EmitDelegate((float value, Player player) => GetOptionBool(Option.ClimbingSpeedPreservation) ? player.Speed.X : value);
            }

            if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(0.2f)))
                cursor.EmitDelegate((float value) => GetOptionBool(Option.ClimbingSpeedPreservation) ? 1f : value);
            
            if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(0.1f)))
                cursor.EmitDelegate((float value) => GetOptionBool(Option.ClimbingSpeedPreservation) ? 0f : value);
        }

        private void modifyPlayerClimbUpdate(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(-45f))) {
                cursor.EmitLdarg0();
                cursor.EmitDelegate((float value, Player player) => {
                    return GetOptionBool(Option.ClimbingSpeedPreservation) ? Math.Min(player.Speed.Y, value) : value;
                });
            }

            if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(80f))) {
                cursor.EmitLdarg0();
                cursor.EmitDelegate((float value, Player player) => {
                    return GetOptionBool(Option.ClimbingSpeedPreservation) ? Math.Max(player.Speed.Y, value) : value;
                });
            }
        }

        private void handleVerticalSpeedToHorizontal(Player self, Vector2 originalSpeed) {
            float verticalToHorizontalSpeedOnGroundJumpValue = GetOptionValue(Option.VerticalToHorizontalSpeedOnGroundJump);
            
            if(verticalToHorizontalSpeedOnGroundJumpValue != (int)VerticalToHorizontalSpeedOnGroundJumpValue.None) {
                GooberPlayerExtensions c = GooberPlayerExtensions.Instance;

                float retainedVerticalSpeed = !c.AwesomeRetentionWasInWater && c.AwesomeRetentionTimer > 0 && c.AwesomeRetentionDirection.X == 0 ? Math.Abs(c.AwesomeRetentionSpeed.Y) : 0;

                float dir = Math.Sign(self.Speed.X);

                if(dir == 0) dir = self.moveX;
                if(dir == 0) dir = (int)self.Facing;

                float speedToConvert = Math.Max(Math.Abs(originalSpeed.Y), retainedVerticalSpeed);
                
                if(verticalToHorizontalSpeedOnGroundJumpValue == (int)VerticalToHorizontalSpeedOnGroundJumpValue.Magnitude) {
                    speedToConvert = new Vector2(speedToConvert, originalSpeed.X).Length();
                }

                self.Speed.X = dir * Math.Max(speedToConvert, Math.Abs(self.Speed.X));
            }
        }

        private void handleVerticalJumpSpeed(Player self, Vector2 originalSpeed) {
            float downwardsOptionValue = GetOptionValue(Option.DownwardsJumpSpeedPreservationThreshold);
            float upwardsOptionValue = GetOptionValue(Option.UpwardsJumpSpeedPreservationThreshold);

            bool doDownwardsStuff = Input.MoveY > 0 && (
                downwardsOptionValue == (int)VerticalJumpSpeedPreservationHybridValue.None ? false :
                downwardsOptionValue == (int)VerticalJumpSpeedPreservationHybridValue.DashSpeed ? self.StateMachine.state == Player.StDash :
                originalSpeed.Y >= downwardsOptionValue
            );

            bool doUpwardsStuff = (
                upwardsOptionValue == (int)VerticalJumpSpeedPreservationHybridValue.None ? false :
                upwardsOptionValue == (int)VerticalJumpSpeedPreservationHybridValue.DashSpeed ? self.StateMachine.state == Player.StDash :
                originalSpeed.Y <= -upwardsOptionValue
            );

            //probably add something to allow conversion between the two

            //gaslighting my own mod
            //did you know that gaslighting was invented by john gas?
            if(doDownwardsStuff) {
                self.Speed.Y *= -1;
                originalSpeed.Y *= -1;
            }

            if(GetOptionBool(Option.AdditiveVerticalJumpSpeed)) {
                self.Speed.Y = Math.Min(self.Speed.Y, self.Speed.Y + Math.Min(originalSpeed.Y, 0));

                self.varJumpSpeed = self.Speed.Y;
            } else {
                if(doDownwardsStuff || doUpwardsStuff) {
                    self.Speed.Y = Math.Min(originalSpeed.Y + self.LiftBoost.Y, self.Speed.Y);
                    self.varJumpSpeed = self.Speed.Y;
                }
            }

            //soliddarking someone elses mod
            if(doDownwardsStuff) {
                self.Speed.Y *= -1;
                originalSpeed.Y *= -1; //i know this is unnecessary but i want it to be symmetric 
                self.varJumpSpeed *= -1;
            }
        }

        private void modifyBounceHelperBounce(ILContext il) {
            var cursor = new ILCursor(il);

            cursor.EmitLdarg(1);
            cursor.EmitLdarg(2);
            cursor.EmitDelegate((Player player, Vector2 bounceSpeed) => {
                if(!GetOptionBool(Option.BounceSpeedPreservation))
                    return bounceSpeed;

                var sign = bounceSpeed.Sign();
                var c = GooberPlayerExtensions.Instance;

                bounceSpeed = bounceSpeed.Abs();

                //down and vertical bounces
                if(bounceSpeed.Y == 210 || bounceSpeed.Y == 200)
                    bounceSpeed.Y = Math.Max(
                        Math.Max(
                            Math.Abs(c.DashStickyRetentionExists ? c.DashStickyRetentionSpeed.Y : 0f),
                            Math.Abs(beforeUpdateSpeed.Y)
                        ),
                        bounceSpeed.Y
                    );

                //horizontal bounces
                else if(bounceSpeed.X == 320)
                    bounceSpeed.X = Math.Max(
                        Math.Max(
                            Math.Abs(c.DashStickyRetentionExists ? c.DashStickyRetentionSpeed.X : 0f),
                            Math.Abs(beforeUpdateSpeed.X)
                        ),
                        bounceSpeed.X
                    );
                
                //diagonal bounces
                else
                    bounceSpeed = bounceSpeed.SafeNormalize() * Math.Max(
                        new Vector2(
                            Math.Max(
                                Math.Abs(c.DashStickyRetentionExists ? c.DashStickyRetentionSpeed.X : 0f),
                                Math.Abs(beforeUpdateSpeed.X)
                            ),
                            Math.Max(
                                Math.Abs(c.DashStickyRetentionExists ? c.DashStickyRetentionSpeed.Y : 0f),
                                Math.Abs(beforeUpdateSpeed.Y)
                            )
                        ).Length(),
                        bounceSpeed.Length()
                    );

                return bounceSpeed * sign;
                // return bounceSpeed.Sign() * Vector2.Max(GooberPlayerExtensions.Instance.BounceSpeedPreserved.Abs(), bounceSpeed.Abs());
            });
            cursor.EmitStarg(2);
        }

        private void modifyPlayerWindMove(ILContext il) {
            var cursor = new ILCursor(il);

            cursor.EmitLdarg0();
            cursor.EmitLdarg1();
            cursor.EmitDelegate((Player player, Vector2 move) => {
                if(
                    GetOptionValue(Option.AllowWindWhileDashing) == (int)AllowWindWhileDashingValue.Speed &&
                    player.DashDir == Vector2.Zero &&
                    player.StateMachine.State == 2
                ) {
                    GooberPlayerExtensions.Instance.DashWindBoost = move / Engine.DeltaTime;                    
                }
            });

            if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcI4(2))) {
                cursor.EmitDelegate((int orig) => {
                    return GetOptionValue(Option.AllowWindWhileDashing) == (int)AllowWindWhileDashingValue.Velocity
                        ? -1
                        : orig;
                });
            }
        }

        private void modPlayerNormalBegin(On.Celeste.Player.orig_NormalBegin orig, Player self) {
            float originalMaxFall = self.maxFall;
            
            orig(self);

            if(GetOptionValue(Option.DownwardsJumpSpeedPreservationThreshold) != -1) self.maxFall = originalMaxFall;
        }

        private void modTheoCrystalCtor(On.Celeste.TheoCrystal.orig_ctor_Vector2 orig, TheoCrystal self, Vector2 position) {
            orig(self, position);

            self.Add(new TheoNuclearReactor());
        }

        private void modHoldableRelease(On.Celeste.Holdable.orig_Release orig, Holdable self, Vector2 force) {
            var holder = self.Holder as Entity;
                        
            orig(self, force);
            
            if(holder is not Player player)
                return;

            if(GetOptionBool(Option.ReverseBackboosts) && Math.Sign(force.X) == Math.Sign(player.Speed.X))
                player.Speed.X *= -1;

            if(!GetOptionBool(Option.HoldablesInheritSpeedWhenThrown))
                return;

            Vector2 holdableSpeed = self.SpeedGetter.Invoke();
            float newLaunchSpeed = force.X * Math.Max(Math.Abs(holdableSpeed.X), Math.Abs(player.Speed.X) * 0.8f);

            self.SpeedSetter.Invoke(new Vector2(newLaunchSpeed, holdableSpeed.Y));
        }

        // private bool modPlayerPickup(On.Celeste.Player.orig_Pickup orig, Player self, Holdable pickup) {
        //     var ducking = self.Ducking;
            
        //     var value = orig(self, pickup);

        //     if(GetOptionBool(Option.AllowCrouchedHoldableGrabbing))
        //         self.Ducking = ducking;

        //     return value;
        // }

        private void modifyPlayerPickup(ILContext il) {
            var cursor = new ILCursor(il);

            var endLabel = cursor.DefineLabel();

            HookHelper.Begin(cursor, "allowing crouched holdable grabbing");

            HookHelper.Move("moving before set_Ducking", () => {
                cursor.GotoNext(MoveType.AfterLabel,
                    instr => instr.MatchLdarg0(),
                    instr => instr.MatchLdcI4(0),
                    instr => instr.MatchCallOrCallvirt<Player>("set_Ducking")
                );
            });

            HookHelper.Do(() => {
                cursor.EmitDelegate(() => GetOptionBool(Option.AllowCrouchedHoldableGrabbing));
                cursor.EmitBrtrue(endLabel);
            });

            HookHelper.Move("setting label after set_Ducking", () => {
                cursor.GotoNext(MoveType.After, instr => instr.MatchCallOrCallvirt<Player>("set_Ducking"));
            });

            HookHelper.Do(() => {
                cursor.MarkLabel(endLabel);
            });

            HookHelper.End();
        }

        private void allowAllDirectionDreamJumps(ILCursor cursor) {
            if(cursor.TryGotoNext(MoveType.After,
                instr => instr.MatchLdfld<Vector2>("X"),
                instr => instr.MatchLdcR4(0f)
            )) {
                cursor.EmitDelegate((float value) => {
                    return GetOptionBool(Option.AllDirectionDreamJumps) ? 100f : value; //dummy value
                });
            }
        }

        //code stolen from https://github.com/EverestAPI/CelesteTAS-EverestInterop/blob/c3595e5af47bde0bca28e4693c80c180434c218c/CelesteTAS-EverestInterop/Source/EverestInterop/Hitboxes/CycleHitboxColor.cs
        //very helpful resource for this
        private void modSceneBeforeUpdate(On.Monocle.Scene.orig_BeforeUpdate orig, Scene self) {
            if(self is not Level) {
                orig(self);

                return;
            }

            float timeActive = self.TimeActive;
            GooberPlayerExtensions c = GooberPlayerExtensions.Instance;

            orig(self);

            if(Math.Abs(timeActive - self.TimeActive) > 0.000001f && c != null) {
                c.Counter++;
            }
        }

        private void modLevelUpdate(On.Celeste.Level.orig_Update orig, Level self) {
            GooberPlayerExtensions c = GooberPlayerExtensions.Instance;

            if(c == null) {
                orig(self);

                return;
            }

            if(GetOptionBool(Option.RefillFreezeGameSuspension) && c.FreezeFrameFrozen) {
                var newInputs = new Utils.InputState();
                
                if(c.FreezeFrameFrozenInputs.FarEnoughFrom(newInputs)) {
                    c.FreezeFrameFrozen = false;

                    Celeste.Freeze(0.01f);
                } else {
                    self.Camera.Position = self.Camera.position + ((c.Entity as Player).CameraTarget - self.Camera.position) * (1f - (float)Math.Pow(0.01f, Engine.DeltaTime));

                    //CODE DIRECTLY COPIED FROM SPEEDRUNTOOL StateManager.cs
                    self.Wipe?.Update(self);
                    self.HiresSnow?.Update(self);
                    self.Foreground.Update(self);
                    self.Background.Update(self);
                    Engine.Scene.Tracker.GetEntity<CassetteBlockManager>()?.Update();
                    foreach(var entity in Engine.Scene.Tracker.GetEntities<CassetteBlock>()) {
                        entity.Update();
                    }

                    foreach(var listener in Engine.Scene.Tracker.GetComponents<CassetteListener>()) {
                        listener.Entity.Update();
                    }

                    GameSuspensionIgnore.UpdateEntities();

                    return;
                }
            }

            if(GetOptionBool(Option.LenientStunning) && !self.Paused && c.StunningWatchTimer > 0f) {
                int offsetGroup = getOffsetGroup(c.StunningOffset);
                bool drifted = offsetGroup != c.StunningGroup;
                //i was going through trials and tribulations while trying to make this account for drift 😭
                //dont worry about the various console logs
                //i should really figure out how to use a debugger huh 

                // Console.WriteLine("-- watching");
                // Console.WriteLine("offset group: " + offsetGroup);
                // Console.WriteLine("drifted: " + drifted);
                // Console.WriteLine("stunning offset: " + c.StunningOffset);
                // Console.WriteLine("group offset: " + getGroupOffset(c.StunningGroup));

                if(drifted) {
                    c.StunningOffset = getGroupOffset(c.StunningGroup);// (c.StunningOffset - Engine.DeltaTime + 0.05f) % 0.05f;
                    
                    setStunnableEntityOffset(c.StunningOffset);
                }

                // Console.WriteLine("new group: " + getOffsetGroup(c.StunningOffset));
                // Console.WriteLine("new offset: " + c.StunningOffset);
                // Console.WriteLine("--");

                c.StunningWatchTimer -= Engine.DeltaTime;
            }

            orig(self);
        }

        //code stolen from https://github.com/EverestAPI/CelesteTAS-EverestInterop/blob/c3595e5af47bde0bca28e4693c80c180434c218c/CelesteTAS-EverestInterop/Source/EverestInterop/Hitboxes/CycleHitboxColor.cs
        private int getOffsetGroup(float offset) {
            float time = Engine.Scene.TimeActive;
            int timeDist = 0;

            while (Math.Floor(((double) time - offset - Engine.DeltaTime) / 0.05f) >= Math.Floor(((double) time - offset) / 0.05f) && timeDist < 3) {
                time += Engine.DeltaTime;
                timeDist++;
            }

            return timeDist < 3 ? (timeDist + GooberPlayerExtensions.Instance.Counter) % 3 : 3;
        }

        private float getGroupOffset(int targetGroup) {
            //terrible
            //terrible
            for(float i = 0; i < 1f; i += Engine.DeltaTime * 0.5f) {
                if(getOffsetGroup(i) == targetGroup) return i;
            }

            return -1;
        }

        private void setStunnableEntityOffset(float offset) {
            //i know using reflection for something like this is freaky as fuck,
            //but afaik this method looks optimized enough given that this method
            //only runs once upon pausing the game. feel free to ping me in modding
            //feedback and suggest an alternative 😭

            //also god damn this language is hot with the variable declaration inside of if statements thing
            //i want that in javascript or typescript so badly
            
            //look at me documenting my own code with comments
            //i should do this more often

            using (List<Component>.Enumerator enumerator = Engine.Scene.Tracker.GetComponents<PlayerCollider>().GetEnumerator()) {
				while (enumerator.MoveNext()) {
                    if (enumerator.Current.Entity is CrystalStaticSpinner spinner && !spinner.Collidable)
                        spinner.offset = offset;

                    if (enumerator.Current.Entity is Lightning lightning && !lightning.Collidable)
                        lightning.toggleOffset = offset;

                    if (enumerator.Current.Entity is DustStaticSpinner dust && !dust.Collidable)
                        dust.offset = offset;
                }
			}
        }

        private void modLevelPause(On.Celeste.Level.orig_Pause orig, Level self, int startIndex, bool minimal, bool quickReset) {
            orig(self, startIndex, minimal, quickReset);

            if(!GetOptionBool(Option.LenientStunning)) return;

            GooberPlayerExtensions c = GooberPlayerExtensions.Instance;
            
            if(c == null) return;

            //dont let the player pause buffer to mimic spinner stunning
            //11 because unpausing time still adds to the counter
            if(c.Counter <= c.LastPauseCounterValue + 11) {
                // Console.WriteLine("zog");
                c.LastPauseCounterValue = c.Counter;

                return;
            }
            c.LastPauseCounterValue = c.Counter;

            float offset = 0f;

            //i dont think it should ever reach 1 but better to be safe than to receive a surprise modding feedback ping
            while (!self.OnInterval(0.05f, offset) && offset < 5f) {
                offset += Engine.DeltaTime / 2f;
            }

            c.StunningWatchTimer = 0.2f;
            c.StunningOffset = offset;
            c.StunningGroup = getOffsetGroup(offset);

            setStunnableEntityOffset(offset);
            
            // Console.WriteLine("offsetGroup: " + c.StunningGroup);
            // Console.WriteLine("offset: " + offset);
        }

        private bool modPlayerSwimCheck(On.Celeste.Player.orig_SwimCheck orig, Player self) {
            if(self.CollideAll<Water>().Any(water => water is Waterfall && (water as Waterfall).nonCollidable))
                return false;
            
            return orig(self);
        }

        private bool modPlayerSwimJumpCheck(On.Celeste.Player.orig_SwimJumpCheck orig, Player self) {
            if(self.CollideAll<Water>().Any(water => water is Waterfall && (water as Waterfall).nonCollidable))
                return false;
            
            return orig(self);
        }

        private bool modPlayerUnderwaterMusicCheck(On.Celeste.Player.orig_UnderwaterMusicCheck orig, Player self) {
            if(self.CollideAll<Water>().Any(water => water is Waterfall && (water as Waterfall).nonCollidable))
                return false;
            
            return orig(self);
        }

        private void modPlayerHairRender(On.Celeste.PlayerHair.orig_Render orig, PlayerHair self) {
            //i need the custom shader to not execute if the startedRendering boolean is false
            //this took way longer than it shouldve to figure out
            //there was a bug where the player trail would be offset from the player for some odd reason
            //i assumed it was a shader problem or more general thing for a while, but i eventually had the idea that it might be specific to the TrailManager (it was)
            //in TrailManager.BeforeRender(), it interrupts the spritebatch and renders specifically the PlayerHair with this method
            //that causes Something to mess up and somehow shift the player trail
            //the startedRendering boolean is set to true when the actual render method is called
            //that Should prevent this method from executing the custom shader code
            //i should document these things more often
            if(GetOptionValue(Option.PlayerShaderMask) != (int)PlayerShaderMaskValue.HairOnly || !startedRendering) {
                orig(self);

                return;
            }

            if(self.Entity is not Player) return;

            doPlayerMaskStuffBefore(new Vector4((self.Entity as Player).Hair.Color.ToVector3() * (self.Entity as Player).Sprite.Color.ToVector3(), 1), true);

            orig(self);

            doPlayerMaskStuffAfter();

            startedRendering = false;
        }

        private void modPlayerDeadBodyRender(On.Celeste.PlayerDeadBody.orig_Render orig, PlayerDeadBody self) {
            if(GetOptionValue(Option.PlayerShaderMask) != (int)PlayerShaderMaskValue.Cover) {
                orig(self);

                return;
            }

            doPlayerMaskStuffBefore(lastPlayerHairColor.ToVector4());

            orig(self);

            doPlayerMaskStuffAfter();
        }
        
        private void modPlayerRender(On.Celeste.Player.orig_Render orig, Player self) {            
            startedRendering = true;

            if(GetOptionValue(Option.PlayerShaderMask) != (int)PlayerShaderMaskValue.Cover) {
                orig(self);

                return;
            }

            lastPlayerHairColor = self.Hair.Color;

            doPlayerMaskStuffBefore(new Vector4(self.Hair.Color.ToVector3() * self.Sprite.Color.ToVector3(), 1));

            orig(self);

            doPlayerMaskStuffAfter();
        }

        private void doPlayerMaskStuffBefore(Vector4 color, bool keepOutlines = false) {
            playerMaskEffect = ModIntegration.FrostHelperAPI.GetEffectOrNull.Invoke("playerMask");

            if((Engine.Scene as Level) == null) return;

            GameplayRenderer.End();

            Texture2D tex = GFX.Game["GooberHelper/mask"].Texture.Texture;
            
            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, playerMaskEffect, (Engine.Scene as Level).GameplayRenderer.Camera.Matrix);
            playerMaskEffect.CurrentTechnique = playerMaskEffect.Techniques["Grongle"];
            playerMaskEffect.Parameters["CamPos"].SetValue((Engine.Scene as Level).Camera.Position);
            playerMaskEffect.Parameters["HairColor"].SetValue(color);
            playerMaskEffect.Parameters["TextureSize"].SetValue(new Vector2(tex.Width, tex.Height));
            playerMaskEffect.Parameters["Time"].SetValue(Engine.Scene.TimeActive);
            playerMaskEffect.Parameters["KeepOutlines"].SetValue(keepOutlines);
            Engine.Graphics.GraphicsDevice.Textures[1] = tex;
        }

        private void doPlayerMaskStuffAfter() {
            Draw.SpriteBatch.End();
            GameplayRenderer.Begin();
        }

        private void modPlayerRedBoost(On.Celeste.Player.orig_RedBoost orig, Player self, Booster booster) {
            GooberPlayerExtensions.Instance.BoostSpeedPreserved = self.Speed;

            orig(self, booster);
        }

        private void modPlayerBoost(On.Celeste.Player.orig_Boost orig, Player self, Booster booster) {
            GooberPlayerExtensions.Instance.BoostSpeedPreserved = self.Speed;

            orig(self, booster);
        }

        private void modifyDashSpeedThing(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNext(MoveType.After, instr => instr.Match(OpCodes.Ldc_R4, 240f))) {
                cursor.EmitDelegate((float value) => {
                    if(!GetOptionBool(Option.BubbleSpeedPreservation))
                        return value;

                    GooberPlayerExtensions c = GooberPlayerExtensions.Instance;

                    value = Math.Max(c.BoostSpeedPreserved.Length(), value);

                    c.BoostSpeedPreserved = Vector2.Zero;

                    return value;
                });
            }
        }

        private void modPlayerBeforeUpTransition(On.Celeste.Player.orig_BeforeUpTransition orig, Player self) {
            if(!GetOptionBool(Option.UpwardsTransitionSpeedPreservation)) {
                orig(self);

                return;
            }

            float varJumpTimer = self.varJumpTimer;
            float varJumpSpeed = self.varJumpSpeed;
            Vector2 speed = self.Speed;
            float dashCooldownTimer = self.dashCooldownTimer;

            orig(self);

            self.varJumpTimer = varJumpTimer;
            self.varJumpSpeed = varJumpSpeed;
            self.Speed = speed;
            self.dashCooldownTimer = dashCooldownTimer;
        }

        // public void modHoldableRelease(On.Celeste.Holdable.orig_Release orig, Holdable self, Vector2 force) {
        //     orig(self, force);

        //     Vector2 speed = DynamicData.For(self.Entity).Get<Vector2>("Speed"); 

        //     Player player = Engine.Scene.Tracker.GetEntity<Player>();

        //     DynamicData.For(self.Entity).Set("Speed", new Vector2(Math.Max(Math.Abs(player.Speed.X), Math.Abs(speed.X)) * Math.Sign(speed.X), speed.Y));
        // }

        // public void modifyPlayerThrow(ILContext il) {
        //     var cursor = new ILCursor(il);

        //     cursor.TryGotoNext(MoveType.After, instr => instr.OpCode == OpCodes.Callvirt);

        //     cursor.EmitDelegate(() => {
        //         Player player = Engine.Scene.Tracker.GetEntity<Player>();
        //         Logger.Log(LogLevel.Info, "i", player.Speed.ToString());

                
        //         Vector2 speed = DynamicData.For(player.Holding.Entity).Get<Vector2>("Speed");

        //         Logger.Log(LogLevel.Info, "i", player.Holding.Holder.ToString());
        //         Logger.Log(LogLevel.Info, "f", speed.ToString());
        //         DynamicData.For(player.Holding.Entity).Set("Speed", speed + Vector2.UnitX * Math.Abs(player.Speed.X) * Math.Sign(speed.X));

        //         speed = DynamicData.For(player.Holding.Entity).Get<Vector2>("Speed");

        //         Logger.Log(LogLevel.Info, "g", speed.ToString());
        //     });
        // }
        
        private void allowAllDirectionHypersAndSupers(ILCursor cursor, OpCode nextInstruction, bool alwaysRefills) {
            ILLabel superJumpLabel = cursor.DefineLabel();

            //this is really stupid
            //i would Like to just go before the ldarg0 instruction, but theres a label that points to it and i need my code to run after whereever that labels points
            //pop and reinsert it is

            HookHelper.Begin(cursor, "allowing all direction hypers and supers");

            HookHelper.Move("finding where SuperJump is called", () => {
                cursor.GotoNextBestFit(MoveType.AfterLabel,
                    instr => instr.MatchLdarg0(),
                    instr => instr.MatchCallOrCallvirt<Player>("SuperJump"),
                    instr => instr.MatchLdcI4(0),
                    instr => instr.MatchRet()
                );
            });

            HookHelper.Do(() => {
                cursor.MarkLabel(superJumpLabel);
            });

            HookHelper.Move("finding where the method returns after calling SuperJump", () => {
                cursor.GotoNext(MoveType.After, 
                    instr => instr.MatchRet(),
                    instr => instr.OpCode == nextInstruction
                );

                cursor.MoveAfterLabels();
            });

            HookHelper.Do(() => {
                cursor.EmitPop();
                cursor.EmitLdarg0();
                cursor.EmitDelegate((Player player) => {
                    var allDirectionHypersAndSupersValue = (AllDirectionHypersAndSupersValue)GetOptionValue(Option.AllDirectionHypersAndSupers);

                    if(allDirectionHypersAndSupersValue == AllDirectionHypersAndSupersValue.None)
                        return false;
                    
                    var extvarsJumpCount = ModIntegration.ExtendedVariantModeAPI.GetJumpCount?.Invoke() ?? 0;
                    
                    //inverse of original conditions
                    //the <= 0f term is there so you dont accidentally super while trying to wavedash
                    if(!player.CanUnDuck || !Input.Jump.Pressed || player.Speed.Y > 0f)
                        return false;
                    
                    //real stuff
                    var coyoteCondition =
                        (player.jumpGraceTimer > 0f || extvarsJumpCount > 0) && 
                        allDirectionHypersAndSupersValue != AllDirectionHypersAndSupersValue.RequireGround; //WorkWithCoyoteTime or WorkWithCoyoteTimeAndRefill

                    var groundedCondition =
                        (player.CollideCheck<JumpThru>(player.Position + Vector2.UnitY * player.Collider.Height) && player.CollideCheck<JumpThru>(player.Position + Vector2.UnitY)) ||
                        player.CollideCheck<Solid>(player.Position + Vector2.UnitY);

                    if(!coyoteCondition && !groundedCondition)
                        return false;

                    //actual logic
                    var canMaybeRefill = groundedCondition || allDirectionHypersAndSupersValue == AllDirectionHypersAndSupersValue.WorkWithCoyoteTime;

                    if(alwaysRefills || canMaybeRefill && player.dashRefillCooldownTimer <= 0f && !player.Inventory.NoRefills)
                        player.RefillDash();

                    if(!groundedCondition && coyoteCondition && player.jumpGraceTimer <= 0f)
                        ModIntegration.ExtendedVariantModeAPI.SetJumpCount?.Invoke(extvarsJumpCount - 1);

                    return true;
                });

                cursor.EmitBrtrue(superJumpLabel);
                cursor.Emit(nextInstruction);
            });

            HookHelper.End();
        }

        private void allowHoldableClimbjumping(ILCursor cursor) {
            for(int i = 0; i < 2; i++) {
                if(
                    cursor.TryGotoNextBestFit(MoveType.After,
                        instr => instr.MatchLdfld<Player>("Stamina"),
                        instr => instr.MatchLdcR4(0),
                        instr => instr.OpCode == OpCodes.Ble_Un_S,
                        instr => instr.MatchLdarg(0),
                        instr => instr.MatchCallvirt<Player>("get_Holding"),
                        instr => instr.OpCode == OpCodes.Brtrue_S
                    )
                ) {
                    cursor.Index--;
                    cursor.EmitDelegate((Holdable value) => {
                        if(!GetOptionBool(Option.AllowHoldableClimbjumping)) return value;

                        return null;
                    });
                } else {
                    Logger.Error("GooberHelper", "COULDNT ALLOW THEO CLIMBJUMPING; PLEASE CONTACT @ZUCCANIUM");
                }
            }
        }

        private void modifyPlayerDreamDashUpdate(ILContext il) {
            allowAllDirectionDreamJumps(new ILCursor(il));
        }
        
        private void modifyPlayerDreamDashEnd(ILContext il) {
            allowAllDirectionDreamJumps(new ILCursor(il));
        }

        private void modifyPlayerLaunchUpdate(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNext(MoveType.After,
                instr => instr.MatchCallOrCallvirt<Player>("get_Ducking"),
                instr => instr.MatchBrtrue(out var _)
            )) {
                cursor.Index--;

                cursor.EmitDelegate((bool value) => {
                    return GetOptionBool(Option.AllowCrouchedHoldableGrabbing) ? false : value;
                });
            }
        }

        private void modifyPlayerRedDashUpdate(ILContext il) {
            allowHoldableClimbjumping(new ILCursor(il));
            allowAllDirectionHypersAndSupers(new ILCursor(il), OpCodes.Ldloc_0, true);
        }

        private void modifyPlayerHitSquashUpdate(ILContext il) {
            allowHoldableClimbjumping(new ILCursor(il));
        }

        private void modifyPlayerNormalUpdate(ILContext il) {
            allowHoldableClimbjumping(new ILCursor(il));
            guardNormalUpdateForOnlyClimbing(new ILCursor(il));

            var cursor = new ILCursor(il);
            ILLabel getOutLabel = null;
            bool gotInLegally = false;

            //enter the if statement illegally if some options are enabled
            if(cursor.TryGotoNext(MoveType.After,
                instr => instr.MatchCallOrCallvirt<Player>("get_Ducking"),
                instr => instr.MatchBrtrue(out getOutLabel)
            )) {
                cursor.Index--;

                cursor.EmitDelegate((bool value) => {
                    gotInLegally = !value;

                    return 
                        GetOptionBool(Option.AllowCrouchedHoldableGrabbing) ||
                        GetOptionBool(Option.AllowCrouchedClimbGrabbing) ||
                        GetOptionBool(Option.AllowUpwardsClimbGrabbing)
                        ? false
                        : value;
                });
            }

            //guard the foreach loop that lets you grab holdables to only run if
            //a. you got into here legally (without gooberhelper modifications)
            //b. crouched holdable grabbing is enabled
            if(cursor.TryGotoNextBestFit(MoveType.AfterLabel,
                instr => instr.MatchLdarg0(),
                instr => instr.MatchCallOrCallvirt<Entity>("get_Scene"),
                instr => instr.MatchCallOrCallvirt<Scene>("get_Tracker"),
                instr => instr.MatchCallOrCallvirt(typeof(Tracker).GetMethod("GetComponents").MakeGenericMethod(typeof(Holdable))),
                instr => instr.MatchCallOrCallvirt<List<Component>>("GetEnumerator")
            )) {
                ILLabel label = cursor.DefineLabel();

                cursor.EmitDelegate(() => gotInLegally || GetOptionBool(Option.AllowCrouchedHoldableGrabbing));
                cursor.EmitBrfalse(label);

                if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchEndfinally())) {
                    cursor.MoveAfterLabels();
                    cursor.MarkLabel(label);

                    //get out of the illegal area if the only resaon to be in there was crouched holdable grabbing
                    cursor.EmitDelegate(() => !gotInLegally && !GetOptionBool(Option.AllowCrouchedClimbGrabbing) && !GetOptionBool(Option.AllowUpwardsClimbGrabbing));
                    cursor.EmitBrtrue(getOutLabel);
                }
            }

            if(cursor.TryGotoNextBestFit(MoveType.After,
                instr => instr.MatchLdarg0(),
                instr => instr.MatchLdflda<Player>("Speed"),
                instr => instr.MatchLdfld<Vector2>("Y"),
                instr => instr.MatchLdcR4(0)
            )) {
                //make the condition hold true always if upwards climb grabbing is enabled
                cursor.EmitDelegate((float value) => {
                    return GetOptionBool(Option.AllowUpwardsClimbGrabbing) ? float.MinValue : value;
                });

                //add a CanUnDuck to make sure the player doesnt clip into the ceiling
                ILLabel failedSpeedCheckLabel = null;
                cursor.GotoNext(MoveType.After, instr => instr.MatchBltUn(out failedSpeedCheckLabel));

                cursor.EmitLdarg0();
                cursor.EmitDelegate((Player player) => !player.CanUnDuck && GetOptionBool(Option.AllowUpwardsClimbGrabbing));
                cursor.EmitBrtrue(failedSpeedCheckLabel);
            }

            //dont let it set player.Ducking to true if the allow crouched grabbing option is set
            if(cursor.TryGotoNextBestFit(MoveType.Before,
                instr => instr.MatchLdarg0(),
                instr => instr.MatchLdcI4(0),
                instr => instr.MatchCallOrCallvirt<Player>("set_Ducking")
            )) {
                ILLabel label = cursor.DefineLabel();

                cursor.EmitDelegate(() => GetOptionBool(Option.AllowCrouchedClimbGrabbing));
                cursor.EmitBrtrue(label);

                cursor.GotoNext(MoveType.After, instr => instr.MatchCallOrCallvirt<Player>("set_Ducking"));
                cursor.MarkLabel(label);
            }
        
            //make sure the player doesnt unduck with a holdable (this was originally in allowcrouchedholdablegrabbing)
            if(cursor.TryGotoNextBestFit(MoveType.After,
                instr => instr.MatchBltUn(out _),
                instr => instr.MatchLdarg0(),
                instr => instr.MatchCallOrCallvirt<Player>("get_CanUnDuck"),
                instr => instr.MatchBrfalse(out _)
            )) {
                int ifBodyIndex = cursor.Index;

                ILLabel label = null;
                cursor.GotoNext(MoveType.After, instr => instr.MatchBr(out label));

                cursor.Index = ifBodyIndex;
                cursor.EmitDelegate(() => GetOptionBool(Option.AllowCrouchedHoldableGrabbing));
                cursor.EmitBrtrue(label);
            }

            if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(900f))) {
                cursor.EmitLdarg0();
                cursor.EmitLdloc(8);
                cursor.EmitDelegate((float value, Player player, float fastfallSpeed) => {
                    //400f is default movement-direction-aligned air friction
                    //0.65f is the default multiplier on horizontal air friction while midair
                    //they call me the magic number documenter
                    return GetOptionBool(Option.DownwardsAirFrictionBehavior) && Math.Abs(player.Speed.Y) > fastfallSpeed && Math.Sign(player.Speed.Y) == Input.MoveY ? 400f * 0.65f : value;
                });
            }

            if(cursor.TryGotoNextBestFit(MoveType.After,
                instr => instr.MatchLdarg0(),
                instr => instr.MatchLdfld<Player>("varJumpSpeed"),
                instr => instr.MatchCallOrCallvirt(out _) //math.min
            )) {
                cursor.EmitLdarg0();
                cursor.EmitDelegate((float value, Player player) => {
                    if(GetOptionValue(Option.DownwardsJumpSpeedPreservationThreshold) == -1) return value;

                    float varJumpSpeed = player.varJumpSpeed;

                    return varJumpSpeed > 0 && !player.onGround ? varJumpSpeed : value;
                });
            }
        }

        //make normalupdate skip past certain code if its only running to check if climbing can occur
        private bool guardNormalUpdateForOnlyClimbing(ILCursor cursor) {
            Func<bool> guard = () => runningNormalUpdateJustForClimbing;
            
            ILCursor startCursor = cursor.Clone();

            ILLabel climbingStuffStartLabel = cursor.DefineLabel();
            ILLabel afterHoldingCheckLabel = null;
            ILLabel afterGrabCheckLabel = null;
            ILLabel afterClimbCheckLabel = null;

            if(!cursor.TryGotoNextBestFit(MoveType.Before,
                instr => instr.MatchStfld<Vector2>("Y"),
                instr => instr.MatchLdarg0(),
                instr => instr.MatchCallOrCallvirt<Player>("get_Holding")
            )) {
                return false;
            }

            cursor.GotoNext(MoveType.AfterLabel, instr => instr.MatchLdarg0());
            cursor.MarkLabel(climbingStuffStartLabel);

            if(!cursor.TryGotoNextBestFit(MoveType.After,
                instr => instr.MatchCallOrCallvirt<Player>("get_Holding"),
                instr => instr.MatchBrtrue(out afterHoldingCheckLabel)
            )) {
                return false;
            }

            if(!cursor.TryGotoNextBestFit(MoveType.After,
                instr => instr.MatchCallOrCallvirt<Player>("get_IsTired"), //yes i know this isnt grabcheck but i didnt want to do weird reflection to get the getter
                instr => instr.MatchBrtrue(out afterGrabCheckLabel)
            )) {
                return false;
            }

            if(!cursor.TryGotoNextBestFit(MoveType.After,
                instr => instr.MatchCallOrCallvirt<Player>("ClimbCheck"),
                instr => instr.MatchBrfalse(out afterClimbCheckLabel)
            )) {
                return false;
            }

            //skip past the first stuff
            cursor = startCursor;
            cursor.EmitDelegate(guard);
            cursor.EmitBrtrue(climbingStuffStartLabel);

            ILLabel[] labels = [afterHoldingCheckLabel, afterGrabCheckLabel, afterClimbCheckLabel];

            foreach(ILLabel label in labels) {
                cursor.GotoLabel(label);
                
                ILLabel afterReturnLabel = cursor.DefineLabel();

                //if the guard is in place, return -1
                cursor.EmitDelegate(guard);
                cursor.EmitBrfalse(afterReturnLabel);
                cursor.EmitLdcI4(-1);
                cursor.EmitRet();
                cursor.MarkLabel(afterReturnLabel);
            }

            return true;
        }

        private bool runNormalUpdateJustForClimbing(Player player) {  
            runningNormalUpdateJustForClimbing = true;

            int result = player.NormalUpdate();

            runningNormalUpdateJustForClimbing = false;

            return result == 1;
        }

        private void modifyPlayerDashUpdate(ILContext il) {
            allowHoldableClimbjumping(new ILCursor(il));
            allowAllDirectionHypersAndSupers(new ILCursor(il), OpCodes.Ldarg_0, false);

            var cursor = new ILCursor(il);

            if(cursor.TryGotoNextBestFit(MoveType.AfterLabel, 
                instr => instr.MatchLdarg0(),
                instr => instr.MatchLdflda<Player>("DashDir"),
                instr => instr.MatchLdfld<Vector2>("Y"),
                instr => instr.MatchCallOrCallvirt(((Func<float, float>)Math.Abs).Method),
                instr => instr.MatchLdcR4(0.1f)
            )) {
                ILLabel afterReturnLabel = cursor.DefineLabel();

                cursor.EmitLdarg0();
                cursor.EmitDelegate((Player player) => {
                    if(GetOptionBool(Option.AllowClimbingInDashState) && player.DashDir != Vector2.Zero) return runNormalUpdateJustForClimbing(player);

                    return false;
                });

                cursor.EmitBrfalse(afterReturnLabel);

                //return 1
                cursor.EmitLdcI4(1);
                cursor.EmitRet();

                cursor.MarkLabel(afterReturnLabel);
            }

            if(cursor.TryGotoNextBestFit(MoveType.Before, 
                instr => instr.MatchLdarg0(),
                instr => instr.MatchCallOrCallvirt<Player>("get_SuperWallJumpAngleCheck"),
                instr => instr.MatchBrfalse(out _)
            )) {
                ILLabel afterReturnLabel = cursor.DefineLabel();

                cursor.MoveAfterLabels();

                //if(customswimming && tryCustomSwimmingJump(this, this.DashDir))
                cursor.EmitLdarg0();
                cursor.EmitDelegate((Player player) => {
                    return GetOptionBool(Option.CustomSwimming) && Input.Jump && tryCustomSwimmingWalljump(player, player.DashDir);
                });

                cursor.EmitBrfalse(afterReturnLabel);

                //return StSwim;
                cursor.EmitLdcI4(Player.StSwim);
                cursor.EmitRet();

                cursor.MarkLabel(afterReturnLabel);
            }
        }

        private void modifyPlayerPickupCoroutine(ILContext il) {
            var cursor = new ILCursor(il);

            cursor.TryGotoNext(MoveType.After, instr => instr.MatchStfld(typeof(Player), nameof(Player.Speed)));
            cursor.TryGotoNext(MoveType.After, instr => instr.MatchStfld(typeof(Player), nameof(Player.Speed)));

            cursor.EmitLdloc1();
            cursor.EmitDelegate((Player player) => {
                if(GetOptionBool(Option.PickupSpeedInversion) && -Math.Sign(player.Speed.X) == (int)Input.MoveX)
                    player.Speed.X *= -1;
            });
        }

        private void makeGoldenBlocksOrSimilarEntitiesAlwaysLoad(ILContext il) {
            var cursor = new ILCursor(il);

            //iteration #1 is for the flag local boolean on golden and silver blocks
            //iteration #2 is for the flag2 local boolean on platinum blocks
            for(int i = 0; i < 2; i++) {
                if(cursor.TryGotoNext(MoveType.After,
                    instr => instr.OpCode == OpCodes.Ldc_I4_0,
                    instr => instr.OpCode == (i == 0 ? OpCodes.Stloc_0 : OpCodes.Stloc_1)
                )) {
                    cursor.Index--;
                    cursor.EmitDelegate((int value) => 
                        GetOptionBool(Option.GoldenBlocksAlwaysLoad)
                            ? 1
                            : value
                    );
                }
            }
        }

        private void modPlayerSuperJump(On.Celeste.Player.orig_SuperJump orig, Player self) {
            // Vector2 originalSpeed = self.Speed;
            // bool wasDucking = self.Ducking;

            orig(self);

            // handleVerticalSpeedToHorizontal(self, originalSpeed);

            // if(GetOptionBool(Option.HyperAndSuperSpeedPreservation)) {
            //     //this exists so that alldirectionsHypersAndSupers can be compatible
            //     //i dont think it will break anything else                                                                                                                         :cluel:
            //     float kindaAbsoluteSpeed = originalSpeed.Length() == 0 ? self.beforeDashSpeed.Length() : originalSpeed.Length();
                
            //     self.Speed.X = (int)self.Facing * Math.Max(Math.Abs(kindaAbsoluteSpeed), Math.Abs(260f * (wasDucking ? 1.25f : 1f))) + self.LiftBoost.X;
            // }

            // if(GetOptionBool(Option.AdditiveVerticalJumpSpeed)) {
            //     self.Speed.Y = Math.Min(self.Speed.Y, self.varJumpSpeed + Math.Min(originalSpeed.Y, 0));

            //     self.varJumpSpeed = self.Speed.Y;
            // }  
        }

        private void modifyPlayerSuperJump(ILContext il) {
            var cursor = new ILCursor(il);

            var originalSpeed = default(Vector2);

            cursor.EmitLdarg0();
            cursor.EmitDelegate((Player player) => {
                originalSpeed = player.GetConservedSpeed();
            });

            if(cursor.TryGotoNext(MoveType.AfterLabel,
                instr => instr.MatchLdarg0(),
                instr => instr.MatchLdfld<Player>("Speed"),
                instr => instr.MatchLdarg0(),
                instr => instr.MatchCallOrCallvirt<Player>("get_LiftBoost")
            )) {
                cursor.EmitLdarg0();
                cursor.EmitDelegate((Player player) => {
                    if(player.Ducking)
                        player.Speed *= new Vector2(1.25f, 0.5f);

                    var hyperAndSuperSpeedPreservationValue = 0f;

                    if(GetOptionBool(Option.HyperAndSuperSpeedPreservation))
                        hyperAndSuperSpeedPreservationValue = originalSpeed.X;

                    player.Speed.X = Math.Sign(player.Speed.X) * Math.Max(
                        Math.Abs(player.Speed.X),
                        Math.Abs(hyperAndSuperSpeedPreservationValue)
                    );

                    if(GetOptionBool(Option.AdditiveVerticalJumpSpeed))
                        player.Speed.Y = Math.Min(player.Speed.Y, player.Speed.Y + Math.Min(originalSpeed.Y, 0));

                    if(player.Ducking)
                        player.Speed /= new Vector2(1.25f, 0.5f);
                });
            }
        }

        private void modPlayerNormalEnd(On.Celeste.Player.orig_NormalEnd orig, Player self) {
            if(self.StateMachine.State == 2) {
                var c = GooberPlayerExtensions.Instance;

                c.BeforeDashSpeedConserved = self.GetConservedSpeed();

                Console.WriteLine(c.BeforeDashSpeedConserved);

                if(self.wallSpeedRetentionTimer > 0) {
                    c.DashStickyRetentionExists = true;
                    c.DashStickyRetentionDirection = new Vector2(Math.Sign(self.wallSpeedRetained), 0);
                    c.DashStickyRetentionSpeed = new Vector2(self.wallSpeedRetained, 0);
                }
            }

            if(GetOptionBool(Option.RemoveNormalEnd))
                return;

            orig(self);
        }

        private void modPlayerDashBegin(On.Celeste.Player.orig_DashBegin orig, Player self) {
            orig(self);
        }

        private void modCrystalStaticSpinnerOnPlayer(On.Celeste.CrystalStaticSpinner.orig_OnPlayer orig, CrystalStaticSpinner self, Player player) {
            if(GetOptionBool(Option.AlwaysExplodeSpinners)) {
                self.Destroy();

                return;
            }

            orig(self, player);
        }

        private void modPlayerCtor(On.Celeste.Player.orig_ctor orig, Player self, Vector2 position, PlayerSpriteMode spriteMode) {
            orig(self, position, spriteMode);

            GooberFlingBird.CustomStateId = self.StateMachine.AddState("GooberFlingBird", new Func<int>(GooberFlingBird.CustomStateUpdate), null, null, null);
            // self.Add(new GooberPlayerExtensions());
            
            if(self.level?.Tracker.GetComponent<GooberPlayerExtensions>() == null) {
                self.Add(new GooberPlayerExtensions());
            }
        }

        private bool modPlayerWallJumpCheck(On.Celeste.Player.orig_WallJumpCheck orig, Player self, int dir) {
            if(self.CollideCheck<Water>() && GetOptionBool(Option.CustomSwimming)) {
                return false;
            }

            return orig(self, dir);
        }

        private void modifyPlayerWallJumpCheck(ILContext il) {
            var cursor = new ILCursor(il);

            //approach based on hitbox width extension. it doesnt work
            //method #1: extension towards the wall
            //the player will be unable to collide with sideways jumpthroughs because they call 
            //Entity.CollideFirstOutside<T>(Vector2 position), a method that requires the unshifted
            //player hitbox to NOT be in the entity of collision. extending the hitbox would force
            //it to always return false if the player is close enough to the sideways jumpthrough
            //method #2: extension away from the wall
            //this would have the same problem. if you extend the player hitbox such that it grows
            //away from the wall, the same thing will happen as method #1 except it will collide
            //with an entity on the other side of the wall check. imagine a scenario where the
            //player has a really high horizontal speed but is trapped in a 2 tile wide hole with
            //a sideways jumpthrough that they're supposed to cornerboost off of. i know this is
            //extemely unlikely, but i dont want to have that edge case.
            //ill take my scuffed inefficient solution that actually works

            // int extension = 0;

            // if(cursor.TryGotoNextBestFit(MoveType.After,
            //     instr => instr.MatchLdarg0(), //stealing this
            //     instr => instr.MatchLdarg1(),
            //     instr => instr.MatchCallOrCallvirt<Player>("ClimbBoundsCheck")
            // )) {
            //     cursor.GotoPrev(MoveType.After, instr => instr.MatchLdarg0());

            //     cursor.EmitLdloc0();
            //     cursor.EmitLdarg1();
            //     cursor.EmitDelegate((Player player, int originalDistance, int dir) => {
            //         extension = 0;

            //         if(GetOptionBool(Option.CornerboostBlocksEverywhere)) {
            //             extension = Math.Max(originalDistance, (int)Math.Ceiling(Math.Abs(player.Speed.X) * Engine.DeltaTime) + 1) - originalDistance;

            //             player.Collider.Width += extension;
            //             if(dir == -1) player.Collider.Position.X -= extension;
            //         }
            //     });

            //     cursor.EmitLdarg0(); //giving it back
            // }

            // while(cursor.TryGotoNext(MoveType.Before, instr => instr.MatchRet())) {
            //     cursor.EmitLdarg0();
            //     cursor.EmitLdarg1();
            //     cursor.EmitDelegate((Player player, int dir) => {
            //         player.Collider.Width -= extension;

            //         if(dir == -1) player.Collider.Position.X += extension;
            //     });

            //     cursor.Index++;
            // }

            if(cursor.TryGotoNextBestFit(MoveType.Before,
                instr => instr.MatchLdarg0(),
                instr => instr.MatchLdarg1(),
                instr => instr.MatchCallOrCallvirt<Player>("ClimbBoundsCheck")
            )) {
                cursor.MoveAfterLabels();

                cursor.EmitLdarg0();
                cursor.EmitLdloc0();
                cursor.EmitDelegate((Player player, int originalDistance) => {
                    return GetOptionBool(Option.CornerboostBlocksEverywhere)
                        ? Math.Max(originalDistance, (int)Math.Ceiling(Math.Abs(player.Speed.X) * Engine.DeltaTime) + 1)
                        : originalDistance;
                });
                cursor.EmitStloc0();
            }

            //this is going to be the worst thing ever
            //i am so sorry

            //okay so essentially what im doing is:
            //if the current collision distance returned false, assume that its overshooting it and subtract the player hitbox width from the collision distance
            //as long as the new collision distance is greater than zero, return back to the start of the evaluation
            //this should work with custom entities such as maddiehelpinghand sideways jumpthrus

            ILLabel startLabel = cursor.DefineLabel();
            ILLabel endLabel = cursor.DefineLabel();

            if(cursor.TryGotoNext(MoveType.Before,
                instr => instr.MatchLdarg0(),
                instr => instr.MatchLdfld<Player>("level")
            )) {
                cursor.MarkLabel(startLabel);
            }

            cursor.TryGotoNext(MoveType.Before, instr => instr.MatchBrtrue(out endLabel));


            //im not gonna bother making these instructions not run without the gooberhelper option enabled
            //nothing here is super expensive
            //its probably fine
            //surely
            //honestly the gooberhelper option check might even be more expensive than this
            if(cursor.TryGotoNext(MoveType.Before, instr => instr.MatchRet())) {
                cursor.EmitBrfalse(endLabel);
                cursor.EmitLdcI4(1);
            }

            if(cursor.TryGotoNext(MoveType.Before, instr => instr.MatchLdcI4(0), instr => instr.MatchRet())) {
                cursor.Index++;
                cursor.EmitLdloc0();
                cursor.EmitLdcI4(8);
                cursor.EmitSub();
                cursor.EmitStloc0();
                cursor.EmitLdloc0();
                //this should be a bgt but theres already a zero beneath loc0 on the evaluation stack
                //the logic is just inverted
                cursor.EmitBle(startLabel);
                cursor.EmitLdcI4(0);
            }
        }

        private void modPlayerSwimBegin(On.Celeste.Player.orig_SwimBegin orig, Player self) {
            orig(self);
            
            if(self.Speed.Y > 0 && GetOptionBool(Option.CustomSwimming)) {
                self.Speed.Y *= 2f;

                GooberPlayerExtensions.Instance.AwesomeRetentionSpeed = Vector2.Zero;
            }
        }

        private void modifyPlayerDashCoroutine(ILContext il) {
            var cursor = new ILCursor(il);

            Vector2 originalDashSpeed = Vector2.Zero;

            if(cursor.TryGotoNextBestFit(MoveType.After, 
                instr => instr.MatchLdloc2(),
                instr => instr.MatchLdcR4(240),
                instr => instr.MatchCall<Vector2>("op_Multiply"),
                instr => instr.MatchStloc3()
            )) {
                cursor.EmitLdloc3();
                cursor.EmitDelegate((Vector2 value) => {
                    originalDashSpeed = value * Vector2.One; // * Vector2.One to copy it
                });
            }

            if(cursor.TryGotoNextBestFit(MoveType.After, 
                instr => instr.MatchLdloc1(),
                instr => instr.MatchLdloc3(),
                instr => instr.MatchStfld<Player>("Speed")
            )) {
                cursor.Index--;

                cursor.EmitLdloc1();
                cursor.EmitDelegate((Vector2 speed, Player player) => {
                    Vector2 newSpeed = speed * Vector2.One;
                    var c = GooberPlayerExtensions.Instance;
                    var originalSpeed = c.BeforeDashSpeedConserved;

                    if(
                        GetOptionValue(Option.MagnitudeBasedDashSpeed) == (int)MagnitudeBasedDashSpeedValue.All ||
                        (
                            GetOptionValue(Option.MagnitudeBasedDashSpeed) == (int)MagnitudeBasedDashSpeedValue.OnlyCardinal &&
                            Vector2.Dot(originalDashSpeed, Vector2.UnitX) % 1 == 0 //the second part just checks if the vector is cardinal. do you like my commenting?
                        )
                    ) {
                        return originalDashSpeed.SafeNormalize() * Math.Max(originalSpeed.Length(), originalDashSpeed.Length());
                    }

                    if(GetOptionBool(Option.VerticalDashSpeedPreservation) && (Math.Sign(originalSpeed.Y) == Math.Sign(speed.Y) || GetOptionBool(Option.ReverseDashSpeedPreservation)) && Math.Abs(originalSpeed.Y) > Math.Abs(speed.Y)) {
                        newSpeed.Y = Math.Abs(originalSpeed.Y) * Math.Sign(originalDashSpeed.Y);
                    }

                    if(GetOptionBool(Option.ReverseDashSpeedPreservation)) {
                        if(Math.Sign(originalSpeed.X) == -Math.Sign(speed.X) && Math.Abs(originalSpeed.X) > Math.Abs(speed.X)) {
                            newSpeed.X = -originalSpeed.X;
                        }
                    }

                    if(c.DashWindBoost != Vector2.Zero) {
                        newSpeed.X += c.DashWindBoost.X;
                        
                        if(!player.onGround)
                            newSpeed.Y += c.DashWindBoost.Y;

                        c.DashWindBoost = Vector2.Zero;
                    }

                    return newSpeed;
                });
            }

            //remove the 0.75x speed multiplier when dashing while in contact with water
            if(cursor.TryGotoNextBestFit(MoveType.After, 
                instr => instr.MatchLdloc1(),
                instr => instr.MatchCallOrCallvirt<Entity>("CollideCheck"), //collidecheck<water>
                instr => instr.MatchBrfalse(out ILLabel buh)
            )) {
                cursor.Index--;

                cursor.EmitDelegate(() => {
                    return !GetOptionBool(Option.CustomSwimming);
                });
                cursor.EmitAnd();
            }

            // if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchStloc(3))) {
            //     cursor.EmitDelegate(() => {
            //         if(GetOptionBool(Option.ReverseDashSpeedPreservation)) {
            //             Player player = Engine.Scene.Tracker.GetEntity<Player>();

            //             Vector2 vector = lastAim");
            //             if (player.OverrideDashDirection != null)
            //             {
            //                 vector = player.OverrideDashDirection.Value;
            //             }
            //             vector = DynamicData.For(player).Invoke<Vector2>("CorrectDashPrecision", vector);

            //             if(vector.X != 0) {
            //                 Vector2 beforeDashSpeed = beforeDashSpeed");
            //                 beforeDashSpeed.X = Math.Sign(vector.X) * Math.Abs(beforeDashSpeed.X);
            //                 beforeDashSpeed", beforeDashSpeed);
            //             }
            //         }
            //     });
            // }

            Func<float, Player, float> makeVerticalDashesNotResetSpeed(DashesDontResetSpeedValue minimum) {
                return (value, player) => {
                    return (
                        (GetOptionBool(Option.CustomSwimming) && player.CollideCheck<Water>()) ||
                        GetOptionValue(Option.DashesDontResetSpeed) >= (int)minimum
                    ) ? float.MinValue : value;
                };
            };

            if(cursor.TryGotoNextBestFit(MoveType.After, 
                instr => instr.MatchLdflda<Player>("DashDir"),
                instr => instr.MatchLdfld<Vector2>("Y"),
                instr => instr.MatchLdcR4(0),
                instr => instr.MatchBgtUn(out _)
            )) {
                cursor.Index--;

                cursor.EmitLdloc1();
                cursor.EmitDelegate(makeVerticalDashesNotResetSpeed(DashesDontResetSpeedValue.Legacy));
            }

            if(cursor.TryGotoNextBestFit(MoveType.After, 
                instr => instr.MatchLdflda<Player>("Speed"),
                instr => instr.MatchLdfld<Vector2>("Y"),
                instr => instr.MatchLdcR4(0),
                instr => instr.MatchBgeUn(out _)
            )) {
                cursor.Index--;

                cursor.EmitLdloc1();
                cursor.EmitDelegate(makeVerticalDashesNotResetSpeed(DashesDontResetSpeedValue.On));
            }
        }

        private bool tryCustomSwimmingWalljump(Player self, Vector2 vector) {
            GooberPlayerExtensions c = GooberPlayerExtensions.Instance;

            float redirectSpeed = Math.Max(self.Speed.Length(), c.AwesomeRetentionSpeed.Length()) + 20;

            if(c.AwesomeRetentionTimer <= 0 || !c.AwesomeRetentionWasInWater) {
                return false;
            }

            Vector2 redirectDirection = -c.AwesomeRetentionDirection;

            if(redirectDirection != Vector2.Zero && redirectSpeed != 0) {
                Input.Jump.ConsumeBuffer();
                self.Speed = redirectDirection.SafeNormalize() * redirectSpeed;

                int index = c.AwesomeRetentionPlatform.GetWallSoundIndex(self, Math.Sign(c.AwesomeRetentionDirection.X));

                Dust.Burst(self.Center - redirectDirection * self.Collider.Size / 2, redirectDirection.Angle(), 4, self.DustParticleFromSurfaceIndex(index));
                self.Play(SurfaceIndex.GetPathFromIndex(index) + "/landing", "surface_index", index);
            }

            return true;
        }

        private bool doCustomSwimMovement(Player self, Vector2 vector) {
            float defaultSpeed = 90f;

            if(Vector2.Dot(self.Speed.SafeNormalize(Vector2.Zero), vector) < -0.5f || vector.Length() == 0 || self.Speed.Length() <= 90) {
                self.Speed = Calc.Approach(self.Speed, defaultSpeed * vector, 350f * Engine.DeltaTime);
            } else {
                self.Speed = self.Speed.RotateTowards(vector.Angle(), 10f * Engine.DeltaTime);
            }

            //awesome retention is used while underwater
            //i need to think of a better name for that
            self.wallSpeedRetentionTimer = 0f;

            if(Input.Jump.Pressed) {
                float distance = Math.Min(self.Speed.Y * Engine.DeltaTime, 0) - 10f;

                if(!self.CollideCheck<Water>(self.Position + Vector2.UnitY * distance)) {
                    if (self.Speed.Y >= 0) {
                        // self.Jump(true, true);

                        return true;
                    }

                    if(self.Speed.Y < -130f) {
                        Input.Jump.ConsumeBuffer();
                        self.Speed += 80f * self.Speed.SafeNormalize(Vector2.Zero);
                        
                        self.launched = true;
                        Dust.Burst(self.Position, (vector * -1f).Angle(), 4);

                        self.Play(SFX.char_mad_jump_super);
                    }
                }

                tryCustomSwimmingWalljump(self, vector);
            }

            return false;
        }

        private void modifyPlayerSwimUpdate(ILContext il) {
            var cursor = new ILCursor(il);

            ILLabel originalMovementEndLabel = cursor.DefineLabel();
            ILLabel customMovementEndLabel = cursor.DefineLabel();
            ILLabel swimJumpEndLabel = cursor.DefineLabel();
            ILLabel afterReturnLabel = cursor.DefineLabel();

            void logError(string location) {
                Logger.Error("GooberHelper", $"Failed to find il while making custom swimming work ({location})");
            }

            if(!cursor.TryGotoNextBestFit(MoveType.After, 
                instr => instr.MatchLdloc1(),
                instr => instr.MatchCallOrCallvirt(((Func<Vector2, Vector2>)Calc.SafeNormalize).Method),
                instr => instr.MatchStloc1()
            )) {
                logError("SafeNormalize");

                return;
            }

            ILCursor afterSafeNormalizeCursor = cursor.Clone();

            if(!cursor.TryGotoNextBestFit(MoveType.Before, 
                instr => instr.MatchLdloc0(),
                instr => instr.MatchBrtrue(out _),
                instr => instr.MatchLdarg0(),
                instr => instr.MatchLdfld<Player>("moveX")
            )) {
                logError("moveX");

                return;
            }

            ILCursor beforeMoveXCursor = cursor.Clone();

            if(!cursor.TryGotoNextBestFit(MoveType.Before,
                instr => instr.MatchLdsfld(typeof(Input).GetField("Jump")),
                instr => instr.MatchCallOrCallvirt<VirtualButton>("get_Pressed"),
                instr => instr.MatchBrfalse(out swimJumpEndLabel)
            )) {
                logError("Jump.Pressed");

                return;
            }

            ILCursor beforeJumpPressedCursor = cursor.Clone();

            cursor = afterSafeNormalizeCursor;
                //if(GetOptionBool(Option.CustomSwimming))
                cursor.EmitDelegate(() => {
                    return GetOptionBool(Option.CustomSwimming);
                });
                cursor.EmitBrfalse(customMovementEndLabel);

                //if(doCustomSwimMovement(this, vector))
                cursor.EmitLdarg0();
                cursor.EmitLdloc1();
                cursor.EmitDelegate(doCustomSwimMovement);
                cursor.EmitBrfalse(afterReturnLabel);
                
                //return 0;
                cursor.EmitLdcI4(0);
                cursor.EmitRet();

                cursor.MarkLabel(afterReturnLabel);

                cursor.EmitBr(originalMovementEndLabel);

                cursor.MarkLabel(customMovementEndLabel);


            cursor = beforeMoveXCursor;
                cursor.MarkLabel(originalMovementEndLabel);

            
            cursor = beforeJumpPressedCursor;
                cursor.MoveAfterLabels();
                cursor.EmitDelegate(() => {
                    return GetOptionBool(Option.CustomSwimming);
                });
                cursor.EmitBrtrue(swimJumpEndLabel);
        }

        private void modLevelLevelLoad(On.Celeste.Level.orig_LoadLevel orig, Level level, Player.IntroTypes playerIntro, bool isFromLoader) {
            if(level.Tracker.GetEntity<GooberIconThing>() == null)
                level.Add(new GooberIconThing());

            if(level.Tracker.GetEntity<GooberSettingsList>() == null)
                level.Add(new GooberSettingsList());

            orig(level, playerIntro, isFromLoader);

            if(level.Tracker.GetComponent<GooberPlayerExtensions>() == null)
                level.Tracker.GetEntity<Player>()?.Add(new GooberPlayerExtensions());
        }

        private void modifyBounceBlockWindUpPlayerCheck(ILContext il) {
            var cursor = new ILCursor(il);

            for(int i = 0; i < 2; i++) {
                if(cursor.TryGotoNextBestFit(MoveType.After, instr => instr.MatchCallOrCallvirt<StateMachine>("get_State"))) {
                    cursor.EmitDelegate((int orig) => 
                        GetOptionBool(Option.CoreBlockAllDirectionActivation) ? 1 : orig
                    );
                }
            }
        }

        private void modPlayerAttractBegin(On.Celeste.Player.orig_AttractBegin orig, Player self) {
            GooberPlayerExtensions.Instance.AttractSpeedPreserved = self.Speed;

            orig(self);
        }

        private void modPlayerFinalBossPushLaunch(On.Celeste.Player.orig_FinalBossPushLaunch orig, Player self, int dir) {
            orig(self, dir);

            if(GetOptionBool(Option.BadelineBossSpeedPreservation)) {
                self.Speed.X = dir * Math.Max(Math.Abs(self.Speed.X), GooberPlayerExtensions.Instance.AttractSpeedPreserved.Length());
            }
        }

        private void modifyPlayerExplodeLaunch(ILContext il) {
            var cursor = new ILCursor(il);

            var originalSpeed = default(Vector2);

            cursor.EmitLdarg0();
            cursor.EmitDelegate((Player player) => {
                originalSpeed = player.GetConservedSpeed();
            });

            //right before it does the logic for bumper boosting
            if(cursor.TryGotoNextBestFit(MoveType.AfterLabel,
                instr => instr.MatchLdarg0(),
                instr => instr.MatchLdflda<Player>("Speed"),
                instr => instr.MatchLdfld<Vector2>("X"),
                instr => instr.MatchLdcR4(0)
            )) {
                cursor.EmitLdarg0();
                cursor.EmitDelegate((Player player) => {
                    var explodeLaunchSpeedPreservationValue = (ExplodeLaunchSpeedPreservationValue)GetOptionValue(Option.ExplodeLaunchSpeedPreservation);
                    
                    if(explodeLaunchSpeedPreservationValue == ExplodeLaunchSpeedPreservationValue.None)
                        return;

                    var componentMax = player.Speed.Sign() * Vector2.Max(player.Speed.Abs(), originalSpeed.Abs());

                    object _ = explodeLaunchSpeedPreservationValue switch {
                        ExplodeLaunchSpeedPreservationValue.Horizontal => player.Speed.X = componentMax.X,
                        ExplodeLaunchSpeedPreservationValue.Vertical => player.Speed.Y = componentMax.Y,
                        ExplodeLaunchSpeedPreservationValue.Both => player.Speed = componentMax,
                        ExplodeLaunchSpeedPreservationValue.Magnitude => player.Speed = player.Speed.SafeNormalize() * Math.Max(originalSpeed.Length(), player.Speed.Length()),
                        _ => null
                    };

                    //i need to leave this in
                    //even in the rewrite
                    if(player.level.Session.Area.SID == "alex21/Dashless+/1A Dashless but Spikier" && player.level.Session.Level == "b-06") {
                        player.Speed.X = 0;
                        player.Speed.Y = -330;
                    }
                });
            }
        }

        private void modifyPlayerOnCollideH(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNextBestFit(MoveType.After,
                instr => instr.MatchDup(),
                instr => instr.MatchLdindR4(),
                instr => instr.MatchLdcR4(-0.5f)
            )) {
                cursor.EmitDelegate((float value) => {
                    return GetOptionBool(Option.CustomFeathers) ? -1f : value;
                });
            }

            if(cursor.TryGotoNextBestFit(MoveType.After, instr => instr.MatchLdcR4(0.06f))) {
                cursor.EmitDelegate((float value) => {
                    float newTime = GetOptionValue(Option.RetentionLength);

                    return newTime != 4 ? newTime / 60f : value;
                });
            }
        }
        
        private void modifyPlayerOnCollideV(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNextBestFit(MoveType.After,
                instr => instr.MatchDup(),
                instr => instr.MatchLdindR4(),
                instr => instr.MatchLdcR4(-0.5f)
            )) {
                cursor.EmitDelegate((float value) => {
                    return GetOptionBool(Option.CustomFeathers) ? -1f : value;
                });
            }
        }

        private void modPlayerStarFlyBegin(On.Celeste.Player.orig_StarFlyBegin orig, Player self) {
            GooberPlayerExtensions.Instance.StarFlySpeedPreserved = self.Speed;

            orig(self);
        }

        private void modifyPlayerStarFlyUpdate(ILContext il) {
            var cursor = new ILCursor(il);

            int start = cursor.Index;

            float lowMult = 0.65f;
            float midMult = 0.90f;
            float highMult = 1.05f;

            //destroyer of reality
            // cursor.EmitDelegate(() => {
            //     Player player = Engine.Scene.Tracker.GetEntity<Player>();

            //     (Engine.Scene as Level).Displacement.AddBurst(player.Center, 2f, 8f, 1000f, 1f, null, null);
            // });

            float[] matches = [91, 140, 190, 140, 140, 140];
            float[] replaceMults = [lowMult, midMult, highMult, midMult, midMult, 0.75f];

            for(int i = 0; i < matches.Length; i++) {
                //fsr i have to put this in a variable instead of just accessing the array from within the delegate. that took way longer to figure out than it shouldve
                float replaceMult = replaceMults[i];

                if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(matches[i]))) {
                    cursor.EmitDelegate((float value) => {
                        return GetOptionBool(Option.CustomFeathers)
                            ? Math.Max(Engine.Scene.Tracker.GetComponent<GooberPlayerExtensions>().StarFlySpeedPreserved.Length() * replaceMult, value)
                            : value;
                    });
                }
            }

            cursor.Index = start;

            if(cursor.TryGotoNextBestFit(MoveType.After,
                instr => instr.MatchLdfld<Player>("starFlyTimer"),
                instr => instr.MatchLdcR4(0f),
                instr => instr.OpCode == OpCodes.Bgt_Un
            )) {
                ILLabel label = cursor.DefineLabel();
                int index = cursor.Index;

                if(cursor.TryGotoNextBestFit(MoveType.Before, 
                    instr => instr.MatchLdcI4(1),
                    instr => instr.MatchLdcI4(1),
                    instr => instr.MatchCall(typeof(Input).GetMethod("Rumble"))
                )) {

                    cursor.MarkLabel(label);
                    cursor.Index = index;

                    cursor.EmitLdarg0();
                    cursor.EmitDelegate((Player player) => {
                        if(GetOptionBool(Option.FeatherEndSpeedPreservation)) {
                            //free feather end boosts
                            if(player.Speed.Y <= 0f) {
                                player.varJumpSpeed = player.Speed.Y;
                                player.AutoJump = true;
                                player.AutoJumpTimer = 0f;
                                player.varJumpTimer = 0.2f;
                            }

                            return true;
                        }

                        return false;
                    });
                    cursor.EmitBrtrue(label);
                }
            }
        }

        private void modifyPlayerStarFlyCoroutine(ILContext il) {
            var cursor = new ILCursor(il);

            var startPosition = cursor.Index;

            if(cursor.TryGotoNextBestFit(MoveType.After,
                instr => instr.MatchLdarg0(),
                instr => instr.MatchLdcI4(-1),
                instr => instr.OpCode == OpCodes.Stfld
            )) {
                startPosition = cursor.Index;
            }

            if(
                cursor.TryGotoNext(MoveType.After, 
                    instr => instr.MatchLdcR4(0.1f)
                ) &&
                cursor.TryGotoNext(MoveType.After, 
                    instr => instr.MatchLdcI4(1),
                    instr => instr.MatchRet()
                )
            ) {
                var afterStarFlyStartLabel = cursor.MarkLabel();

                cursor.Index = startPosition;
                cursor.EmitLdloc1();
                cursor.EmitDelegate((Player player) => {
                    if(GetOptionValue(Option.CustomFeathers) == (int)CustomFeathersValue.SkipIntro) {
                        player.Sprite.Play("starFly", false, false);

                        return true;
                    }

                    return false;
                });
                cursor.Emit(OpCodes.Brtrue_S, afterStarFlyStartLabel);
            }
            
            if(cursor.TryGotoNextBestFit(MoveType.After,
                instr => instr.MatchLdcR4(250),
                instr => instr.OpCode == OpCodes.Call,
                instr => instr.OpCode == OpCodes.Stfld
            )) {
                cursor.EmitLdloc1();
                cursor.EmitDelegate((Player player) => {
                    if(GetOptionBool(Option.CustomFeathers)) {
                        var direction = GetOptionValue(Option.CustomFeathers) == (int)CustomFeathersValue.KeepIntro
                            ? player.Speed
                            : GooberPlayerExtensions.Instance.StarFlySpeedPreserved;

                        player.Speed = direction.SafeNormalize() * Math.Max(GooberPlayerExtensions.Instance.StarFlySpeedPreserved.Length(), 250);
                    }
                });
            }
        }

        private void modPlayerClimbJump(On.Celeste.Player.orig_ClimbJump orig, Player self) {
            var beforeJumpCount = SaveData.Instance.TotalJumps;

            if(self.wallSpeedRetentionTimer > 0f && GetOptionBool(Option.GetClimbjumpSpeedInRetention))
                self.Speed.X = self.wallSpeedRetained;

            orig(self);

            if(self.wallSpeedRetentionTimer > 0f && GetOptionBool(Option.GetClimbjumpSpeedInRetention))
                self.wallSpeedRetained = self.Speed.X;

            //the method didnt run; dont do anything else
            if(beforeJumpCount == SaveData.Instance.TotalJumps) return;

            if(GetOptionBool(Option.WallboostDirectionIsOppositeSpeed) && Input.MoveX == 0)
                self.wallBoostDir = Math.Sign(-self.Speed.X);
        }

        private void modPlayerWallJump(On.Celeste.Player.orig_WallJump orig, Player self, int dir) {
            // Vector2 originalSpeed = self.Speed;

            // if(self.wallSpeedRetentionTimer > 0)
            //     originalSpeed.X = self.wallSpeedRetained;


            // GooberPlayerExtensions.Instance.WallJumpSpeedPreserved = originalSpeed;

            // int beforeJumpCount = SaveData.Instance.TotalWallJumps;

            orig(self, dir);
            
            // //the method didnt run; dont do anything else
            // if(beforeJumpCount == SaveData.Instance.TotalWallJumps) return;

            // if(GetOptionBool(Option.SwapHorizontalAndVerticalSpeedOnWalljump)) {
            //     self.Speed.X = Math.Max(Math.Abs(originalSpeed.Y), Math.Abs(self.Speed.X)) * Math.Sign(self.Speed.X);
            //     self.Speed.Y = -Math.Max(Math.Max(Math.Abs(originalSpeed.X), Math.Abs(self.wallSpeedRetained) * (self.wallSpeedRetentionTimer > 0f ? 1f : 0f)), Math.Abs(self.Speed.Y));
            //     self.varJumpSpeed = self.Speed.Y;

            //     return;
            // }

            // handleVerticalJumpSpeed(self, originalSpeed);

            // WalljumpSpeedPreservationValue wallJumpSpeedPreservationValue = (WalljumpSpeedPreservationValue)GetOptionValue(Option.WalljumpSpeedPreservation);

            // if(wallJumpSpeedPreservationValue == WalljumpSpeedPreservationValue.Invert) {
            //     self.Speed.X = Math.Sign(self.Speed.X) * Math.Max(
            //         Math.Abs(self.Speed.X),
            //         Math.Abs(originalSpeed.X) + self.LiftBoost.X
            //     );
            // } else if(Math.Sign(self.Speed.X) == Math.Sign(originalSpeed.X) && wallJumpSpeedPreservationValue != WalljumpSpeedPreservationValue.None) { //fakercb or preserve
            //     self.Speed.X = Math.Sign(originalSpeed.X) * Math.Max(
            //         Math.Abs(self.Speed.X),
            //         Math.Abs(originalSpeed.X) - (self.moveX == 0 || wallJumpSpeedPreservationValue == WalljumpSpeedPreservationValue.Preserve ? 0f : 40f) + self.LiftBoost.X
            //     );
            // }
        }

        private void modifyPlayerWallJump(ILContext il) {
            var cursor = new ILCursor(il);

            var originalSpeed = default(Vector2);

            cursor.EmitLdarg0();
            cursor.EmitDelegate((Player player) => {
                originalSpeed = player.GetConservedSpeed();
            });

            if(cursor.TryGotoNext(MoveType.AfterLabel,
                instr => instr.MatchLdarg0(),
                instr => instr.MatchLdfld<Player>("Speed"),
                instr => instr.MatchLdarg0(),
                instr => instr.MatchCallOrCallvirt<Player>("get_LiftBoost")
            )) {
                cursor.EmitLdarg0();
                cursor.EmitLdarg1();
                cursor.EmitDelegate((Player player, int dir) => {
                    //x speed stuff
                    var wallJumpSpeedPreservationValue = (WalljumpSpeedPreservationValue)GetOptionValue(Option.WalljumpSpeedPreservation);

                    var wallJumpSpeedPreservationResult = 0f;
                    var swapHorizontalAndVerticalSpeedOnWalljumpResult = 0f;

                    if(wallJumpSpeedPreservationValue != WalljumpSpeedPreservationValue.None) {
                        wallJumpSpeedPreservationResult = wallJumpSpeedPreservationValue switch {
                            WalljumpSpeedPreservationValue.Invert => Math.Abs(originalSpeed.X),
                            WalljumpSpeedPreservationValue.None => 0f,
                            
                            //FakeRCB and Preserve (theyre practically the same)
                            _ => Math.Sign(originalSpeed.X) == dir ? Math.Abs(originalSpeed.X) : 0f,
                        };

                        if(wallJumpSpeedPreservationValue == WalljumpSpeedPreservationValue.FakeRCB && player.moveX != 0)
                            wallJumpSpeedPreservationResult -= 40f;
                    }

                    if(GetOptionBool(Option.SwapHorizontalAndVerticalSpeedOnWalljump))
                        swapHorizontalAndVerticalSpeedOnWalljumpResult = Math.Abs(originalSpeed.Y);
                    
                    player.Speed.X = Utils.SignedAbsMax(
                        player.Speed.X,
                        wallJumpSpeedPreservationResult,
                        swapHorizontalAndVerticalSpeedOnWalljumpResult
                    );

                    //y speed stuff
                    handleVerticalJumpSpeed(player, originalSpeed);
                });
            }
        }

        private int modPlayerDreamDashUpdate(On.Celeste.Player.orig_DreamDashUpdate orig, Player self) {
            if(GetOptionBool(Option.DreamBlockSpeedPreservation)) {
                var correctSpeed = GooberPlayerExtensions.Instance.PreservedDreamBlockSpeedMagnitude;

                if(self.Speed.X == -correctSpeed.X && Math.Abs(self.Speed.X) > 0) correctSpeed.X *= -1; else 
                if(self.Speed.Y == -correctSpeed.Y && Math.Abs(self.Speed.Y) > 0) correctSpeed.Y *= -1; else
                {
                    var dreamBlockType = self.dreamBlock.GetType().Name;
                    var data = DynamicData.For(self.dreamBlock);

                    //i know this is evil but also putting code to update the player speed to anything constant is evil too so it cancels out and its fine
                    if(dreamBlockType == "ConnectedDreamBlock" && data.Get<bool>("FeatherMode")) {
                        self.Speed = self.Speed.SafeNormalize() * correctSpeed.Length();
                    } else {
                        self.Speed = correctSpeed;
                    }
                }
            }

            return orig(self);
        }

        private void modPlayerDreamDashBegin(On.Celeste.Player.orig_DreamDashBegin orig, Player self) {
            Vector2 originalSpeed = self.GetConservedSpeed();

            orig(self);

            var optionValue = (DreamBlockSpeedPreservationValue)GetOptionValue(Option.DreamBlockSpeedPreservation);

            if(optionValue != DreamBlockSpeedPreservationValue.None) {
                var componentMax = self.Speed.Sign() * Vector2.Max(self.Speed.Abs(), originalSpeed.Abs());

                object _ = optionValue switch {
                    DreamBlockSpeedPreservationValue.Horizontal => self.Speed.X = componentMax.X,
                    DreamBlockSpeedPreservationValue.Vertical => self.Speed.Y = componentMax.Y,
                    DreamBlockSpeedPreservationValue.Both => self.Speed = componentMax,
                    DreamBlockSpeedPreservationValue.Magnitude => self.Speed = self.Speed.SafeNormalize() * Math.Max(originalSpeed.Length(), self.Speed.Length()),
                    _ => null
                };
                
                GooberPlayerExtensions.Instance.PreservedDreamBlockSpeedMagnitude = self.Speed;
            }
        }

        private void modifyPlayerSuperWallJump(ILContext il) {
            var cursor = new ILCursor(il);

            var originalSpeed = default(Vector2);

            cursor.EmitLdarg0();
            cursor.EmitDelegate((Player player) => {
                originalSpeed = player.GetConservedSpeed();
            });

            if(cursor.TryGotoNext(MoveType.AfterLabel,
                instr => instr.MatchLdarg0(),
                instr => instr.MatchLdfld<Player>("Speed"),
                instr => instr.MatchLdarg0(),
                instr => instr.MatchCallOrCallvirt<Player>("get_LiftBoost")
            )) {
                cursor.EmitLdarg0();
                cursor.EmitLdarg1();
                cursor.EmitDelegate((Player player, int dir) => {
                    var wallbounceSpeedPreservationResult = 0f;

                    if(GetOptionBool(Option.WallbounceSpeedPreservation)) {
                        var c = GooberPlayerExtensions.Instance;
                        
                        wallbounceSpeedPreservationResult = Math.Max(
                            Math.Abs(player.beforeDashSpeed.X),
                            c.DashStickyRetentionExists ? Math.Abs(c.DashStickyRetentionSpeed.X) : 0f
                        );
                    }

                    player.Speed.X = Math.Sign(player.Speed.X) * Math.Max(wallbounceSpeedPreservationResult, Math.Abs(player.Speed.X));

                    handleVerticalJumpSpeed(player, originalSpeed);
                });
            }
        }

        private void modCelesteFreeze(On.Celeste.Celeste.orig_Freeze orig, float time) {
            float newTime = GetOptionValue(Option.RefillFreezeLength);
            
            //as long as all refill freeze freezeframe callers have "refillroutine" in their names and nothing else then this should work
            if(refillRoutineRegex.IsMatch(new System.Diagnostics.StackTrace().ToString())) {
                if(newTime != 3f) time = newTime / 60f;

                if(GetOptionBool(Option.RefillFreezeGameSuspension)) {
                    GooberPlayerExtensions c = GooberPlayerExtensions.Instance;

                    c.FreezeFrameFrozen = true;
                    c.FreezeFrameFrozenInputs = new Utils.InputState();

                    return;
                }
            }

            orig(time);
        }

        private void modPlayerPointBounce(On.Celeste.Player.orig_PointBounce orig, Player self, Vector2 from) {
            if(!GetOptionBool(Option.ReboundSpeedPreservation)) {
                orig(self, from);

                return;
            }

            var originalSpeed = self.GetConservedSpeed();

            orig(self, from);

            self.Speed = self.Speed.SafeNormalize() * Math.Max(originalSpeed.Length(), self.Speed.Length());
        }

        private void modPlayerRebound(On.Celeste.Player.orig_Rebound orig, Player self, int direction = 0) {
            if(!GetOptionBool(Option.ReboundSpeedPreservation)) {
                orig(self, direction);

                return;
            }

            var originalSpeed = self.GetConservedSpeed();

            orig(self, direction);

            var sign = Utils.FirstSign(self.Speed.X, self.moveX, originalSpeed.X);
                        
            self.Speed.X = sign * Utils.UnsignedAbsMax(
                self.Speed.X,
                originalSpeed.X
            );
        }

        private void modPlayerReflectBounce(On.Celeste.Player.orig_ReflectBounce orig, Player self, Vector2 direction) {
            if(!GetOptionBool(Option.ReboundSpeedPreservation)) {
                orig(self, direction);
                
                return;
            }

            var originalSpeed = self.GetConservedSpeed();

            orig(self, direction);
            
            var sign = direction.X == 0
                ? Utils.FirstSign(self.moveX, self.Speed.X)
                : Utils.FirstSign(self.Speed.X);

            self.Speed.X = sign * Utils.UnsignedAbsMax(
                self.Speed.X,
                originalSpeed.X
            );
        }

        private bool modPlayerSideBounce(On.Celeste.Player.orig_SideBounce orig, Player self, int dir, float fromX, float fromY) {
            if(!GetOptionBool(Option.SpringSpeedPreservation))
                return orig(self, dir, fromX, fromY);

            var originalSpeed = self.GetConservedSpeed();

            bool res = orig(self, dir, fromX, fromY);
            
            self.Speed.X = Utils.SignedAbsMax(self.Speed.X, originalSpeed.X);

            return res;
        }

        private void modPlayerSuperBounce(On.Celeste.Player.orig_SuperBounce orig, Player self, float fromY) {
            var springSpeedPreservationValue = GetOptionValue(Option.SpringSpeedPreservation);

            if(springSpeedPreservationValue == (int)SpringSpeedPreservationValue.None) {
                orig(self, fromY);

                return;
            }

            var originalSpeed = self.Speed;

            orig(self, fromY);

            self.Speed.X = originalSpeed.X;

            if(springSpeedPreservationValue == (int)SpringSpeedPreservationValue.Invert && self.moveX == -Math.Sign(self.Speed.X))
                self.Speed.X *= -1;
        }

        private void modPlayerDashEnd(On.Celeste.Player.orig_DashEnd orig, Player self) {
            orig(self);

            GooberPlayerExtensions.Instance.DashStickyRetentionExists = false;
        }

        private void modPlayerOnCollideH(On.Celeste.Player.orig_OnCollideH orig, Player self, CollisionData data) {
            if(UseAwesomeRetention) {
                GooberPlayerExtensions c = GooberPlayerExtensions.Instance;

                if(c.AwesomeRetentionTimer <= 0.0f) {
                    c.AwesomeRetentionSpeed.X = self.Speed.X;
                    c.AwesomeRetentionTimer = 0.06f;
                    c.AwesomeRetentionWasInWater = self.CollideCheck<Water>();
                    c.AwesomeRetentionPlatform = data.Hit;

                    c.AwesomeRetentionDirection = new Vector2(data.Direction.X, c.AwesomeRetentionDirection.Y);
                }
            }

            if(self.StateMachine.State == 2) {
                GooberPlayerExtensions c = GooberPlayerExtensions.Instance;
                
                c.DashStickyRetentionExists = true;
                c.DashStickyRetentionDirection = data.Direction;
                c.DashStickyRetentionSpeed = self.Speed;
            }
            
            float originalDashAttack = self.dashAttackTimer;

            orig(self, data);

            if(GetOptionBool(Option.KeepDashAttackOnCollision)) {
                self.dashAttackTimer = originalDashAttack;
            }
        }

        private void modPlayerOnCollideV(On.Celeste.Player.orig_OnCollideV orig, Player self, CollisionData data) {
            if(UseAwesomeRetention) {
                GooberPlayerExtensions c = GooberPlayerExtensions.Instance;

                if(c.AwesomeRetentionTimer <= 0.0f) {
                    c.AwesomeRetentionSpeed.Y = self.Speed.Y;
                    c.AwesomeRetentionTimer = 0.06f;
                    c.AwesomeRetentionWasInWater = self.CollideCheck<Water>();
                    c.AwesomeRetentionPlatform = data.Hit;

                    c.AwesomeRetentionDirection = new Vector2(c.AwesomeRetentionDirection.X, data.Direction.Y);
                }
            }

            if(self.StateMachine.State == 2) {
                GooberPlayerExtensions c = GooberPlayerExtensions.Instance;

                if(!c.DashStickyRetentionExists) {
                    c.DashStickyRetentionExists = true;
                    c.DashStickyRetentionDirection = data.Direction;
                    c.DashStickyRetentionSpeed = self.Speed;
                }
            }

            float originalDashAttack = self.dashAttackTimer;

            orig(self, data);

            if(GetOptionBool(Option.KeepDashAttackOnCollision)) {
                self.dashAttackTimer = originalDashAttack;
            }
        }

        private void modPlayerJump(On.Celeste.Player.orig_Jump orig, Player self, bool particles, bool playSfx) {
            bool isClimbjump = particles == false && playSfx == false;
            Vector2 originalSpeed = self.Speed;
            var jumpInversionValue = (JumpInversionValue)GetOptionValue(Option.JumpInversion);

            if(self.moveX == -Math.Sign(self.Speed.X)) {
                if(
                    jumpInversionValue == JumpInversionValue.All ||
                    !isClimbjump && jumpInversionValue == JumpInversionValue.GroundJumps
                ) {
                    self.Speed.X *= -1;
                }
            }

            if(!isClimbjump) handleVerticalSpeedToHorizontal(self, originalSpeed);

            int beforeJumpCount = SaveData.Instance.TotalJumps;

            orig(self, particles, playSfx);

            //the method didnt run; dont do anything else
            if(beforeJumpCount == SaveData.Instance.TotalJumps) return;

            handleVerticalJumpSpeed(self, originalSpeed);
        }

        private void modPlayerUpdate(On.Celeste.Player.orig_Update orig, Player self) {
            beforeUpdateSpeed = self.Speed;

            if(GetOptionBool(Option.HorizontalTurningSpeedInversion) && Input.MoveX != Math.Sign(self.Speed.X) && Input.MoveX != 0) {
                self.Speed.X *= -1;
            }

            //weird as hell
            if(GetOptionBool(Option.VerticalTurningSpeedInversion) && Input.MoveY != Math.Sign(self.Speed.Y) && Input.MoveY != 0) {
                if(self.varJumpTimer > 0 && self.Speed.Y < 0f) {
                    self.varJumpTimer = 0f;
                }

                self.Speed.Y *= -1;
            }

            if(UseAwesomeRetention) {
                GooberPlayerExtensions c = GooberPlayerExtensions.Instance;

                if(c.AwesomeRetentionTimer > 0) {
                    // Console.WriteLine($"speed: {c.AwesomeRetentionSpeed}, dir: {c.AwesomeRetentionDirection}, time: {c.AwesomeRetentionTimer}");

                    c.AwesomeRetentionTimer -= Engine.DeltaTime;
                } else {
                    c.AwesomeRetentionDirection = Vector2.Zero;
                }
            }

            orig(self);

            if(GetOptionBool(Option.PickupSpeedInversion) && self.StateMachine.State == 8) {
                self.Facing = self.moveX == 0 ? self.Facing : (Facings)self.moveX;
            }
        }

        private void modifyPlayerUpdate(ILContext il) {
            var cursor = new ILCursor(il);

            //both of these variables dont need to be part of the session
            //theyre both assigned and accessed in the same method
            bool upwardsCoyote = false;

            if(cursor.TryGotoNextBestFit(MoveType.Before,
                instr => instr.MatchLdarg0(),
                instr => instr.MatchLdfld<Player>("StateMachine"),
                instr => instr.MatchCallOrCallvirt<StateMachine>("get_State"),
                instr => instr.MatchLdcI4(9),
                instr => instr.MatchBneUn(out _)
            )) {
                cursor.MoveAfterLabels();

                cursor.EmitLdarg0();
                cursor.EmitDelegate((Player player) => {
                    if(!GetOptionBool(Option.AllowUpwardsCoyote) || player.Speed.Y > 0) {
                        upwardsCoyote = false;

                        return;
                    }

                    upwardsCoyote = 
                        player.CollideFirst<Solid>(player.Position + Vector2.UnitY) != null ||
                        (player.CollideCheck<JumpThru>(player.Position + Vector2.UnitY * player.Collider.Height) && player.CollideCheck<JumpThru>(player.Position + Vector2.UnitY));
                });
            }

            float cobwob_originalSpeed = 0;

            //[BEFORE] this.Speed.X = 130f * (float)this.moveX;
            if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(130f))) {
                cursor.EmitLdarg0();
                cursor.EmitDelegate((float orig, Player player) => {
                    if(GetOptionValue(Option.CobwobSpeedInversion) == (int)CobwobSpeedInversionValue.None) return orig;

                    if (player == null) return orig;

                    cobwob_originalSpeed = player.Speed.X;

                    return orig;
                });
            }

            //[BEFORE] this.Stamina += 27.5f;
            if (cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(27.5f))) {
                cursor.EmitLdarg0();
                cursor.EmitDelegate((float orig, Player player) => {
                    if(
                        GetOptionValue(Option.CobwobSpeedInversion) == (int)CobwobSpeedInversionValue.None &&
                        !GetOptionBool(Option.WallboostSpeedIsOppositeSpeed)
                    ) return orig;

                    if (player == null) return orig;

                    float dir = Math.Sign(player.Speed.X);
                    float newAbsoluteSpeed = Math.Max(130f, Math.Abs(cobwob_originalSpeed));

                    if(
                        GetOptionBool(Option.WallboostSpeedIsOppositeSpeed) &&
                        !GetOptionBool(Option.WallboostDirectionIsOppositeSpeed) &&
                        player.wallBoostDir == Math.Sign(cobwob_originalSpeed - 11f * Math.Sign(cobwob_originalSpeed))
                    ) {
                        dir = -Math.Sign(cobwob_originalSpeed);
                    }
                    
                    if(player.wallSpeedRetentionTimer > 0.0 && GetOptionValue(Option.CobwobSpeedInversion) == (int)CobwobSpeedInversionValue.WorkWithRetention) {
                        float retainedSpeed = player.wallSpeedRetained;

                        newAbsoluteSpeed = Math.Max(130f, Math.Abs(retainedSpeed));
                    }

                    player.Speed.X = dir * newAbsoluteSpeed;

                    return orig;
                });
            }

            ILLabel beforeStaminaRefillLabel = cursor.DefineLabel();
            ILLabel beforeCoyoteRefillLabel = cursor.DefineLabel();

            for(int i = 0; i < 2; i++) {
                if(cursor.TryGotoNextBestFit(MoveType.Before,
                    instr => instr.MatchLdarg0(),
                    instr => instr.MatchLdfld<Player>("onGround"),
                    instr => instr.MatchBrfalse(out _)
                )) {
                    cursor.MoveAfterLabels();
                    cursor.EmitDelegate(() => {
                        return upwardsCoyote;
                    });
                    cursor.EmitBrtrue(i == 0 ? beforeStaminaRefillLabel : beforeCoyoteRefillLabel);
                }
                
                if(i == 0) {
                    if(cursor.TryGotoNextBestFit(MoveType.Before,
                        instr => instr.MatchLdarg0(),
                        instr => instr.MatchLdcR4(110),
                        instr => instr.MatchStfld<Player>("Stamina")
                    )) {
                        cursor.MarkLabel(beforeStaminaRefillLabel);
                    }
                } else {
                    if(cursor.TryGotoNextBestFit(MoveType.Before,
                        instr => instr.MatchLdarg0(),
                        instr => instr.MatchLdcR4(0.1f),
                        instr => instr.MatchStfld<Player>("jumpGraceTimer")
                    )) {
                        cursor.MarkLabel(beforeCoyoteRefillLabel);
                    }
                }
            }
        }
    }
}