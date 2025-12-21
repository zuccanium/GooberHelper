using System;
using System.Linq;

namespace Celeste.Mod.GooberHelper.Options {
    public abstract class AbstractOption {
        //important ones
        public virtual Option Option { get; set; }
        public virtual OptionType Type { get; set; } = OptionType.Boolean;
        public virtual float DefaultValue { get; set; } = 0f;
        public virtual OptionCategory Category { get; set; }

        public virtual Type EnumType { get; set; }
        
        public virtual float? LeftMin { get; set; }
        public virtual float? LeftMax { get; set; }
        public virtual float? RightMin { get; set; }
        public virtual float? RightMax { get; set; }
        public virtual bool SkipLeftMax { get; set; } = false;
        public virtual bool SkipRightMin { get; set; } = false;
        public virtual float Step { get; set; } = 1;
        public virtual string Suffix { get; set; }

        protected virtual void InferOption() {
            if(!Enum.TryParse<Option>(GetType().Name, out var parsedOption)) {
                Utils.Log($"couldnt parse {GetType()} as an option :/ ?");

                return;                
            }

            Option = parsedOption;
        }

        protected virtual void InferCategory() {
            var namespacePrefix = typeof(AbstractOption).Namespace;
            var categoryString = GetType().Namespace[namespacePrefix.Length..]
                .Split(".")
                .Last();

            if(!Enum.TryParse<OptionCategory>(categoryString, out var parsedCategory)) {
                Utils.Log($"couldnt figure out the category of {GetType()}");

                return;
            }

            Category = parsedCategory;
        }

        protected virtual void InferEnumType() {
            if(EnumType != null)
                return;
            
            foreach(var maybeEnumValueType in GetType().GetNestedTypes()) {
                if(maybeEnumValueType.Name.EndsWith("Value")) {
                    EnumType = maybeEnumValueType;

                    break;
                }
            }
        }

        protected virtual void InferType() {
            if(Type != OptionType.Boolean)
                return;

            if(EnumType != null)
                Type = OptionType.Enum;
        }

        public virtual void InferData() {
            InferOption();
            InferCategory();
            InferEnumType();
            InferType();
        }

        public virtual string GetName()
            => Dialog.Clean($"gooberhelper_option_{GetType().Name}");

        public virtual string GetDescription() {
            var id = $"gooberhelper_option_description_{GetType().Name}";

            return Dialog.Has(id)
                ? Dialog.Clean(id)
                : "";
        }
    }
}