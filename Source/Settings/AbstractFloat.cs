using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Mod.GooberHelper.Settings {
    public abstract class AbstractFloat : AbstractSetting {
        public abstract float Min { get; set; }
        public abstract float Max { get; set; }
        public virtual float Step { get; set; } = 1f;
        public virtual string Suffix { get; set; } = "";

        public virtual void OnValueChange(float value)
            => SettingProperty.SetValue(ContainerObject, value);

        public override void CreateEntry(object container, bool inGame) {
            base.CreateEntry(container, inGame);

            Utils.Log($"creating float for {GetType()}");

            if(SettingProperty.GetValue(ContainerObject) is not float value) {
                Logger.Error("GooberHelper", "hwfehowjhefoiawjeofjawiojefioawj");

                value = 0;
            }

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

            var toggle = new TextMenuExt.EnumerableSlider<float>(
                GetName(),
                values,
                value
            );

            toggle.OnValueChange += OnValueChange;
            
            Entry = toggle;

            AddToContainer();
            AddDescription();
        }
    }
}