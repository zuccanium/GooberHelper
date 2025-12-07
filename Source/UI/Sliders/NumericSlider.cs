using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.GooberHelper.UI.Sliders {
    // public class NumericSlider : TextMenuExt.EnumerableSlider<float> {
    //     public class NumericSliderData {
    //         public float Min;
    //         public float Max;
    //         public float Step;
    //         public bool ExponentialIncrease;
    //         public string MaxLabel;
    //         public string Suffix;
    //         public Type EnumType;
    //         public float EnumMax;
    //     }

    //     public class NumericSliderOptionEnumerator : IEnumerable<KeyValuePair<float, string>> {
    //         public NumericSliderData Data;

    //         public NumericSliderOptionEnumerator(NumericSliderData data)
    //             => Data = data;

    //         public IEnumerator<KeyValuePair<float, string>> GetSideEnumerator(int dir, float start) {
    //             var n = start;
    //             var mag = Data.Step;

    //             for(var i = 0; n <= Math.Abs(dir == 1 ? Data.Max : Data.Min); i++) {
    //                 if(n != 0 || dir == 1) //dont have a -0
    //                     yield return new KeyValuePair<float, string>(n * dir, (n * dir).ToString() + Data.Suffix);

    //                 if(Data.ExponentialIncrease) {
    //                     if(n == mag * 100) mag *= 10;

    //                     n += mag * (
    //                         n < mag * 20 ? 1 :
    //                         n < mag * 50 ? 2 :
    //                         5
    //                     );
    //                 } else {
    //                     n += Data.Step;
    //                 }
                    
    //                 n = MathF.Round(n / Data.Step) * Data.Step;
    //             }
    //         }

    //         public IEnumerator<KeyValuePair<float, string>> GetEnumerator() {
    //             if(Data.EnumType != null)
    //                 for(var i = -1; i >= -Data.EnumMax; i--)
    //                     yield return new KeyValuePair<float, string>(i, Dialog.Clean($"gooberhelper_enum_{Data.EnumType.GetEnumName(i)}"));

    //             var leftEnumerator = GetSideEnumerator(-1, 0);
    //             var rightEnumerator = GetSideEnumerator(1, 0);

    //             //enumerate the left side
    //             List<KeyValuePair<float, string>> left = [];

    //             while(leftEnumerator.MoveNext())
    //                 left.Add(leftEnumerator.Current);

    //             for(var i = left.Count - 1; i >= 0; i--)
    //                 yield return left[i];

    //             //enumerate the right side
    //             while(rightEnumerator.MoveNext())
    //                 yield return rightEnumerator.Current;

    //             yield break;
    //         }

    //         IEnumerator IEnumerable.GetEnumerator()
    //             => GetEnumerator();
    //     }

    //     public NumericSliderData Data;

    //     public NumericSlider(string label, NumericSliderData data, float defaultValue) : base(label, new NumericSliderOptionEnumerator(data), defaultValue) {
    //         Data = data;
            
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