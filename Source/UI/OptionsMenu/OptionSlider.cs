using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.GooberHelper.Options;
using Celeste.Mod.GooberHelper.UI.OptionSliderContent;

namespace Celeste.Mod.GooberHelper.UI {
    public class OptionSlider : TextMenuExt.EnumerableSlider<float> {
        public Option Option;
        private OptionData optionData;
        
        //the base call is garbage data on purpose
        //fill it manually later
        public OptionSlider(Option option) : base("", new List<float>(), 0) {
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

            //i swear on my life i will never use explicit typing for variables ever again
            //i know this is stupid as hell but i will not let one of those fuckers slip into my precious codebase ever again
            //sorry for the rude language
            //i have absolutely no idea whos gonna be reading this if anyone, but if youre my employer or something, my bad lmao
            var sliderOptions = default(IEnumerable<KeyValuePair<float, string>>);

            sliderOptions =
                optionData.Type == OptionType.Boolean
                    ? new BooleanSliderOptions()
                
                : optionData.Type == OptionType.Enum
                    ? new EnumSliderOptions(optionData.EnumType)

                : new NumericSliderOptions(optionData);

            var caughtOptionValue = false;

            foreach(var sliderOptionPair in sliderOptions) {
                if(!caughtOptionValue && sliderOptionPair.Key > optionValue)
                    Add(optionValue + optionData.Suffix, optionValue, caughtOptionValue |= true);

                var equal = sliderOptionPair.Key == optionValue;
                caughtOptionValue |= equal;

                Add(sliderOptionPair.Value, sliderOptionPair.Key, equal);
            }

            var description = optionData.GetDialogDescription();

            if(description != "")
                this.AddDescription(MenuManager.CurrentMenu, description);
            
            UnselectedColor = GetOptionColor(optionData.Id);

            OnValueChange += onValueChange;
            OnAltPressed += onAltPressed;

            IncludeWidthInMeasurement = false;
        }

        private void onValueChange(float value) {
            SetOptionValue(optionData.Id, value, OptionSetter.User);

            UnselectedColor = GetOptionColor(optionData.Id);
        }

        private void onAltPressed() {
            ResetOptionValue(optionData.Id, OptionSetter.User);

            Audio.Play(SFX.ui_main_button_toggle_on);

            Refresh();
        }

        public void Refresh() {
            var newValue = GetOptionValue(Option);

            UnselectedColor = GetOptionColor(Option);

            if(optionData.Type == OptionType.Boolean) {
                if(newValue < 0 || newValue > 1) { //qhat the fuck this isnt a boolean
                    Index = PreviousIndex = newValue >= 1 ? 1 : 0;

                    return;
                }
            }

            if(Values.Last().Item2 < newValue) {
                Add(newValue.ToString() + optionData.Suffix, newValue, true);

                return;
            }

            var min = 0;
            var max = Values.Count - 1;

            while(min <= max) {
                var mid = (int)Math.Floor((min + max) / 2f);
                var value = Values[mid].Item2;
                
                if(value <= ReservedHybridEnumConstant && value > ReservedHybridEnumConstant - ReservedHybridEnumSize)
                    value = 0;

                if(value > newValue) {
                    max = mid - 1;
                } else if(value < newValue) {
                    min = mid + 1;
                } else {
                    Index = PreviousIndex = mid;

                    return;
                }
            }

            Values.Insert(min, new Tuple<string, float>(newValue.ToString() + optionData.Suffix, newValue));

            Index = PreviousIndex = min;
        }
    }
}