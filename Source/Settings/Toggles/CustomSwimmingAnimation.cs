using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Extensions;
using Celeste.Mod.GooberHelper.Helpers;
using Celeste.Mod.GooberHelper.Options.GeneralHooks;
using Celeste.Mod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Settings.Toggles {
    [GooberHelperSetting]
    public class CustomSwimmingAnimation : AbstractToggle {
        public static ParticleType P_SwimTrail;

        [OnLoadContent]
        public static void LoadContent()
            => P_SwimTrail = new() {
                SourceChooser = new Chooser<MTexture>([
                    GFX.Game["particles/zappysmoke00"],
                    GFX.Game["particles/zappysmoke01"],
                    GFX.Game["particles/zappysmoke02"],
                    GFX.Game["particles/zappysmoke03"]
                ]),
                LifeMin = 0.3f,
                LifeMax = 0.5f,
                Size = 0.7f,
                SizeRange = 0.2f,
                // DirectionRange = 0.5f,
                SpeedMin = 80f,
                SpeedMax = 100f,
                RotationMode = ParticleType.RotationModes.Random,
                ScaleOut = true,
                UseActualDeltaTime = true
            };
        
        public static bool ShouldDoAnimation(Player player)
            => player is not null
            && player.StateMachine.State == Player.StSwim
            && GetOptionBool(Option.CustomSwimming) 
            && GooberHelperModule.Settings.CustomSwimmingAnimation;

        [ILHook]
        private static void patch_Player_SwimUpdate(ILContext il) {
            var cursor = new ILCursor(il);

            cursor.EmitLdarg0();
            cursor.EmitDelegate(maybeCreateTrail);
        }

        [ILHook]
        private static void patch_Player_UpdateSprite(ILContext il) {
            var cursor = new ILCursor(il);

            HookHelper.Begin(cursor, "bingle b", true);

            var exitLabel = cursor.DefineLabel();

            HookHelper.Move("going to the stswim check", () => {
                cursor.GotoNextBestFit(MoveType.After,
                    instr => instr.MatchLdarg0(),
                    instr => instr.MatchLdfld<Player>("StateMachine"),
                    instr => instr.MatchCallOrCallvirt<StateMachine>("get_State"),
                    instr => instr.MatchLdcI4(3),
                    instr => instr.MatchBneUn(out var _)
                );
            });
            
            HookHelper.Do(() => {
                cursor.EmitLdarg0();
                cursor.EmitDelegate(maybeUpdateSprite);
                cursor.EmitBrtrue(exitLabel);
            });

            HookHelper.Move("grabbing the exit label", () => {
                cursor.GotoNextBestFit(MoveType.After,
                    instr => instr.MatchLdstr("swimDown"),
                    instr => instr.MatchLdcI4(0),
                    instr => instr.MatchLdcI4(0),
                    instr => instr.MatchCallOrCallvirt<Sprite>("Play"),
                    instr => instr.MatchBr(out exitLabel)
                );
            });

            HookHelper.Do(() => {});

            for(var i = 0; i < 2; i++) {
                HookHelper.Move($"going before the sprite rate assignment (#{i + 1})", () => {
                    cursor.GotoNextBestFit(MoveType.AfterLabel, instr => instr.MatchStfld<Sprite>("Rate"));
                });

                HookHelper.Do(() => {
                    cursor.EmitLdarg0();
                    cursor.EmitDelegate(overrideSpriteRate);
                });
            }

            HookHelper.End();
        }

        private static bool maybeUpdateSprite(Player player) {
            if(!ShouldDoAnimation(player)) {
                PlayerRender.PlayerRotationTarget = 0f;
                PlayerRender.PlayerRotation = 0f;

                return false;
            }

            if(Input.Aim != Vector2.Zero) {
                var speedAngle = player.GetConservedSpeed().Angle() + MathF.PI / 2f;

                PlayerRender.PlayerRotationTarget = speedAngle;

                var animationStarted = player.Sprite.CurrentAnimationID == "spin";

                player.Sprite.Play("spin");
                
                if(!animationStarted) {
                    player.Sprite.CurrentAnimationFrame = 8;
                    player.Sprite.SetFrame(player.Sprite.currentAnimation.Frames[player.Sprite.CurrentAnimationFrame]);
                }
            } else {
                PlayerRender.PlayerRotationTarget = 0f;
                player.Sprite.Play("swimIdle");
            }
            
            return true;
        }

        private static float overrideSpriteRate(float orig, Player player) {
            if(!ShouldDoAnimation(player))
                return orig;

            return orig * (MathF.Log2(player.Speed.Length() / 64f + 1f) + 1f);
        }

        private static void maybeCreateTrail(Player player) {
            if(!GooberHelperModule.Settings.CustomSwimmingAnimation)
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
                    color: Color.Lerp(Color.White, Color.Cyan, Random.Shared.NextFloat()) * 0.1f,
                    direction: MathF.PI + player.Speed.Angle() + Random.Shared.Range(-0.5f, 0.5f)
                );
            }
        }
    }
}