using System;
using System.Collections;
using System.Collections.Generic;
using Celeste.Mod.GooberHelper.Options;

namespace Celeste.Mod.GooberHelper.UI.OptionSliderContent {
    public class NumericSliderOptions : IEnumerable<KeyValuePair<float, string>> {
        public float Min;
        public float Max;
        public float Step;
        public bool ExponentialIncrease;
        public bool IgnoreZero;
        public string Suffix;
        public Type EnumType;

        public NumericSliderOptions(OptionData optionData) {
            Min = optionData.Min;
            Max = optionData.Max;
            Step = optionData.Step;
            ExponentialIncrease = optionData.ExponentialIncrease;
            IgnoreZero = optionData.IgnoreZero;
            Suffix = optionData.Suffix;
            EnumType = optionData.EnumType;
        }

        public IEnumerator<KeyValuePair<float, string>> GetSideEnumerator(int dir, float start) {
            var n = start;
            var mag = Step;

            for(var i = 0; n <= Math.Abs(dir == 1 ? Max : Min); i++) {
                if(IgnoreZero && n == 0 || n == 0) //yeah
                    goto increment;
                
                if(n == 0 && dir == -1) //dont have a -0
                    goto increment;

                yield return new KeyValuePair<float, string>(n * dir, (n * dir).ToString() + Suffix);

                increment:
                if(ExponentialIncrease) {
                    if(n == mag * 100) mag *= 10;

                    n += mag * (
                        n < mag * 20 ? 1 :
                        n < mag * 50 ? 2 :
                        5
                    );
                } else {
                    n += Step;
                }
                
                n = MathF.Round(n / Step) * Step;
            }
        }

        public IEnumerator<KeyValuePair<float, string>> GetEnumerator() {
            //enumerate the left side
            var leftEnumerator = GetSideEnumerator(-1, 0);

            List<KeyValuePair<float, string>> left = [];

            while(leftEnumerator.MoveNext())
                left.Add(leftEnumerator.Current);

            for(var i = left.Count - 1; i >= 0; i--)
                yield return left[i];

            //middle
            if(EnumType != null)
                foreach(var value in Enum.GetValues(EnumType))
                    yield return new KeyValuePair<float, string>(Convert.ToSingle(value), Dialog.Clean($"gooberhelper_enum_{value}"));

            //enumerate the right side
            var rightEnumerator = GetSideEnumerator(1, 0);

            while(rightEnumerator.MoveNext())
                yield return rightEnumerator.Current;

            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}