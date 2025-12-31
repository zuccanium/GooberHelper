using System;

namespace Celeste.Mod.GooberHelper.Options {
    public static partial class OptionsManager {
        public static float GetOptionValue(Option option)
            => GooberHelperModule.Settings.UserDefinedOptions.TryGetValue(option, out var userValue)
                ? userValue
            
            : GooberHelperModule.Session?.MapDefinedOptions.TryGetValue(option, out var mapValue) == true
                ? mapValue
            
            : OptionToInstance[option].DefaultValue;

        //i would just reference GetOptionValue here but that would be a few extra instructions and im really cautious about performance stuff
        //the compiler would probably inline it but whatever
        public static bool GetOptionBool(Option option)
            => GooberHelperModule.Settings.UserDefinedOptions.TryGetValue(option, out var userValue)
                ? userValue >= 1

            : GooberHelperModule.Session?.MapDefinedOptions.TryGetValue(option, out var mapValue) == true
                ? mapValue >= 1

            : OptionToInstance[option].DefaultValue == 1;

        //stupid dumb scuffed c# code
        public static T GetOptionEnum<T>(Option option) where T : Enum
            => (T)Enum.ToObject(typeof(T), (int)GetOptionValue(option));

        public static string GetOptionEnumName(Option option)
            => OptionToInstance[option].EnumType.GetEnumName((int)GetOptionValue(option)) is string enumName
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
                : OptionToInstance[option].DefaultValue;

        public static string GetEnabledOptionsString() {
            var str = "";

            foreach(var pair in OptionToInstance) {
                if(GetOptionSetter(pair.Key) == OptionSetter.None)
                    continue;

                str += $"{pair.Value.GetName()}: {(
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
                    : OptionToInstance[option].DefaultValue;

                if(value == neutralValue) {
                    GooberHelperModule.Settings.UserDefinedOptions.Remove(option);

                    return true;
                }
            } else if(setter == OptionSetter.Map && GooberHelperModule.Session != null) {
                GooberHelperModule.Session.MapDefinedOptions[option] = value;

                if(value == OptionToInstance[option].DefaultValue) {
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
    }
}