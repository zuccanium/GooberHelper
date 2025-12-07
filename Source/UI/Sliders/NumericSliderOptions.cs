using System;
using System.Collections;
using System.Collections.Generic;
using Celeste.Mod.GooberHelper.Options;

namespace Celeste.Mod.GooberHelper.UI.Sliders {
    //holy c#slop
    // public static IEnumerable<KeyValuePair<float, string>> BooleanSliderOptions = new List<KeyValuePair<float, string>>([
    //     new KeyValuePair<float, string>(0, Dialog.Clean("options_off")),
    //     new KeyValuePair<float, string>(1, Dialog.Clean("options_on"))
    // ]);

    // public class EnumSliderOptions : IEnumerable<KeyValuePair<float, string>> {
    //     public Type EnumType;
    //     public string EnumName;

    //     public EnumSliderOptions(Type enumType) {
    //         EnumType = enumType;
    //         EnumName = enumType.Name;
    //     }

    //     public IEnumerator<KeyValuePair<float, string>> GetEnumerator() {
    //         var i = 0;
            
    //         while(Enum.IsDefined(EnumType, i)) {
    //             yield return new KeyValuePair<float, string>(i, Dialog.Clean($"gooberhelper_enum_{EnumType.GetEnumName(i)}"));

    //             i++;
    //         }

    //         yield break;
    //     }

    //     IEnumerator IEnumerable.GetEnumerator()
    //         => GetEnumerator();
    // }
}