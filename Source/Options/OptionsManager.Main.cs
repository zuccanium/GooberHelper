using System;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.GooberHelper.Options {
    //who up reworking they helper
    public static partial class OptionsManager {
        public static readonly Color DefaultColor = Color.White;
        public static readonly Color MapDefinedColor = Color.DeepSkyBlue;
        public static readonly Color UserDefinedEvilColor = new(0.5f,0.5f,1f,0.2f);
        public static readonly Color UserDefinedCoolColor = new(1f,0.5f,0f,0.2f);


        public enum OptionSetter {
            None,
            Map,
            User
        }
        
        public enum OptionType {
            Boolean,
            Integer,
            Float,
            Enum
        }

        public static float GetOptionValue(Option option)
            => GooberHelperModule.Settings.UserDefinedOptions.TryGetValue(option, out var userValue)
                ? userValue
            
            : GooberHelperModule.Session?.MapDefinedOptions.TryGetValue(option, out var mapValue) == true
                ? mapValue
            
            : Options[option].DefaultValue;

        //i would just reference GetOptionValue here but that would be a few extra instructions and im really cautious about performance stuff
        //the compiler would probably inline it but whatever
        public static bool GetOptionBool(Option option)
            => GooberHelperModule.Settings.UserDefinedOptions.TryGetValue(option, out var userValue)
                ? userValue >= 1

            : GooberHelperModule.Session?.MapDefinedOptions.TryGetValue(option, out var mapValue) == true
                ? mapValue >= 1

            : Options[option].DefaultValue == 1;

        //stupid dumb scuffed c# code
        public static T GetOptionEnum<T>(Option option) where T : Enum
            => (T)Enum.ToObject(typeof(T), (int)GetOptionValue(option));

        public static string GetOptionEnumName(Option option)
            => Options[option].EnumType.GetEnumName((int)GetOptionValue(option)) is string enumName
                ? Dialog.Clean($"gooberhelper_enum_{enumName}")
                : "[unknown enum value]";

        public static OptionSetter GetOptionSetter(Option option)
            => GooberHelperModule.Settings.UserDefinedOptions.ContainsKey(option)
                ? OptionSetter.User

            : GooberHelperModule.Session?.MapDefinedOptions.ContainsKey(option) == true
                ? OptionSetter.Map

            : OptionSetter.None;

        public static Color GetOptionColor(Option option)
            => GetOptionSetter(option) switch {
                OptionSetter.User => option == Option.GoldenBlocksAlwaysLoad
                    ? UserDefinedCoolColor
                    : UserDefinedEvilColor,
                OptionSetter.Map => MapDefinedColor,
                _ => DefaultColor
            };

        public static float GetOptionMapDefinedValueOrDefault(Option option)
            => GooberHelperModule.Session?.MapDefinedOptions.TryGetValue(option, out var value) == true
                ? value
                : Options[option].DefaultValue;

        public static string GetEnabledOptionsString() {
            var str = "";

            foreach(var pair in Options) {
                if(GetOptionSetter(pair.Key) == OptionSetter.None)
                    continue;

                str += $"{pair.Value.GetDialogName()}: {(
                    pair.Value.Type == OptionType.Boolean
                        ? GetOptionBool(pair.Key).ToString()

                    : pair.Value.Type == OptionType.Enum || (pair.Value.EnumType != null && GetOptionValue(pair.Key) < 0)
                        ? GetOptionEnumName(pair.Key).ToString()

                    : GetOptionValue(pair.Key).ToString() + pair.Value.Suffix)}\n";
            }

            return str;
        }

        public static bool SetOptionValue(Option option, float value, OptionSetter setter) {
            if(setter == OptionSetter.User) {
                GooberHelperModule.Settings.UserDefinedOptions[option] = value;
                var neutralValue = GooberHelperModule.Session?.MapDefinedOptions.TryGetValue(option, out var v) == true
                    ? v
                    : Options[option].DefaultValue;

                if(value == neutralValue) {
                    GooberHelperModule.Settings.UserDefinedOptions.Remove(option);

                    return true;
                }
            } else if(setter == OptionSetter.Map && GooberHelperModule.Session != null) {
                GooberHelperModule.Session.MapDefinedOptions[option] = value;

                if(value == Options[option].DefaultValue) {
                    GooberHelperModule.Session.MapDefinedOptions.Remove(option);

                    return true;
                }
            }

            return false;
        }

        public static void ResetOptionValue(Option option, OptionSetter setter) {
            if(setter == OptionSetter.User) {
                GooberHelperModule.Settings.UserDefinedOptions.Remove(option);
            } else if(setter == OptionSetter.Map) {
                GooberHelperModule.Session?.MapDefinedOptions.Remove(option);
            }
        }

        public static void ResetCategory(OptionCategory category, OptionSetter setter) {
            if(setter != OptionSetter.User)
                return;

            foreach(var optionData in Categories[category])
                GooberHelperModule.Settings.UserDefinedOptions.Remove(optionData.Id);
        }

        public static Color GetCategoryColor(OptionCategory category) {
            var color = DefaultColor;

            if(!Categories.TryGetValue(category, out var categoryOptions))
                return color;

            foreach(var optionData in categoryOptions) {
                if(GooberHelperModule.Settings.UserDefinedOptions.ContainsKey(optionData.Id)) {
                    if(optionData.Id != Option.GoldenBlocksAlwaysLoad)
                        return UserDefinedEvilColor;

                    color = UserDefinedCoolColor;
                }

                if(GooberHelperModule.Session?.MapDefinedOptions.ContainsKey(optionData.Id) == true && color == DefaultColor)
                    color = MapDefinedColor;
            }

            return color;
        }

        public static void ResetAll(OptionSetter setter) {
            if(setter == OptionSetter.User)
                GooberHelperModule.Settings.UserDefinedOptions.Clear();
        }

        public static bool GetUserEnabledEvilOption()
            => GooberHelperModule.Settings.UserDefinedOptions.Any(a =>
                Options[a.Key].Category != OptionCategory.Visuals &&
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

        private static Dictionary<Option, OptionData> createOptionsFromCategories() {
            var dict = new Dictionary<Option, OptionData>();

            foreach(var pair in Categories) {
                foreach(var option in pair.Value) {
                    dict[option.Id] = option;

                    option.Category = pair.Key;
                }
            }

            return dict;
        }

        public static readonly Dictionary<Option, OptionData> Options = createOptionsFromCategories();
    }
}