using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.Helpers;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Physics.Moving {
    [GooberHelperOption]
    public class HalfGravityThreshold : AbstractOption {
        public override OptionGroup HeadGroup { get; set; } = OptionGroup.AllowingThings;
        public override OptionType Type { get; set; } = OptionType.Float;
        public override Type EnumType { get; set; } = typeof(Value);
        public override float DefaultValue { get; set; } = Player.HalfGravThreshold;
        public override float? RightMin { get; set; } = 0f;
        public override float Step { get; set; } = 10f;
        public override string Suffix { get; set; } = " px/s";

        public enum Value {
            None = ReservedHybridEnumConstant + 0
        }
        
        private static readonly FieldInfo f_Player_Speed = typeof(Player).GetField(nameof(Player.Speed));
        private static readonly FieldInfo f_Vector2_Y = typeof(Vector2).GetField(nameof(Vector2.Y));
        
        [ILHook]
        private static void patch_Player_NormalUpdate(ILContext il) {
            var cursor = new ILCursor(il);
            
            if(cursor.TryGotoNextBestFit(MoveType.After,
                instr => instr.MatchLdarg0(),
                instr => instr.MatchLdflda(f_Player_Speed),
                instr => instr.MatchLdfld(f_Vector2_Y),
                instr => instr.MatchCall(((Func<float, float>)(Math.Abs)).Method),
                instr => instr.MatchLdcR4(Player.HalfGravThreshold)
            )) {
                cursor.EmitDelegate(overrideHalfGravThreshold);
            }
        }

        private static float overrideHalfGravThreshold(float orig) {
            var value = GetOptionValue(Option.HalfGravityThreshold);

            return
                value == (int)Value.None
                    ? float.MaxValue
                    
                : value != Player.HalfGravThreshold
                    ? value
                    
                : orig;
        }
    }
}