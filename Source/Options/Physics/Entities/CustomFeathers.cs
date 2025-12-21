using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Extensions;
using Celeste.Mod.Helpers;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Entities {
    [GooberHelperOption(Option.CustomFeathers)]
    public static class CustomFeathers {
        [ILHook(typeof(Player), "OnCollideH")]
        [ILHook(typeof(Player), "OnCollideV")]
        private static void boyoyoyoing(ILContext il) { //boyoyoyoyoing
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNextBestFit(MoveType.After,
                instr => instr.MatchDup(),
                instr => instr.MatchLdindR4(),
                instr => instr.MatchLdcR4(-0.5f)
            )) {
                cursor.EmitDelegate(overrideBounceFactor);
            }
        }

        [ILHook]
        private static void patch_Player_StarFlyUpdate(ILContext il) {
            var cursor = new ILCursor(il);

            var start = cursor.Index;

            var lowMult = 0.65f;
            var midMult = 0.90f;
            var highMult = 1.05f;

            //destroyer of reality
            // cursor.EmitDelegate(() => {
            //     Player player = Engine.Scene.Tracker.GetEntity<Player>();

            //     (Engine.Scene as Level).Displacement.AddBurst(player.Center, 2f, 8f, 1000f, 1f, null, null);
            // });

            var matches = new float[] { 91, 140, 190, 140, 140, 140 };
            var replaceMults = new float[] { lowMult, midMult, highMult, midMult, midMult, 0.75f };

            for(var i = 0; i < matches.Length; i++) {
                //fsr i have to put this in a variable instead of just accessing the array from within the delegate. that took way longer to figure out than it shouldve
                var replaceMult = replaceMults[i];

                if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(matches[i]))) {
                    cursor.EmitLdarg0();
                    cursor.EmitLdcR4(replaceMult);
                    cursor.EmitDelegate(overrideFeatherSpeed);
                }
            }

            cursor.Index = start;
        }
        
        [ILHook]
        private static void patch_Player_StarFlyCoroutine(ILContext il) {
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
                cursor.EmitDelegate(maybeSkipIntoAndPlayAnimation);
                cursor.Emit(OpCodes.Brtrue_S, afterStarFlyStartLabel);
            }
            
            if(cursor.TryGotoNextBestFit(MoveType.After,
                instr => instr.MatchLdcR4(250),
                instr => instr.MatchCall<Vector2>("op_Multiply"),
                instr => instr.MatchStfld<Player>("Speed")
            )) {
                cursor.EmitLdloc1();
                cursor.EmitDelegate(setInitialFeatherSpeed);
            }
        }

        [ILHook]
        private static void patch_Player_StarFlyBegin(ILContext il) {
            var cursor = new ILCursor(il);

            cursor.EmitLdarg0();
            cursor.EmitDelegate(setPreservedSpeed);
        }

        private static float overrideBounceFactor(float orig)
            => GetOptionBool(Option.FeatherEndSpeedPreservation)
                ? -1f :
                orig;

        private static float overrideFeatherSpeed(float value, Player player, float replaceMult)
            => GetOptionBool(Option.CustomFeathers)
                ? Math.Max(player.GetExtensionFields().StarFlySpeedPreserved.Length() * replaceMult, value)
                : value;

        private static void setInitialFeatherSpeed(Player player) {
            if(GetOptionBool(Option.CustomFeathers)) {
                var direction = GetOptionValue(Option.CustomFeathers) == (int)CustomFeathersValue.KeepIntro
                    ? player.Speed
                    : player.GetExtensionFields().StarFlySpeedPreserved;

                player.Speed = direction.SafeNormalize() * Math.Max(player.GetExtensionFields().StarFlySpeedPreserved.Length(), player.Speed.Length());
            }
        }
        
        private static bool maybeSkipIntoAndPlayAnimation(Player player) {
            if(GetOptionValue(Option.CustomFeathers) == (int)CustomFeathersValue.SkipIntro) {
                player.Sprite.Play("starFly", false, false);

                return true;
            }

            return false;
        }

        private static void setPreservedSpeed(Player player)
            => player.GetExtensionFields().StarFlySpeedPreserved = player.Speed;
    }
}