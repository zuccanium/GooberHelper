using System.Linq;

namespace Celeste.Mod.GooberHelper.Options {
    public static partial class OptionsManager {
        public static void ResetAll(OptionSetter setter) {
            if(setter == OptionSetter.User)
                GooberHelperModule.Settings.UserDefinedOptions.Clear();
            
            if(setter == OptionSetter.Map)
                GooberHelperModule.Session.MapDefinedOptions.Clear();
        }

        public static bool GetUserEnabledEvilOption()
            => GooberHelperModule.Settings.UserDefinedOptions.Any(a =>
                OptionToInstance[a.Key].Category != OptionCategory.Visuals &&
                a.Key != Option.GoldenBlocksAlwaysLoad &&
                a.Key != Option.ShowActiveOptions
            );

        public static bool GetUserEnabledCoolOption()
            => GooberHelperModule.Settings.UserDefinedOptions.TryGetValue(Option.GoldenBlocksAlwaysLoad, out var value) && value == 1;

        public static Color GetGlobalColor()
            => GetUserEnabledEvilOption()
                ? UserDefinedEvilColor
            
            : GetUserEnabledCoolOption()
                ? UserDefinedCoolColor

            : GooberHelperModule.Session?.MapDefinedOptions.Count > 0
                ? MapDefinedColor

            : DefaultColor;
    }
}