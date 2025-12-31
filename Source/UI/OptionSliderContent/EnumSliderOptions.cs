using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste.Mod.GooberHelper.UI.OptionSliderContent {
    public class EnumSliderOptions : IEnumerable<KeyValuePair<float, string>> {
        public Type EnumType;
        public string EnumName;

        public EnumSliderOptions(Type enumType) {
            EnumType = enumType;
            EnumName = enumType.Name;
        }

        public IEnumerator<KeyValuePair<float, string>> GetEnumerator() {            
            foreach(var value in Enum.GetValues(EnumType))
                yield return new KeyValuePair<float, string>(Convert.ToSingle(value), Dialog.Clean($"gooberhelper_enum_{value}"));

            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}