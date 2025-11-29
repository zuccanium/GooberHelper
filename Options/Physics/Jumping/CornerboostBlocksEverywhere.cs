using System;
using System.Reflection;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Helpers;
using Celeste.Mod.Helpers;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Jumping {
    [GooberHelperOption(Option.CornerboostBlocksEverywhere)]
    public static class CornerboostBlocksEverywhere {
        private static int redoLogicThreshold = 0;
        private static FieldInfo f_CornerboostBlocksEverywhere_redoLogicThreshold = typeof(CornerboostBlocksEverywhere).GetField(nameof(redoLogicThreshold), Utils.BindingFlagsAll);


        [ILHook]
        private static void patch_Player_WallJumpCheck(ILContext il) {
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

            HookHelper.Begin(cursor, "implementing cornerboost blocks everywhere");

            HookHelper.Move("going before the climb bounds check", () => {
                cursor.GotoNextBestFit(MoveType.Before,
                    instr => instr.MatchLdarg0(),
                    instr => instr.MatchLdarg1(),
                    instr => instr.MatchCallOrCallvirt<Player>("ClimbBoundsCheck")
                );
            });
            
            HookHelper.Do(() => {
                cursor.MoveAfterLabels();

                cursor.EmitLdarg0();
                cursor.EmitLdloc0();
                cursor.EmitDelegate(overrideDistance);
                cursor.EmitStloc0();
            });

            //this is going to be the worst thing ever
            //i am so sorry

            //okay so essentially what im doing is:
            //if the current collision distance returned false, assume that its overshooting it and subtract the player hitbox width from the collision distance
            //as long as the new collision distance is greater than zero, return back to the start of the evaluation
            //this should work with custom entities such as maddiehelpinghand sideways jumpthrus

            var startLabel = cursor.DefineLabel();
            var endLabel = cursor.DefineLabel();

            HookHelper.Move("going before loading player.Level", () => {
                cursor.GotoNext(MoveType.Before,
                    instr => instr.MatchLdarg0(),
                    instr => instr.MatchLdfld<Player>("level")
                );
            });
            
            HookHelper.Do(() => {
                cursor.MarkLabel(startLabel);
            });

            HookHelper.Move("going before first return", () => {
                cursor.GotoNext(MoveType.Before, instr => instr.MatchBrtrue(out endLabel));
                cursor.GotoNext(MoveType.Before, instr => instr.MatchRet());
            });
            
            HookHelper.Do(() => {
                cursor.EmitBrfalse(endLabel);
                cursor.EmitLdcI4(1);
            });

            HookHelper.Move("going before the second return", () => {
                cursor.GotoNext(MoveType.AfterLabel,
                    instr => instr.MatchLdcI4(0),
                    instr => instr.MatchRet()
                );
            });
        
            HookHelper.Do(() => {
                //num (local 0 that stores collision distance) -= 8 (hitbox width);
                cursor.EmitLdloc0();
                cursor.EmitLdcI4(8);
                cursor.EmitSub();
                cursor.EmitStloc0();

                //if num (local 0 that stores collision distance) > 0, do the check again
                //except it not actually zero
                //its only zero when the option is enabled (as set in overrideDistance)
                //its int.MaxValue when the option isnt enabled so it shouldnt mess with anything that does weird stuff
                cursor.EmitLdloc0();
                cursor.EmitLdsfld(f_CornerboostBlocksEverywhere_redoLogicThreshold);
                cursor.EmitBgt(startLabel);
            });

            HookHelper.End();
        }

        private static int overrideDistance(Player player, int originalDistance) {
            if(GetOptionBool(Option.CornerboostBlocksEverywhere)) {
                redoLogicThreshold = 0;
                
                return Math.Max(originalDistance, (int)Math.Ceiling(Math.Abs(player.Speed.X) * Engine.DeltaTime) + 1);
            }

            redoLogicThreshold = int.MaxValue;

            return originalDistance;
        }
    }
}