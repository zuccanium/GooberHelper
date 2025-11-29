using System;
using System.Reflection;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Extensions;
using Celeste.Mod.Helpers;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Other {
    public static class FastFallHitboxSquish {
        private static bool ignoreFalseDucking = false;
        private static float oldCompression = 0f;
        
        private static FieldInfo f_FastFallHitboxSquish_ignoreFalseDucking = typeof(FastFallHitboxSquish).GetField(nameof(ignoreFalseDucking), Utils.BindingFlagsAll);

        [ILHook]
        private static void patch_Player_Update(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNext(MoveType.AfterLabel, instr => instr.MatchCallOrCallvirt<Player>("set_Ducking"))) {
                cursor.EmitLdcI4(1);
                cursor.EmitStsfld(f_FastFallHitboxSquish_ignoreFalseDucking);
            }
        }

        [ILHook]
        private static void patch_Player_set_Ducking(ILContext il) {
            var cursor = new ILCursor(il);

            cursor.EmitLdarg0();
            cursor.EmitLdarg1();
            cursor.EmitDelegate(maybeUncompressHitboxesOnFalseDucking);
        }

        [ILHook(typeof(Player), "NormalUpdate")]
        [ILHook(typeof(Player), "OnCollideV")]
        private static void changeHitboxWhenSquishedX(ILContext il) {
            var cursor = new ILCursor(il);

            var targetValue = 0f;
            var amountLocalIndex = 0;

            if(cursor.TryGotoNextBestFit(MoveType.After,
                instr => instr.MatchLdcR4(1f),
                instr => instr.MatchLdcR4(out targetValue),
                instr => instr.MatchLdloc(out amountLocalIndex),
                instr => instr.MatchCall(((Func<float, float, float, float>)MathHelper.Lerp).Method),
                instr => instr.MatchStfld<Vector2>("X")
            )) {
                cursor.EmitLdarg0();
                
                if(targetValue == 0.5f) {
                    cursor.EmitLdloc(amountLocalIndex);
                } else {
                    cursor.EmitLdcR4(0);
                }

                cursor.EmitDelegate(setCompression);
            }
        }

        [OnHook]
        private static void patch_Player_Update(On.Celeste.Player.orig_Update orig, Player self) {
            var ext = self.GetExtensionFields();

            oldCompression = ext.HitboxCompression;
            
            orig(self);

            if(Input.MoveY != 1f || self.Speed.Y < self.maxFall)
                ext.HitboxCompression = Calc.Approach(ext.HitboxCompression, 0f, 3.5f * Engine.DeltaTime);

            if(self.StateMachine.State != Player.StNormal)
                ext.HitboxCompression = 0f;

            updateCompression(self);
        }
        
        private static void setColliderWidth(Hitbox collider, float width) {
            collider.Width = width;
            collider.CenterX = width % 2 * 0.5f;
        }

        private static void compressAllColliders(Player player, float compression) {
            //i hate using a constant here but what can you do
            var width = (int)MathF.Round(8f * (1f - compression));

            setColliderWidth(player.duckHitbox, width);
            setColliderWidth(player.duckHurtbox, width);
            setColliderWidth(player.normalHitbox, width);
            setColliderWidth(player.normalHurtbox, width);
        }

        private static void updateCompression(Player player) {
            var optionValue = GetOptionValue(Option.FastFallHitboxSquish) / 100f;
            var ext = player.GetExtensionFields();

            if(optionValue == 0)
                ext.HitboxCompression = 0;

            if(ext.HitboxCompression == 0 && oldCompression == 0)
                return;

            compressAllColliders(player, ext.HitboxCompression * optionValue);

            if(!player.CollideCheck<Solid>())
                return;
                            
            for(var i = 1; i <= 4; i++) {
                for(var dir = -1; dir <= 1; dir++) {
                    if(player.CollideCheck<Solid>(player.Position + new Vector2(dir * i, 0)))
                        continue;

                    player.Position.X += i * dir;
                    player.movementCounter.X = 0f;

                    return;
                }
            }

            ext.HitboxCompression = oldCompression;

            compressAllColliders(player, ext.HitboxCompression * optionValue);
        }

        private static void maybeUncompressHitboxesOnFalseDucking(Player player, bool value) {
            if(value == false && !ignoreFalseDucking) {
                setCompression(player, 0);
                
                updateCompression(player);
            }

            ignoreFalseDucking = false;
        }

        private static void setCompression(Player player, float compression)
            => player.GetExtensionFields().HitboxCompression = compression;

        private static void setOldCompression(Player player)
            => oldCompression = player.GetExtensionFields().HitboxCompression;
    }
}