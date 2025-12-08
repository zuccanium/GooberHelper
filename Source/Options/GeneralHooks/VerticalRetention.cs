using System;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Extensions;
using Celeste.Mod.GooberHelper.Options.Physics.Other;
using Celeste.Mod.Helpers;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.GeneralHooks {
    public static class VerticalRetention {
        [ILHook]
        private static void patch_Player_Update(ILContext il) {
            var cursor = new ILCursor(il);

            cursor.EmitLdarg0();
            cursor.EmitDelegate(updateRetention);
        }

        [ILHook]
        private static void patch_Player_OnCollideV(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNextBestFit(MoveType.AfterLabel,
                instr => instr.MatchLdarg1(),
                instr => instr.MatchLdfld<CollisionData>("Hit"),
                instr => instr.MatchBrfalse(out var _)
            )) {
                cursor.EmitLdarg0();
                cursor.EmitDelegate(setRetention);
            }
        }

        private static void setRetention(Player player) {
            var ext = player.GetExtensionFields();

            if(ext.VerticalRetentionTimer <= 0f) {
                ext.VerticalRetentionSpeed = player.Speed.Y;
                ext.VerticalRetentionTimer = RetentionLength.OverrideRetentionLength(0.06f);
            }
        }

        private static void updateRetention(Player player) {
            var ext = player.GetExtensionFields();

            if(ext.VerticalRetentionTimer <= 0f) {
                ext.VerticalRetentionSpeed = 0f;

                return;
            }
            
            Utils.Log($"[vertical retention info] speed: {ext.VerticalRetentionSpeed}, time: {ext.VerticalRetentionTimer}");

            ext.VerticalRetentionTimer -= Engine.DeltaTime;
        }
    }
}