using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.Helpers;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Other {
    [GooberHelperOption]
    public class RetentionLength : AbstractOption {
        public override OptionGroup HeadGroup { get; set; } = OptionGroup.Special;

        public override OptionType Type { get; set; } = OptionType.Float;
        public override float DefaultValue { get; set; } = 4f;
        public override float? RightMin { get; set; } = 0f;
        public override float Step { get; set; } = 1f;
        public override string Suffix { get; set; } = "f";

        [ILHook]
        private static void patch_Player_OnCollideH(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNextBestFit(MoveType.After, instr => instr.MatchLdcR4(0.06f))) {
                cursor.EmitDelegate(OverrideRetentionLength);
            }
        }
        
        //exposed so GeneralHooks/VerticalRetention can use it
        public static float OverrideRetentionLength(float orig) {
            var retentionLength = GetOptionValue(Option.RetentionLength);

            return retentionLength != 4f
                ? retentionLength / 60f
                : orig;
        }
    }
}