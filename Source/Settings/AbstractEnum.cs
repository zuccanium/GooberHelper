using System;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.GooberHelper.Settings {
    public abstract class AbstractEnum<T> : AbstractValuedSetting<T, T> where T : struct, Enum {
        public override TextMenu.Item CreateItem(T value) {
            var enumValues = Enum.GetValues<T>();

            var options = enumValues
                .Select(value => KeyValuePair.Create(value, Dialog.Clean($"gooberhelper_enum_{value}")));

            var item = new TextMenuExt.EnumerableSlider<T>(GetName(), options, value);
            
            item.OnValueChange += OnValueChange;
            
            return item;
        }
    }
}