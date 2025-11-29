using System.Reflection;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Helpers;
using Celeste.Mod.Helpers;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Jumping {
    [GooberHelperOption(Option.AllowUpwardsCoyote)]
    public static class AllowUpwardsCoyote {
        private static bool upwardsCoyote = false;
        private static FieldInfo f_AllowUpwardsCoyote_upwardsCoyote = typeof(AllowUpwardsCoyote).GetField(nameof(upwardsCoyote), Utils.BindingFlagsAll);

        [ILHook]
        private static void patch_Player_Update(ILContext il) {
            var cursor = new ILCursor(il);

            HookHelper.Begin(cursor, "implementing allow upwards coyote");
            
            //this is before anything that uses onGround is called
            //it doesnt actually change onGround or use it or anything but it makes sense for it to be here
            HookHelper.Move("going before the onGround stuff", () => {     
                cursor.GotoNextBestFit(MoveType.AfterLabel,
                    instr => instr.MatchLdarg0(),
                    instr => instr.MatchLdfld<Player>("StateMachine"),
                    instr => instr.MatchCallOrCallvirt<StateMachine>("get_State"),
                    instr => instr.MatchLdcI4(9),
                    instr => instr.MatchBneUn(out _)
                );
            });

            HookHelper.Do(() => {
                cursor.EmitLdarg0();
                cursor.EmitDelegate(determineUpwardsCoyote);
            });

            var beforeStaminaRefillLabel = cursor.DefineLabel();
            var beforeCoyoteRefillLabel = cursor.DefineLabel();

            for(var i = 0; i < 2; i++) {
                if(i == 0) {
                    HookHelper.Move("going before stamina refill", () => {
                        cursor.GotoNextBestFit(MoveType.Before,
                            instr => instr.MatchLdarg0(),
                            instr => instr.MatchLdcR4(110),
                            instr => instr.MatchStfld<Player>("Stamina")
                        );
                    });

                    HookHelper.Do(() => {
                        cursor.MarkLabel(beforeStaminaRefillLabel);
                    });
                } else {
                    HookHelper.Move("going before coyote refill", () => {
                        cursor.GotoNextBestFit(MoveType.Before,
                            instr => instr.MatchLdarg0(),
                            instr => instr.MatchLdcR4(0.1f),
                            instr => instr.MatchStfld<Player>("jumpGraceTimer")
                        );
                    });
                    
                    HookHelper.Do(() => {
                        cursor.MarkLabel(beforeCoyoteRefillLabel);
                    });
                }

                HookHelper.Move($"going before onGround (#{i + 1})", () => {
                    cursor.GotoPrevBestFit(MoveType.AfterLabel,
                        instr => instr.MatchLdarg0(),
                        instr => instr.MatchLdfld<Player>("onGround"),
                        instr => instr.MatchBrfalse(out _)
                    );
                });

                HookHelper.Do((i) => {
                    cursor.EmitLdsfld(f_AllowUpwardsCoyote_upwardsCoyote);
                    cursor.EmitBrtrue(i == 0 ? beforeStaminaRefillLabel : beforeCoyoteRefillLabel);
                }, i);
            }

            HookHelper.End();
        }

        private static void determineUpwardsCoyote(Player player)
            => upwardsCoyote =
                GetOptionBool(Option.AllowUpwardsCoyote) &&
                player.Speed.Y < 0 &&
                (
                    player.CollideCheck<Solid>(player.Position + Vector2.UnitY) ||
                    player.CollideCheck<JumpThru>(player.Position + Vector2.UnitY * player.Collider.Height) && player.CollideCheck<JumpThru>(player.Position + Vector2.UnitY)
                );
    }
}