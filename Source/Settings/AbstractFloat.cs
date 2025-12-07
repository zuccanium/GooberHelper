using System;
using System.Collections.Generic;
using Celeste.Mod.GooberHelper.Settings.Toggles;

namespace Celeste.Mod.GooberHelper.Settings {
    public abstract class AbstractFloat : AbstractValuedSetting<float, float> {
        public abstract float Min { get; set; }
        public abstract float Max { get; set; }
        public virtual float Step { get; set; } = 1f;
        public virtual string Suffix { get; set; } = "";

        public override TextMenu.Item CreateItem(float value) {
            var caughtValue = false;
            var values = new List<KeyValuePair<float, string>>();

            for(var i = Min; i <= Max; i += Step) {
                if(!caughtValue && value < i) {
                    values.Add(KeyValuePair.Create(value, value + Suffix));

                    caughtValue = true;
                }

                if(value == i)
                    caughtValue = true;
                
                values.Add(KeyValuePair.Create(i, i + Suffix));
            }

            if(!caughtValue && value > Max)
                values.Add(KeyValuePair.Create(value, value + Suffix));

            var item = new TextMenuExt.EnumerableSlider<float>(GetName(), values, value);
            
            item.OnValueChange += OnValueChange;
        
            return item;
        }
    }
}