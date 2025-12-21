namespace Celeste.Mod.GooberHelper.Options {
    public static partial class OptionsManager {
        public static void ResetCategory(OptionCategory category, OptionSetter setter) {
            if(setter != OptionSetter.User)
                return;

            foreach(var option in CategoryToOptions[category])
                GooberHelperModule.Settings.UserDefinedOptions.Remove(option);
        }

        public static Color GetCategoryColor(OptionCategory category) {
            var color = DefaultColor;

            if(!CategoryToOptions.TryGetValue(category, out var optionInstances))
                return color;

            foreach(var option in optionInstances) {
                if(GooberHelperModule.Settings.UserDefinedOptions.ContainsKey(option)) {
                    if(option != Option.GoldenBlocksAlwaysLoad)
                        return UserDefinedEvilColor;

                    color = UserDefinedCoolColor;
                }

                if(GooberHelperModule.Session?.MapDefinedOptions.ContainsKey(option) == true && color == DefaultColor)
                    color = MapDefinedColor;
            }

            return color;
        }
    }
}