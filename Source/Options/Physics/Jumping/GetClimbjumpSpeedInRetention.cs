using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Helpers;
using Celeste.Mod.Helpers;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Jumping {
    [GooberHelperOption]
    public class GetClimbjumpSpeedInRetention : AbstractOption {
        [ILHook]
        private static void patch_Player_ClimbJump(ILContext il) {
            var cursor = new ILCursor(il);

            HookHelper.Begin(cursor, "bingle bongle get climbjump speed in retenteniotnwetwoietj");

            HookHelper.Move("before jump", () => {
                cursor.GotoNextBestFit(MoveType.Before,
                    instr => instr.MatchLdarg0(),
                    instr => instr.MatchLdcI4(0),
                    instr => instr.MatchLdcI4(0),
                    instr => instr.MatchCallOrCallvirt<Player>("Jump")
                );
            });

            HookHelper.Do(() => {
                cursor.EmitLdarg0();
                cursor.EmitDelegate(setSpeedToRetention);
            });

            HookHelper.Move("before jump", () => {
                cursor.GotoNextBestFit(MoveType.After, instr => instr.MatchCallOrCallvirt<Player>("Jump"));
            });

            HookHelper.Do(() => {
                cursor.EmitLdarg0();
                cursor.EmitDelegate(setRetentionToSpeed);
            });

            HookHelper.End();
        }

        private static void setSpeedToRetention(Player player) {
            if(player.wallSpeedRetentionTimer > 0f && GetOptionBool(Option.GetClimbjumpSpeedInRetention))
                player.Speed.X = player.wallSpeedRetained;
        }

        private static void setRetentionToSpeed(Player player) {
            if(player.wallSpeedRetentionTimer > 0f && GetOptionBool(Option.GetClimbjumpSpeedInRetention))
                player.wallSpeedRetained = player.Speed.X;
        }
    }
}