using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.Helpers;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Dashing {
    [GooberHelperOption(Option.DashesDontResetSpeed)]
    public static class DashesDontResetSpeed {
        [ILHook]
        private static void patch_Player_DashCoroutine(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNextBestFit(MoveType.After, 
                instr => instr.MatchLdflda<Player>("DashDir"),
                instr => instr.MatchLdfld<Vector2>("Y"),
                instr => instr.MatchLdcR4(0),
                instr => instr.MatchBgtUn(out _)
            )) {
                cursor.Index--;

                cursor.EmitLdloc1();
                cursor.EmitLdcI4((int)DashesDontResetSpeedValue.Legacy);
                cursor.EmitDelegate(overrideCondition);
            }

            if(cursor.TryGotoNextBestFit(MoveType.After, 
                instr => instr.MatchLdflda<Player>("Speed"),
                instr => instr.MatchLdfld<Vector2>("Y"),
                instr => instr.MatchLdcR4(0),
                instr => instr.MatchBgeUn(out _)
            )) {
                cursor.Index--;

                cursor.EmitLdloc1();
                cursor.EmitLdcI4((int)DashesDontResetSpeedValue.On);
                cursor.EmitDelegate(overrideCondition);
            }
        }

        private static float overrideCondition(float orig, Player player, int minimumValue)
            => (GetOptionBool(Option.CustomSwimming) && player.CollideCheck<Water>())
            || GetOptionValue(Option.DashesDontResetSpeed) >= minimumValue
                ? float.MinValue
                : orig;
    }
}