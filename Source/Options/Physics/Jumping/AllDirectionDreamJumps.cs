using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Jumping {
    [GooberHelperOption]
    public class AllDirectionDreamJumps : AbstractOption {
        [ILHook(typeof(Player), "DreamDashUpdate")]
        [ILHook(typeof(Player), "DreamDashEnd")]
        private static void allowAllDirectionDreamJumps(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNext(MoveType.After,
                instr => instr.MatchLdflda<Player>("DashDir"),
                instr => instr.MatchLdfld<Vector2>("X"),
                instr => instr.MatchLdcR4(0f)
            )) {
                cursor.EmitDelegate(overrideDashDirX);
            }
        }

        private static float overrideDashDirX(float orig)
            => GetOptionBool(Option.AllDirectionDreamJumps)
                ? 13 //literally anything except 0 (thank you dragonned for the number submission)
                : orig;
    }
}