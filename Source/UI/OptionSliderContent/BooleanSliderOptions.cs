using System.Collections;
using System.Collections.Generic;

namespace Celeste.Mod.GooberHelper.UI.OptionSliderContent {
    public class BooleanSliderOptions : IEnumerable<KeyValuePair<float, string>> {
        private static IEnumerable<KeyValuePair<float, string>> options = new List<KeyValuePair<float, string>>([
            new KeyValuePair<float, string>(0, Dialog.Clean("options_off")),
            new KeyValuePair<float, string>(1, Dialog.Clean("options_on"))
        ]);

        public IEnumerator<KeyValuePair<float, string>> GetEnumerator()
            => options.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => options.GetEnumerator();
    }
}