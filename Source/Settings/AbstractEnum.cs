using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.GooberHelper.UI;

namespace Celeste.Mod.GooberHelper.Settings {
    public abstract class AbstractEnum : AbstractSetting {
        public abstract Type EnumType { get; set; }

        public virtual void OnValueChange(object value)
            => SettingProperty.SetValue(ContainerObject, value);

        public override void CreateEntry(object container, bool inGame) {
            base.CreateEntry(container, inGame);

            Utils.Log($"creating enum for {GetType()}");

            if(SettingProperty.GetValue(ContainerObject) is not object value || value.GetType() != EnumType) {
                Logger.Error("GooberHelper", "hwfehowjhefoiawjeofjawiojefioawj");

                value = null;
            }

            var enumNames = Enum.GetNames(EnumType);
            var enumValues = Enum.GetValuesAsUnderlyingType(EnumType).Cast<object>();

            var enumPairs = enumValues
                .Zip(enumNames)
                .Select(pair => KeyValuePair.Create(pair.First, pair.Second));

            var slider = new TextMenuExt.EnumerableSlider<object>(
                GetName(),
                enumPairs,
                enumValues.First()
            );

            slider.OnValueChange += OnValueChange;
            
            Entry = slider;

            AddToContainer();
            AddDescription();
        }
    }
}