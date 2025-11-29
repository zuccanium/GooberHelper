using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Celeste.Mod.GooberHelper.Options {
    //who up reworking they helper
    public static class OptionsManager {
        public static readonly Color DefaultColor = Color.White;
        public static readonly Color MapDefinedColor = Color.DeepSkyBlue;
        public static readonly Color UserDefinedEvilColor = new(0.5f,0.5f,1f,0.2f);
        public static readonly Color UserDefinedCoolColor = new(1f,0.5f,0f,0.2f);

        public class OptionData {
            public Option Id;
            public string Name;
            public OptionType Type;
            public string Category;
            public float DefaultValue;
            public float Min = 0;
            public float Max = 100;
            public float Step = 1;
            public bool ExponentialIncrease = true;
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
                var id = $"gooberhelper_option_description_{this.Name}";

                return Dialog.Has(id)
                    ? Dialog.Clean(id)
                    : "";
            }
        }

        public class OptionsProfile {
            public string Name;
            public Dictionary<Option, float> UserDefinedOptions;

            //this is required for modsettings to work
            //it will throw "Cannot dynamically create an instance of type '[this one]'. Reason: No parameterless constructor defined"
            public OptionsProfile() {}

            public OptionsProfile(string name, Dictionary<Option, float> userDefinedOptions) {
                Name = name;
                UserDefinedOptions = userDefinedOptions;
            }

            public static void Create(string name) {
                GooberHelperModule.Settings.OptionsProfiles[name] = new OptionsProfile(name, GooberHelperModule.Settings.UserDefinedOptions.ToDictionary());
                GooberHelperModule.Settings.OptionsProfileOrder.Add(name);
            }

            public static OptionsProfile CreateFromImport() {
                var deserializedProfile = Deserialize(TextInput.GetClipboardText());

                deserializedProfile.Name = Utils.PreventNameCollision(deserializedProfile.Name, GooberHelperModule.Settings.OptionsProfiles);

                GooberHelperModule.Settings.OptionsProfiles[deserializedProfile.Name] = deserializedProfile;
                GooberHelperModule.Settings.OptionsProfileOrder.Add(deserializedProfile.Name);

                return deserializedProfile;
            }

            public static void Load(string name)
                => GooberHelperModule.Settings.UserDefinedOptions = GooberHelperModule.Settings.OptionsProfiles[name].UserDefinedOptions.ToDictionary();

            public static void Save(string name)
                => GooberHelperModule.Settings.OptionsProfiles[name].UserDefinedOptions = GooberHelperModule.Settings.UserDefinedOptions.ToDictionary();

            public static void Rename(string from, string to) {
                if(from == to) return;

                GooberHelperModule.Settings.OptionsProfiles[to] = GooberHelperModule.Settings.OptionsProfiles[from];
                GooberHelperModule.Settings.OptionsProfiles.Remove(from);

                GooberHelperModule.Settings.OptionsProfiles[to].Name = to;

                GooberHelperModule.Settings.OptionsProfileOrder[GooberHelperModule.Settings.OptionsProfileOrder.IndexOf(from)] = to;
            }

            public static OptionsProfile Duplicate(string name, out int insertionIndex) {
                var duplicate = new OptionsProfile(
                    name: Utils.CreateCopyName(name, GooberHelperModule.Settings.OptionsProfiles, out var lastNameCollision),
                    userDefinedOptions: GooberHelperModule.Settings.OptionsProfiles[name].UserDefinedOptions.ToDictionary()
                );

                GooberHelperModule.Settings.OptionsProfiles[duplicate.Name] = duplicate;

                insertionIndex = GooberHelperModule.Settings.OptionsProfileOrder.IndexOf(lastNameCollision) + 1;
                GooberHelperModule.Settings.OptionsProfileOrder.Insert(insertionIndex, duplicate.Name);

                return duplicate;
            }

            public static void Export(string name)
                => TextInput.SetClipboardText(GooberHelperModule.Settings.OptionsProfiles[name].Serialize());

            public static OptionsProfile Import(string name) {
                var deserializedProfile = Deserialize(TextInput.GetClipboardText());

                GooberHelperModule.Settings.OptionsProfiles[name].UserDefinedOptions = deserializedProfile.UserDefinedOptions;

                return deserializedProfile;
            }

            public static void Delete(string name) {
                GooberHelperModule.Settings.OptionsProfiles.Remove(name);
                GooberHelperModule.Settings.OptionsProfileOrder.Remove(name);
            }

            //this oop stuff is getting ridiculous
            public static bool GetExists(string name)
                => GooberHelperModule.Settings.OptionsProfiles.ContainsKey(name);

            public string Serialize() {
                List<byte> data = [];
                var nameBytes = Encoding.UTF8.GetBytes(Name);

                for(var i = 0; i < nameBytes.Length; i++) {
                    data.Add(nameBytes[i]);
                }

                data.Add(0); //null termination

                //i shouldve just used a binary writer for this but it works and i dont want to redo it
                //i didnt know those existed when writing this lmao
                foreach(var pair in UserDefinedOptions) {
                    var keyBytes = BitConverter.GetBytes((ushort)pair.Key);
                    var valueBytes = BitConverter.GetBytes(pair.Value);
                    
                    data.Add(keyBytes[0]);
                    data.Add(keyBytes[1]);
                    data.Add(valueBytes[0]);
                    data.Add(valueBytes[1]);
                    data.Add(valueBytes[2]);
                    data.Add(valueBytes[3]);
                }

                using(var compressedStream = new MemoryStream()) {
                    using(var gzipStream = new GZipStream(compressedStream, CompressionLevel.Fastest)) {
                        gzipStream.Write(data.ToArray());
                        gzipStream.Close();

                        return Convert.ToBase64String(compressedStream.ToArray());
                    }
                }
            }

            public static OptionsProfile Deserialize(string str) {
                OptionsProfile profile = new("", new Dictionary<Option, float>());
                byte[] data;

                using(var compressedStream = new MemoryStream(Convert.FromBase64String(str))) {
                    using(var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress)) {
                        using(var resultsStream = new MemoryStream()) {
                            gzipStream.CopyTo(resultsStream);
                            gzipStream.Close();

                            data = resultsStream.ToArray();
                        }
                    }
                }

                var stringLength = Array.IndexOf(data, (byte)0);

                if(stringLength == -1) {
                    throw new Exception("couldnt find the null termination of the options profile's name");
                }

                profile.Name = Encoding.UTF8.GetString(data, 0, stringLength);

                for(var i = stringLength + 1; i < data.Length; i += 6) {
                    var key = (Option)BitConverter.ToUInt16(data, i);
                    var value = BitConverter.ToSingle(data, i + 2);

                    profile.UserDefinedOptions[key] = value;
                }

                return profile;
            }
        }

        public class OptionChanges {
            public Dictionary<Option, float> Enable;
            public Dictionary<Option, float> Disable;
            public bool ResetAll;
            public EntityID ID;

            public static readonly Regex ParsingRegex = new(@"(?<key>[A-Z|a-z]+)($|:(\s+)?(?<value>[-\w\.]+))");

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
                            } else if(Options[option].EnumType != null) {
                                if(Enum.TryParse(Options[option].EnumType, valueString, true, out var enumValue)) {
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

        public static string GetOptionEnumName(Option option) {
            var type = Options[option].EnumType;
            var value = MathF.Floor(Math.Max(GetOptionValue(option), -Options[option].EnumMax));

            return Dialog.Clean($"gooberhelper_enum_{type.GetEnumName((int)(value > Options[option].Max ? 0 : value))}");
        }

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

        public static void ResetCategory(string category, OptionSetter setter) {
            if(setter != OptionSetter.User)
                return;

            foreach(var optionData in Categories[category])
                GooberHelperModule.Settings.UserDefinedOptions.Remove(optionData.Id);
        }

        public static Color GetCategoryColor(string category) {
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
                Options[a.Key].Category != "Visuals" &&
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
        
        public static bool TryParseOptionValue(Option option, string value, out float result, out List<string> possibleEnumKeys) {
            possibleEnumKeys = [];

            var optionData = Options[option];
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

            var optionEnum = Options[option].EnumType;

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

        public enum Option {
            //jumping
            JumpInversion,
            WalljumpSpeedPreservation,
            WallbounceSpeedPreservation,
            HyperAndSuperSpeedPreservation,
            UpwardsJumpSpeedPreservationThreshold,
            DownwardsJumpSpeedPreservationThreshold,
            BounceHelperBounceSpeedPreservation,

            GetClimbjumpSpeedInRetention,
            AdditiveVerticalJumpSpeed,
            SwapHorizontalAndVerticalSpeedOnWalljump,
            VerticalToHorizontalSpeedOnGroundJump,
            CornerboostBlocksEverywhere,

            AllDirectionHypersAndSupers,
            AllowUpwardsCoyote,
            AllDirectionDreamJumps,
            AllowHoldableClimbjumping,

            //dashing
            VerticalDashSpeedPreservation,
            ReverseDashSpeedPreservation,

            MagnitudeBasedDashSpeed,

            DashesDontResetSpeed,
            KeepDashAttackOnCollision,
            DownDemoDashing,

            //moving
            CobwobSpeedInversion,

            WallboostDirectionIsOppositeSpeed,
            WallboostSpeedIsOppositeSpeed,
            HorizontalTurningSpeedInversion,
            VerticalTurningSpeedInversion,
            DownwardsAirFrictionBehavior,

            UpwardsTransitionSpeedPreservation,

            //other
            RefillFreezeLength,
            RetentionLength,

            ConserveBeforeDashSpeed,
            DreamBlockSpeedPreservation,
            SpringSpeedPreservation,
            ReboundSpeedPreservation,
            ExplodeLaunchSpeedPreservation,
            PickupSpeedInversion,
            BubbleSpeedPreservation,
            FeatherEndSpeedPreservation,
            BadelineBossSpeedPreservation,

            CustomFeathers,
            CustomSwimming,
            RemoveNormalEnd,
            LenientStunning,
            HoldablesInheritSpeedWhenThrown,
            FastFallHitboxSquish,

            AllowCrouchedHoldableGrabbing,
            AllowUpwardsClimbGrabbing,
            AllowCrouchedClimbGrabbing,
            ClimbingSpeedPreservation,
            AllowClimbingInDashState,
            CoreBlockAllDirectionActivation,
            AllowWindWhileDashing,
            LiftboostAdditionHorizontal,
            LiftboostAdditionVertical,
            AdvantageousLiftBoost,
            ReverseBackboosts,

            //visuals
            PlayerShaderMask,
            TheoNuclearReactor,

            //miscellaneous
            AlwaysExplodeSpinners,
            GoldenBlocksAlwaysLoad,
            RefillFreezeGameSuspension,
            BufferDelayVisualization,
            Ant,

            //general
            ShowActiveOptions,
        }

        public enum JumpInversionValue {
            None,
            GroundJumps,
            All
        }

        public enum WalljumpSpeedPreservationValue {
            None,
            FakeRCB,
            Preserve,
            Invert,
        }

        public enum VerticalJumpSpeedPreservationHybridValue {
            None = -1,
            DashSpeed = -2,
        }

        public enum AllDirectionHypersAndSupersValue {
            None,
            RequireGround,
            WorkWithCoyoteTimeAndDontRefill,
            WorkWithCoyoteTime
        }

        public enum VerticalToHorizontalSpeedOnGroundJumpValue {
            None,
            Vertical,
            Magnitude
        }

        public enum MagnitudeBasedDashSpeedValue {
            None,
            OnlyCardinal,
            All
        }

        public enum CobwobSpeedInversionValue {
            None,
            RequireSpeed,
            WorkWithRetention
        }

        public enum DreamBlockSpeedPreservationValue {
            None,
            Horizontal,
            Vertical,
            Both,
            Magnitude,
        }

        public enum SpringSpeedPreservationValue {
            None,
            Preserve,
            Invert
        }
        
        public enum CustomFeathersValue {
            None,
            KeepIntro,
            SkipIntro
        }

        public enum PlayerShaderMaskValue {
            None,
            HairOnly,
            Cover,
        }

        public enum DashesDontResetSpeedValue {
            None,
            Legacy,
            On,
        }

        public enum AllowWindWhileDashingValue {
            None,
            Velocity,
            Speed,
        }

        public enum ExplodeLaunchSpeedPreservationValue {
            None,
            Horizontal,
            Vertical,
            Both,
            Magnitude,
        }

        public enum AllowUpwardsClimbGrabbingValue {
            None,
            WhileHoldingUp,
            Always
        }

        //the order within categories is
        //- speed preservation
        //- new thing
        //- allowing things that are prevented in vanilla
        //these subcategories are sorted roughly by creation order or however i want 😭
        //important things can be pinned to the top
        
        //important terminology definitions:
        //preservation = it preserves speed
        //inversion -> it preserves speed AND the player can decide which direction to go 

        public static readonly Dictionary<string, List<OptionData>> Categories = new() {
            { "Jumping", [
                //goodbye buhbu 💗 i will love you forever
                // new OptionData(Option.buhbu, OptionType.Float, 0) { min = 0, max = 10, growthFactor = 10, suffix = " frames" },
                // new OptionData(Option.zonmgle),
                // new OptionData(Option.zingle)
                new OptionData(Option.JumpInversion, typeof(JumpInversionValue), JumpInversionValue.None),
                new OptionData(Option.WalljumpSpeedPreservation, typeof(WalljumpSpeedPreservationValue), WalljumpSpeedPreservationValue.None),
                new OptionData(Option.WallbounceSpeedPreservation),
                new OptionData(Option.HyperAndSuperSpeedPreservation),
                new OptionData(Option.UpwardsJumpSpeedPreservationThreshold, typeof(VerticalJumpSpeedPreservationHybridValue), OptionType.Integer, -1) { Max = 240, Step = 10, ExponentialIncrease = false, Suffix = "px/s" },
                new OptionData(Option.DownwardsJumpSpeedPreservationThreshold, typeof(VerticalJumpSpeedPreservationHybridValue), OptionType.Integer, -1) { Max = 240, Step = 10, ExponentialIncrease = false, Suffix = "px/s" },
                new OptionData(Option.BounceHelperBounceSpeedPreservation),

                new OptionData(Option.GetClimbjumpSpeedInRetention),
                new OptionData(Option.AdditiveVerticalJumpSpeed),
                new OptionData(Option.SwapHorizontalAndVerticalSpeedOnWalljump),
                new OptionData(Option.VerticalToHorizontalSpeedOnGroundJump, typeof(VerticalToHorizontalSpeedOnGroundJumpValue), VerticalToHorizontalSpeedOnGroundJumpValue.None),
                new OptionData(Option.CornerboostBlocksEverywhere),
                
                new OptionData(Option.AllDirectionHypersAndSupers, typeof(AllDirectionHypersAndSupersValue), AllDirectionHypersAndSupersValue.None),
                new OptionData(Option.AllowUpwardsCoyote),
                new OptionData(Option.AllDirectionDreamJumps),
                new OptionData(Option.AllowHoldableClimbjumping),
            ]},
            { "Dashing", [
                new OptionData(Option.VerticalDashSpeedPreservation),
                new OptionData(Option.ReverseDashSpeedPreservation),

                new OptionData(Option.MagnitudeBasedDashSpeed, typeof(MagnitudeBasedDashSpeedValue), MagnitudeBasedDashSpeedValue.None),
                
                new OptionData(Option.DashesDontResetSpeed, typeof(DashesDontResetSpeedValue), DashesDontResetSpeedValue.None),
                new OptionData(Option.KeepDashAttackOnCollision),
                new OptionData(Option.DownDemoDashing),
            ]},
            { "Moving", [
                new OptionData(Option.CobwobSpeedInversion, typeof(CobwobSpeedInversionValue), CobwobSpeedInversionValue.None),
                
                new OptionData(Option.WallboostDirectionIsOppositeSpeed),
                new OptionData(Option.WallboostSpeedIsOppositeSpeed),
                new OptionData(Option.HorizontalTurningSpeedInversion),
                new OptionData(Option.VerticalTurningSpeedInversion),
                new OptionData(Option.DownwardsAirFrictionBehavior),

                new OptionData(Option.UpwardsTransitionSpeedPreservation),
            ]},
            { "Other", [
                new OptionData(Option.RefillFreezeLength, OptionType.Float, 3) { Min = 0, Max = 10000, Step = 1, Suffix = "f", ExponentialIncrease = true },
                new OptionData(Option.RetentionLength, OptionType.Float, 4) { Min = 0, Max = 10000, Step = 1, Suffix = "f", ExponentialIncrease = true },
                
                new OptionData(Option.ConserveBeforeDashSpeed),
                new OptionData(Option.DreamBlockSpeedPreservation, typeof(DreamBlockSpeedPreservationValue), DreamBlockSpeedPreservationValue.None),
                new OptionData(Option.SpringSpeedPreservation, typeof(SpringSpeedPreservationValue), SpringSpeedPreservationValue.None),
                new OptionData(Option.ReboundSpeedPreservation),
                new OptionData(Option.ExplodeLaunchSpeedPreservation, typeof(ExplodeLaunchSpeedPreservationValue), ExplodeLaunchSpeedPreservationValue.None),
                new OptionData(Option.PickupSpeedInversion),
                new OptionData(Option.BubbleSpeedPreservation),
                new OptionData(Option.FeatherEndSpeedPreservation),
                new OptionData(Option.BadelineBossSpeedPreservation),

                new OptionData(Option.CustomFeathers, typeof(CustomFeathersValue), CustomFeathersValue.None),
                new OptionData(Option.CustomSwimming),
                new OptionData(Option.RemoveNormalEnd),
                new OptionData(Option.LenientStunning),
                new OptionData(Option.HoldablesInheritSpeedWhenThrown),
                new OptionData(Option.FastFallHitboxSquish, OptionType.Float, 0) { Min = 0, Max = 100, Step = 5, Suffix = "%" },

                new OptionData(Option.AllowCrouchedHoldableGrabbing),
                new OptionData(Option.AllowUpwardsClimbGrabbing, typeof(AllowUpwardsClimbGrabbingValue), AllowUpwardsClimbGrabbingValue.None),
                new OptionData(Option.AllowCrouchedClimbGrabbing),
                new OptionData(Option.ClimbingSpeedPreservation),
                new OptionData(Option.AllowClimbingInDashState),
                new OptionData(Option.CoreBlockAllDirectionActivation),
                new OptionData(Option.AllowWindWhileDashing, typeof(AllowWindWhileDashingValue), AllowWindWhileDashingValue.None),
                new OptionData(Option.LiftboostAdditionHorizontal, OptionType.Float, 0) { Min = -10000, Max = 10000, Step = 5, Suffix = "px/s", ExponentialIncrease = true },
                new OptionData(Option.LiftboostAdditionVertical, OptionType.Float, 0) { Min = -10000, Max = 10000, Step = 5, Suffix = "px/s", ExponentialIncrease = true },
                new OptionData(Option.AdvantageousLiftBoost),
                new OptionData(Option.ReverseBackboosts),
            ]},
            { "Visuals", [
                new OptionData(Option.PlayerShaderMask, typeof(PlayerShaderMaskValue), PlayerShaderMaskValue.None),
                new OptionData(Option.TheoNuclearReactor),
            ]},
            { "Miscellaneous", [
                new OptionData(Option.AlwaysExplodeSpinners),
                new OptionData(Option.GoldenBlocksAlwaysLoad),
                new OptionData(Option.RefillFreezeGameSuspension),
                new OptionData(Option.BufferDelayVisualization),
                new OptionData(Option.Ant),
            ]},
            { "General", [
                new OptionData(Option.ShowActiveOptions),
            ]},
        };

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