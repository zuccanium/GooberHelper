using System;
using Celeste.Mod.GooberHelper.Options;
using Celeste.Mod.GooberHelper.UI.OptionSliderContent;

namespace Celeste.Mod.GooberHelper.UI {
    public class OptionSlider : TextMenuGooberExt.DynamicOption<float>, IRefreshable {
        public Option Option;
        private AbstractOption optionInstance;
        
        public OptionSlider(Option option) : base() {
            Option = option;
            optionInstance = OptionToInstance[option];
            
            var optionValue = GetOptionValue(option);

            //do this but for enums
            if(optionInstance.Type == OptionType.Boolean)
                optionValue = optionValue >= 1
                    ? 1
                    : 0;
            
            if(optionInstance.Type == OptionType.Enum)
                optionValue = Enum.IsDefined(optionInstance.EnumType, (int)optionValue)
                    ? optionValue
                    : optionInstance.DefaultValue;

            Label = optionInstance.GetName();

            if(optionInstance.Type == OptionType.Boolean)
                AddEnumerable(new BooleanSliderOptions(), optionValue);
        
            else if(optionInstance.EnumType is Type enumType)
                AddEnumerable(new EnumSliderOptions(optionInstance.EnumType), optionValue);
            
            if(optionInstance.Type == OptionType.Float || optionInstance.Type == OptionType.Integer) {
                LeftMin = optionInstance.LeftMin;
                LeftMax = optionInstance.LeftMax;
                RightMin = optionInstance.RightMin;
                RightMax = optionInstance.RightMax;
                Step = optionInstance.Step;
                SkipLeftMax = optionInstance.SkipLeftMax;
                SkipRightMin = optionInstance.SkipRightMin;
                Suffix = optionInstance.Suffix;
                AllowFastMovement = true;
            }

            Current = optionValue;
            RecalculateCachedRightWidth();
            
            UnselectedColor = GetOptionColor(Option);

            OnValueChange += onValueChange;
            OnAltPressed += onAltPressed;

            IncludeWidthInMeasurement = false;
        }

        public override void Added() {
            base.Added();

            var description = optionInstance.GetDescription();

            if(description != "")
                this.AddDescription(MenuManager.CurrentMenu, description);
        }

        private void onValueChange(float value) {
            SetOptionValue(Option, value, OptionSetter.User);

            Refresh();
            MenuManager.RefreshAll();
        }

        private void onAltPressed() {
            ResetOptionValue(Option, OptionSetter.User);

            Audio.Play(SFX.ui_main_button_toggle_on);

            Refresh();
            MenuManager.RefreshAll();
        }

        public void Refresh() {
            UnselectedColor = GetOptionColor(Option);
            Current = GetOptionValue(Option);

            RecalculateCachedRightWidth();
        }
    }
}