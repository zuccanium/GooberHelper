using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Extensions;
using Celeste.Mod.GooberHelper.Helpers;
using Celeste.Mod.GooberHelper.Options.GeneralHooks;
using Celeste.Mod.GooberHelper.Settings.Toggles;
using Celeste.Mod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Settings.Root {
    [GooberHelperSetting]
    public class CustomSwimmingAnimation : AbstractToggle {
        public static ParticleType P_SwimTrail;
        public static ParticleType P_SwimLaunchBurst;

        [OnLoadContent]
        public static void LoadContent() {
            P_SwimTrail = new() {
                SourceChooser = new Chooser<MTexture>([
                    GFX.Game["particles/zappysmoke00"],
                    GFX.Game["particles/zappysmoke01"],
                    GFX.Game["particles/zappysmoke02"],
                    GFX.Game["particles/zappysmoke03"]
                ]),
                LifeMin = 0.3f,
                LifeMax = 0.6f,
                Size = 0.7f,
                SizeRange = 0.2f,
                SpeedMin = 80f,
                SpeedMax = 100f,
                RotationMode = ParticleType.RotationModes.Random,
                // ScaleOut = true,
                FadeMode = ParticleType.FadeModes.Linear,
                UseActualDeltaTime = true
            };

            P_SwimLaunchBurst = new() {
                SourceChooser = new Chooser<MTexture>([
                    GFX.Game["particles/smoke0"],
                    GFX.Game["particles/smoke1"],
                    GFX.Game["particles/smoke2"],
                    GFX.Game["particles/smoke3"]
                ]),
                LifeMin = 0.3f,
                LifeMax = 1.0f,
                Size = 0.8f,
                SizeRange = 0.2f,
                SpeedMin = 120f,
                SpeedMax = 240f,
                Acceleration = new Vector2(0, 200),
                RotationMode = ParticleType.RotationModes.Random,
                ScaleOut = true,
                UseActualDeltaTime = true
            };
        }
        
        public static bool ShouldDoAnimation(Player player)
            => player is not null
            // && player.CollideCheck<Water>(player.Position + new Vector2(0, -15))
            && GetOptionBool(Option.CustomSwimming) 
            && GooberHelperModule.Settings.CustomSwimmingAnimation;

        [ILHook]
        private static void patch_Player_SwimUpdate(ILContext il) {
            var cursor = new ILCursor(il);

            cursor.EmitLdarg0();
            cursor.EmitDelegate(maybeCreateTrail);
        }

        private static void setPlayerRotation(Player player, PlayerExtensions.PlayerExtensionFields ext) {
            var speedAngle = player.GetConservedSpeed(ext).Angle() + MathF.PI / 2f;

            player.Sprite.Rate = MathF.Log2(player.Speed.Length() / 64f + 1f) + 1f;

            ext.PlayerRotationTarget = speedAngle;
        }

        public static bool OnUpdateSprite(Player player, PlayerExtensions.PlayerExtensionFields ext, ref bool somethingActive) {
            if(!ShouldDoAnimation(player))
                return false;

            somethingActive = true;

            player.Sprite.Rate = 1f;

            if(player.StateMachine.State == Player.StNormal) {                
                if(ext.IsDolphin) {
                    if((player.Speed / new Vector2(1f, 2f)).Length() > 90f) {
                        setPlayerRotation(player, ext);

                        player.Sprite.Play("spin");
                    } else {
                        ext.IsDolphin = false;

                        ext.PlayerRotationTarget = 0f;
                    }

                    return true;
                } else {
                    ext.PlayerRotationTarget = 0f;
                }
            } else if(player.StateMachine.State == Player.StSwim) {
                if(Input.Aim != Vector2.Zero) {
                    setPlayerRotation(player, ext);

                    var animationStarted = player.Sprite.CurrentAnimationID == "spin";

                    player.Sprite.Play("spin");
                    
                    if(!animationStarted) {
                        player.Sprite.CurrentAnimationFrame = 8;
                        player.Sprite.SetFrame(player.Sprite.currentAnimation.Frames[player.Sprite.CurrentAnimationFrame]);
                    }
                } else {
                    ext.PlayerRotationTarget = 0f;
                    
                    player.Sprite.Play("swimIdle");
                }

                return true;
            } else {
                ext.PlayerRotationTarget = 0f;
                ext.PlayerRotation = 0f;

                ext.IsDolphin = false;
            }

            return false;
        }

        private static void particleBurstOnSystem(Player player, ParticleSystem system) {
            for(var i = -0.5f; i <= 0.5f; i += 0.1f)
                system.Emit(
                    P_SwimLaunchBurst,
                    Random.Shared.Range(5, 10),
                    player.BottomCenter + Calc.AngleToVector(Random.Shared.NextAngle(), 10f),
                    new Vector2(5, 5),
                    Color.Lerp(Color.White, Color.SkyBlue, Random.Shared.NextFloat()) * Random.Shared.Range(0.2f, 0.4f),
                    Calc.AngleLerp(player.Speed.Angle(), -MathF.PI / 2f, 0.5f) + i
                );
        }

        public static void ParticleBurst(Player player) {
            particleBurstOnSystem(player, player.level.ParticlesBG);
            particleBurstOnSystem(player, player.level.ParticlesFG);
        }

        private static void maybeCreateTrail(Player player) {
            if(!GooberHelperModule.Settings.CustomSwimmingAnimation || !GetOptionBool(Option.CustomSwimming))
                return;

            var speed = player.Speed.Length();
            var factor = 1f / (25f / MathF.Exp(speed / 60f) + 1f);

            if(player.level.OnInterval(0.02f))
                player.level.Displacement.AddBurst(player.Center, 1f, 0, 40, factor * 0.3f, Ease.SineOut, Ease.SineIn);

            if(player.level.OnInterval(0.02f)) {
                player.level.ParticlesBG.Emit(
                    type: P_SwimTrail,
                    amount: (int)(factor * 4),
                    position: player.Center,
                    positionRange: new Vector2(4, 4),
                    color: Color.Lerp(Color.White, Color.SkyBlue, Random.Shared.NextFloat()) * 0.1f,
                    direction: MathF.PI + player.Speed.Angle() + Random.Shared.Range(-0.5f, 0.5f)
                );
            }
        }
    }
}