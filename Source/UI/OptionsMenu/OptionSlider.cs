using System;
using Celeste.Mod.GooberHelper.Options;
using Celeste.Mod.GooberHelper.UI.OptionSliderContent;

namespace Celeste.Mod.GooberHelper.UI {
    public class OptionSlider : TextMenuGooberExt.DynamicOption<float>, IRefreshable {
        public Option Option;
        private OptionData optionData;
        
        public OptionSlider(Option option) : base() {
            Option = option;
            optionData = OptionsManager.Options[option];
            
            var optionValue = GetOptionValue(option);

            //do this but for enums
            if(optionData.Type == OptionType.Boolean)
                optionValue = optionValue >= 1
                    ? 1
                    : 0;
            
            if(optionData.Type == OptionType.Enum)
                optionValue = Enum.IsDefined(optionData.EnumType, (int)optionValue)
                    ? optionValue
                    : optionData.DefaultValue;

            Label = optionData.GetDialogName();

            if(optionData.Type == OptionType.Boolean)
                AddEnumerable(new BooleanSliderOptions(), optionValue);
        
            else if(optionData.EnumType is Type enumType)
                AddEnumerable(new EnumSliderOptions(optionData.EnumType), optionValue);
            
            if(optionData.Type == OptionType.Float || optionData.Type == OptionType.Integer) {
                LeftMin = optionData.LeftMin;
                LeftMax = optionData.LeftMax;
                RightMin = optionData.RightMin;
                RightMax = optionData.RightMax;
                Step = optionData.Step;
                SkipLeftMax = optionData.SkipLeftMax;
                SkipRightMin = optionData.SkipRightMin;
                Suffix = optionData.Suffix;
                AllowFastMovement = true;
            }

            Current = optionValue;
            RecalculateCachedRightWidth();
            
            UnselectedColor = GetOptionColor(optionData.Id);

            OnValueChange += onValueChange;
            OnAltPressed += onAltPressed;

            IncludeWidthInMeasurement = false;
        }

        public override void Added() {
            base.Added();

            var description = optionData.GetDialogDescription();

            if(description != "")
                this.AddDescription(MenuManager.CurrentMenu, description);
        }

        private void onValueChange(float value) {
            SetOptionValue(optionData.Id, value, OptionSetter.User);

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