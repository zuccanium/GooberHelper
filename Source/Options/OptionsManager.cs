using System;
using System.Collections.Generic;
using Celeste.Mod.GooberHelper.Attributes;

//the order within categories is
//- speed preservation
//- new thing
//- allowing things that are prevented in vanilla
//these subcategories are sorted roughly by creation order or however i want 😭
//important things can be pinned to the top

//important terminology definitions:
//preservation = it preserves speed
//inversion = it preserves speed AND the player can decide which direction to go

//goodbye buhbu 💗 i will love you forever
// new OptionData(Option.buhbu, OptionType.Float, 0) { min = 0, max = 10, growthFactor = 10, suffix = " frames" },
// new OptionData(Option.zonmgle),
// new OptionData(Option.zingle)

namespace Celeste.Mod.GooberHelper.Options {
    //who up reworking they helper
    public static partial class OptionsManager {
        public static readonly Color DefaultColor = Color.White;
        public static readonly Color MapDefinedColor = Color.DeepSkyBlue;
        public static readonly Color UserDefinedEvilColor = new(0.5f, 0.5f, 1f, 0.2f);
        public static readonly Color UserDefinedCoolColor = new(1f, 0.5f, 0f, 0.2f);
        
        public const int ReservedHybridEnumConstant = -899405; //thank you sparky

        public static readonly Dictionary<Option, AbstractOption> OptionToInstance = [];
        public static readonly Dictionary<OptionCategory, List<Option>> CategoryToOptions = [];

        public enum OptionSetter {
            None,
            Map,
            User
        }
        
        public enum OptionType {
            Boolean,
            Float,
            Enum
        }

        public enum OptionCategory {
            Jumping,
            Dashing,
            Moving,
            Entities,
            Other,
            Visuals,
            Miscellaneous,
            General
        }

        public enum OptionGroup {
            None,
            Special,
            SpeedPreservation,
            NewThings,
            AllowingThings
        }

        [OnLoad]
        public static void Load() {
            foreach(var type in typeof(GooberHelperModule).Assembly.GetTypes()) {
                if(!type.IsDefined(typeof(GooberHelperOptionAttribute), false))
                    continue;
                
                var instance = Activator.CreateInstance(type);

                if(instance is not AbstractOption optionInstance) {
                    Utils.Log("somehow not an options instance??? what???");

                    continue;
                }

                optionInstance.InferData();

                Utils.Log($"{optionInstance.Option} is of category {optionInstance.Category}");

                //normal option stuff
                OptionToInstance[optionInstance.Option] = optionInstance;

                //category stuff                
                if(!CategoryToOptions.TryGetValue(optionInstance.Category, out var categoryOptions)) {
                    categoryOptions = [];

                    CategoryToOptions[optionInstance.Category] = categoryOptions;
                }

                categoryOptions.Add(optionInstance.Option);
            }

            if(OptionToInstance.Count != Enum.GetValues<Option>().Length) {
                Utils.Log("heyyyy uhhh i think you forgot an option or two");
                
                foreach(var option in Enum.GetValues<Option>())
                    if(!OptionToInstance.TryGetValue(option, out var _))
                        Utils.Log($"couldnt find {option} in the dictionary!");
            }

            //theyre sorted by however c# sorts the types iterator in an assembly for now
            //they should be sorted by option enum value
            foreach(var categoryOptions in CategoryToOptions.Values)
                categoryOptions.Sort();
        }

        [OnUnload]
        public static void Unload() {
            OptionToInstance.Clear();
            CategoryToOptions.Clear();
        }
    }
}