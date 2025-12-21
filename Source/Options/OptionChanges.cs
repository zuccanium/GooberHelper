using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Celeste.Mod.GooberHelper.Options {
    public class OptionChanges {
        public Dictionary<Option, float> Enable;
        public Dictionary<Option, float> Disable;
        public bool ResetAll;
        public EntityID ID;

        public static readonly Regex ParsingRegex = new(@"(?<key>[A-Z|a-z]+)($|:(\s+)?(?<value>[-\w\.]+))");

        //exists for the same reason as the default constructor for OptionsProfile.cs
        public OptionChanges() {}

        public OptionChanges(EntityData data) {
            ID = new EntityID(data.Level.Name, data.ID);

            Enable = ParseOptionsString(data.Attr("enable"));
            Disable = ParseOptionsString(data.Attr("disable"));
            ResetAll = data.Bool("resetAll");
        }

        public static Dictionary<Option, float> ParseOptionsString(string str) {
            var options = new Dictionary<Option, float>();

            if(str.Length == 0) return options;

            foreach(var assignment in str.Split(",")) {
                var match = ParsingRegex.Match(assignment);

                if(match.Success) {
                    if(!match.Groups.TryGetValue("key", out var keyGroup))
                        Logger.Warn("GooberHelper", $"Weird assignment \"{assignment}\"");

                    if(!Enum.TryParse(keyGroup.Value, false, out Option option))
                        Logger.Warn("GooberHelper", $"Failed to parse {keyGroup.Value} as an option name!");

                    float value = 1;

                    if(match.Groups.TryGetValue("value", out var valueGroup) && valueGroup.Success) {
                        var valueString = valueGroup.Value;

                        if(float.TryParse(valueString, out var floatValue)) {
                            value = floatValue;
                        } else if(OptionsManager.OptionToInstance[option].EnumType != null) {
                            if(Enum.TryParse(OptionsManager.OptionToInstance[option].EnumType, valueString, true, out var enumValue)) {
                                value = (int)enumValue;
                            } else {
                                Logger.Warn("GooberHelper", $"Failed to parse {valueString} as an option enum value!");
                            }
                        }
                    }

                    options[option] = value;
                }
            }

            return options;
        }

        public void Apply() {
            if(ResetAll) {
                ResetAll(OptionSetter.Map);
            } else {
                foreach(var pair in Disable)
                    ResetOptionValue(pair.Key, OptionSetter.Map);
            }

            foreach(var pair in Enable)
                SetOptionValue(pair.Key, pair.Value, OptionSetter.Map);
        }

        public static void UpdateStack() {
            GooberHelperModule.Session.MapDefinedOptions.Clear();

            foreach(var changes in GooberHelperModule.Session.Stack)
                changes.Apply();
        }
    }
}