using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.GooberHelper.UI.Sliders {
    // public class EnumSlider : TextMenuExt.EnumerableSlider<float> {
    //     public class EnumSliderOptionEnumerator : IEnumerable<KeyValuePair<float, string>> {
    //         public Type EnumType;
    //         public string EnumName;

    //         public EnumSliderOptionEnumerator(Type enumType) {
    //             EnumType = enumType;
    //             EnumName = enumType.Name;
    //         }

    //         public IEnumerator<KeyValuePair<float, string>> GetEnumerator() {
    //             var i = 0;
                
    //             while(Enum.IsDefined(EnumType, i)) {
    //                 yield return new KeyValuePair<float, string>(i, Dialog.Clean($"gooberhelper_enum_{EnumType.GetEnumName(i)}"));

    //                 i++;
    //             }

    //             yield break;
    //         }

    //         IEnumerator IEnumerable.GetEnumerator()
    //             => GetEnumerator();
    //     }

    //     public Type EnumType;

    //     public EnumSlider(string label, Type enumType, float defaultValue) : base(label, new EnumSliderOptionEnumerator(enumType), defaultValue) {
    //         EnumType = enumType;

    //         UpdateValue(defaultValue);
    //     }

    //     public void UpdateValue(float newValue) {
    //         if(Values.Last().Item2 < newValue) {
    //             Add(newValue.ToString() + Data.Suffix, newValue, true);

    //             return;
    //         }

    //         var min = 0;
    //         var max = Values.Count - 1;

    //         while(min <= max) {
    //             var mid = (int)Math.Floor((min + max)/2f);

    //             if(Values[mid].Item2 > newValue) {
    //                 max = mid - 1;
    //             } else if(Values[mid].Item2 < newValue) {
    //                 min = mid + 1;
    //             } else {
    //                 Index = PreviousIndex = mid;

    //                 return;
    //             }
    //         }

    //         Values.Insert(min, new Tuple<string, float>(newValue.ToString() + Data.Suffix, newValue));

    //         Index = PreviousIndex = min;
    //     }
    // }
}