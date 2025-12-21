using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Celeste.Mod.GooberHelper.Options {
    public static partial class OptionsManager {
        public static bool TryParseOptionValue(Option option, string value, out float result, out List<string> possibleEnumKeys) {
            possibleEnumKeys = [];

            var optionData = OptionToInstance[option];
            var enumType = optionData.EnumType;
            var underlyingEnumType = Enum.GetUnderlyingType(enumType);

            if(float.TryParse(value, out var floatValue)) {
                if(Enum.GetName(optionData.EnumType, Convert.ChangeType(floatValue, underlyingEnumType)) is string enumKeyFromFloat)
                    possibleEnumKeys.Add(enumKeyFromFloat);

                result = floatValue;
                
                return true;
            }

            if(Enum.TryParse(enumType, value, ignoreCase: true, out var parsedEnumValue)) {
                possibleEnumKeys.Add(Enum.GetName(enumType, parsedEnumValue));
                result = Convert.ToSingle(parsedEnumValue);

                return true;
            }

            foreach(var name in Enum.GetNames(enumType)) {
                var dialogName = Dialog.Clean($"gooberhelper_enum_{name}");

                if(
                    dialogName.Contains(value, StringComparison.InvariantCultureIgnoreCase) ||
                    value.Contains(dialogName, StringComparison.InvariantCultureIgnoreCase) ||

                    name.Contains(value, StringComparison.InvariantCultureIgnoreCase) ||
                    value.Contains(name, StringComparison.InvariantCultureIgnoreCase)
                ) {
                    possibleEnumKeys.Add(name);
                }
            }

            if(possibleEnumKeys.FirstOrDefault() is string enumKey) {
                result = Convert.ToSingle(Enum.Parse(enumType, enumKey));

                return true;
            }

            result = 0f;

            return false;
        }

        public static void ResetOptionValueFromString(string optionNameString, OptionSetter setter) {
            if(!Enum.TryParse<Option>(optionNameString, true, out var option)) {
                Engine.Commands.Log($"[GooberHelper] {optionNameString} is not a valid option!");

                return;
            }

            ResetOptionValue(option, setter);
        }

        public static void SetOptionValueFromStrings(string optionNameString, string optionValueString, OptionSetter setter) {
            if(!Enum.TryParse<Option>(optionNameString, true, out var option)) {
                Engine.Commands.Log($"[GooberHelper] {optionNameString} is not a valid option!");

                return;
            }

            if(optionNameString is null) {
                Engine.Commands.Log($"[GooberHelper] please provide an argument for the option name");

                return;
            }

            if(optionValueString is null) {
                Engine.Commands.Log($"[GooberHelper] please provide an argument for the option value");

                return;
            }

            var optionEnum = OptionToInstance[option].EnumType;

            void logEnumKeys() {
                var builder = new StringBuilder($"[GooberHelper] the valid enum keys for {option} are\n");

                foreach(var value in Enum.GetValues(optionEnum))
                    builder.Append($"- {value} ({Convert.ToSingle(value)})\n");

                builder.Length--;

                Engine.Commands.Log(builder);
            }
            
            if(TryParseOptionValue(option, optionValueString, out var parsedOptionValue, out var possibleEnumKeys)) {
                if(possibleEnumKeys.Count > 1) {
                    Engine.Commands.Log($"[GooberHelper] found ambiguity between {Utils.JoinList(possibleEnumKeys)} when trying to parse {optionValueString}!!!");
                    
                    logEnumKeys();
                }
                
                SetOptionValue(option, parsedOptionValue, setter);

                Engine.Commands.Log($"[GooberHelper] set {option} to {parsedOptionValue}" + (possibleEnumKeys.Count > 0 ? $" ({possibleEnumKeys.First()})" : ""));
            } else {
                Engine.Commands.Log($"[GooberHelper] couldnt parse {optionValueString}!!!");
                    
                logEnumKeys();
            }
        }
        
        //setters
        [Command("set_gooberhelper_option_map", "sets a gooberhelper option value on the map level (session)")]
        public static void CmdSetGooberhelperOptionMap(string optionNameString, string optionValueString)
            => SetOptionValueFromStrings(optionNameString, optionValueString, OptionSetter.Map);

        //holy ugly
        [Command("set_gooberhelper_option_user", "sets a gooberhelper option value on the user level (settings)")]
        public static void CmdSetGooberhelperOptionUser(string optionNameString, string optionValueString)
            => SetOptionValueFromStrings(optionNameString, optionValueString, OptionSetter.User);

        //resetters
        [Command("reset_gooberhelper_option_map", "resets a gooberhelper option value on the map level (session)")]
        public static void CmdResetGooberhelperOptionMap(string optionNameString)
            => ResetOptionValueFromString(optionNameString, OptionSetter.Map);

        //the comments are only for separation
        [Command("reset_gooberhelper_option_user", "resets a gooberhelper option value on the user level (settings)")]
        public static void CmdResetGooberhelperOptionUser(string optionNameString)
            => ResetOptionValueFromString(optionNameString, OptionSetter.User);
    }
}