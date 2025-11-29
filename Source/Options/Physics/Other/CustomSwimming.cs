using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Extensions;
using Celeste.Mod.GooberHelper.Helpers;
using Celeste.Mod.Helpers;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Other {
    [GooberHelperOption(Option.CustomSwimming)]
    public static class CustomSwimming {
        private static bool tryCustomSwimmingWalljump(Player self, Vector2 vector) {
            var ext = self.GetExtensionFields();

            var redirectSpeed = Math.Max(self.Speed.Length(), ext.AwesomeRetentionSpeed.Length()) + 20;

            if(ext.AwesomeRetentionTimer <= 0 || !ext.AwesomeRetentionWasInWater)
                return false;

            var redirectDirection = -ext.AwesomeRetentionDirection;

            if(redirectDirection != Vector2.Zero && redirectSpeed != 0) {
                Input.Jump.ConsumeBuffer();
                self.Speed = redirectDirection.SafeNormalize() * redirectSpeed;

                var index = ext.AwesomeRetentionPlatform.GetWallSoundIndex(self, Math.Sign(ext.AwesomeRetentionDirection.X));

                Dust.Burst(self.Center - redirectDirection * self.Collider.Size / 2, redirectDirection.Angle(), 4, self.DustParticleFromSurfaceIndex(index));
                self.Play(SurfaceIndex.GetPathFromIndex(index) + "/landing", "surface_index", index);
            }

            return true;
        }

        private static bool doCustomSwimMovement(Player self, Vector2 vector) {
            var defaultSpeed = 90f;

            self.Speed = Vector2.Dot(self.Speed.SafeNormalize(Vector2.Zero), vector) < -0.5f || vector.Length() == 0 || self.Speed.Length() <= 90
                ? Calc.Approach(self.Speed, defaultSpeed * vector, 350f * Engine.DeltaTime)
                : self.Speed.RotateTowards(vector.Angle(), 10f * Engine.DeltaTime);

            //awesome retention is used while underwater
            //i need to think of a better name for that
            self.wallSpeedRetentionTimer = 0f;

            if(Input.Jump.Pressed) {
                var distance = Math.Min(self.Speed.Y * Engine.DeltaTime, 0) - 10f;

                if(!self.CollideCheck<Water>(self.Position + Vector2.UnitY * distance)) {
                    if(self.Speed.Y >= 0)
                        return true;

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

        [ILHook]
        private static void patch_Player_SwimUpdate(ILContext il) {
            var cursor = new ILCursor(il);

            var originalMovementEndLabel = cursor.DefineLabel();
            var customMovementEndLabel = cursor.DefineLabel();
            var swimJumpEndLabel = cursor.DefineLabel();
            var afterReturnLabel = cursor.DefineLabel();

            HookHelper.Begin(cursor, "making custom swimming work");

            HookHelper.Move("going after safenormalize", () => {
                cursor.GotoNextBestFit(MoveType.After, 
                    instr => instr.MatchLdloc1(),
                    instr => instr.MatchCallOrCallvirt(((Func<Vector2, Vector2>)Calc.SafeNormalize).Method),
                    instr => instr.MatchStloc1()
                );
            });

            HookHelper.Do(() => {
                //if(GetOptionBool(Option.CustomSwimming))
                cursor.EmitDelegate(getOptionBool);
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
            });

            HookHelper.Move("going before moveX", () => {
                cursor.GotoNextBestFit(MoveType.Before, 0x20,
                    instr => instr.MatchLdloc0(),
                    instr => instr.MatchBrtrue(out _),
                    instr => instr.MatchLdarg0(),
                    instr => instr.MatchLdfld<Player>("moveX")
                );
            });

            HookHelper.Do(() => {
                cursor.MarkLabel(originalMovementEndLabel);
            });

            HookHelper.Move("going before Jump.Pressed", () => {
                cursor.GotoNextBestFit(MoveType.Before,
                    instr => instr.MatchLdsfld(typeof(Input).GetField("Jump")),
                    instr => instr.MatchCallOrCallvirt<VirtualButton>("get_Pressed"),
                    instr => instr.MatchBrfalse(out swimJumpEndLabel)
                );
            });

            HookHelper.Do(() => {
                cursor.MoveAfterLabels();
                cursor.EmitDelegate(getOptionBool);
                cursor.EmitBrtrue(swimJumpEndLabel);
            });

            HookHelper.End();
        }

        [ILHook]
        private static void patch_Player_DashUpdate(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNextBestFit(MoveType.Before, 
                instr => instr.MatchLdarg0(),
                instr => instr.MatchCallOrCallvirt<Player>("get_SuperWallJumpAngleCheck"),
                instr => instr.MatchBrfalse(out _)
            )) {
                var afterReturnLabel = cursor.DefineLabel();

                cursor.MoveAfterLabels();

                //if(customswimming && tryCustomSwimmingJump(this, this.DashDir))
                cursor.EmitLdarg0();
                cursor.EmitDelegate(getWalljumpConditionMaybeDoIt);

                cursor.EmitBrfalse(afterReturnLabel);

                //return StSwim;
                cursor.EmitLdcI4(Player.StSwim);
                cursor.EmitRet();

                cursor.MarkLabel(afterReturnLabel);
            }
        }

        [ILHook]
        private static void patch_Player_DashCoroutine(ILContext il) {
            var cursor = new ILCursor(il);

            //remove the 0.75x speed multiplier when dashing while in contact with water
            if(cursor.TryGotoNextBestFit(MoveType.After, 
                instr => instr.MatchLdloc1(),
                instr => instr.MatchCallOrCallvirt<Entity>("CollideCheck"), //collidecheck<water>
                instr => instr.MatchBrfalse(out var _)
            )) {
                cursor.Index--;

                cursor.EmitDelegate(getOptionBool);
                cursor.EmitNot();
                cursor.EmitAnd();
            }
        }

        [OnHook]
        private static bool patch_Player_WallJumpCheck(On.Celeste.Player.orig_WallJumpCheck orig, Player self, int dir) {
            if(self.CollideCheck<Water>() && GetOptionBool(Option.CustomSwimming))
                return false;

            return orig(self, dir);
        }

        [ILHook]
        private static void patch_Player_SwimBegin(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(0.5f)))
                cursor.EmitDelegate(overrideHalfSpeed);
        }

        private static bool getOptionBool()
            => GetOptionBool(Option.CustomSwimming);

        private static bool getWalljumpConditionMaybeDoIt(Player player)
            => GetOptionBool(Option.CustomSwimming) && Input.Jump && tryCustomSwimmingWalljump(player, player.DashDir);

        private static float overrideHalfSpeed(float orig)
            => GetOptionBool(Option.CustomSwimming)
                ? 1f
                : 0f;
    }
}