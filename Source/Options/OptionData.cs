using System;

namespace Celeste.Mod.GooberHelper.Options {
    public class OptionData {
        public Option Id;
        public string Name;
        public OptionType Type;
        public OptionCategory Category;
        public float DefaultValue;
        public float Min = 0;
        public float Max = 100;
        public float Step = 1;
        public bool ExponentialIncrease = true;
        public bool IgnoreZero;
        public string MaxLabel;
        public string Suffix;
        public Type EnumType;
        public float EnumMax;

        public OptionData(Option option, OptionType type = OptionType.Boolean, float defaultValue = 0) {
            Id = option;
            Name = Enum.GetName(typeof(Option), option);
            Type = type;
            DefaultValue = defaultValue;
        }

        public OptionData(Option option, Type enumType, OptionType type, float defaultValue) {
            Id = option;
            Name = Enum.GetName(typeof(Option), option);
            Type = type;
            EnumType = enumType;
            DefaultValue = defaultValue;
            EnumMax = Enum.GetValues(enumType).Length;
        }

        public OptionData(Option option, Type enumType, Enum defaultValue) {
            Id = option;
            Name = Enum.GetName(typeof(Option), option);
            EnumType = enumType;
            Type = OptionType.Enum;
            DefaultValue = Convert.ToSingle(defaultValue);
            EnumMax = Enum.GetValues(enumType).Length - 1;
        }

        public string GetDialogName()
            => Dialog.Clean($"gooberhelper_option_{Name}");

        public string GetDialogDescription() {
            var id = $"gooberhelper_option_description_{Name}";

            return Dialog.Has(id)
                ? Dialog.Clean(id)
                : "";
        }
    }
}