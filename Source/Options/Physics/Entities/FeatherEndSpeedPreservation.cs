using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.Helpers;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Entities {
    [GooberHelperOption(Option.FeatherEndSpeedPreservation)]
    public static class FeatherEndSpeedPreservation {
        [ILHook]
        private static void patch_Player_StarFlyUpdate(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNextBestFit(MoveType.After,
                instr => instr.MatchLdfld<Player>("starFlyTimer"),
                instr => instr.MatchLdcR4(0f),
                instr => instr.MatchBgtUn(out var _)
            )) {
                var label = cursor.DefineLabel();
                var index = cursor.Index;

                if(cursor.TryGotoNextBestFit(MoveType.Before, 
                    instr => instr.MatchLdcI4(1),
                    instr => instr.MatchLdcI4(1),
                    instr => instr.MatchCall(typeof(Input).GetMethod("Rumble"))
                )) {

                    cursor.MarkLabel(label);
                    cursor.Index = index;

                    cursor.EmitLdarg0();
                    cursor.EmitDelegate(tryPreserveSpeed);
                    cursor.EmitBrtrue(label);
                }
            }
        }

        private static bool tryPreserveSpeed(Player player) {
            if(!GetOptionBool(Option.FeatherEndSpeedPreservation))
                return false;
            
            //free feather end boosts
            if(player.Speed.Y <= 0f) {
                player.varJumpSpeed = player.Speed.Y;
                player.AutoJump = true;
                player.AutoJumpTimer = 0f;
                player.varJumpTimer = 0.2f;
            }

            return true;
        }
    }
}