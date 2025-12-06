using System;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.GooberHelper.Settings {
    public abstract class AbstractEnum : AbstractSetting {
        public abstract Type EnumType { get; set; }

        public virtual void OnValueChange(object value)
            => SettingProperty.SetValue(ContainerObject, value);

        public override void CreateEntry(object container, bool inGame) {
            base.CreateEntry(container, inGame);

            Utils.Log($"creating enum for {GetType()}");

            if(SettingProperty.GetValue(ContainerObject) is not object value || value.GetType() != EnumType)
                throw new Exception("NOT THE CORRECT TYPE");

            var enumNames = Enum.GetNames(EnumType);
            var enumValues = Enum.GetValues(EnumType).Cast<object>();

            var enumPairs = enumValues
                .Zip(enumNames)
                .Select((pair, index) => KeyValuePair.Create(pair.First, Dialog.Clean($"gooberhelper_enum_{pair.Second}")));

            var slider = new TextMenuExt.EnumerableSlider<object>(
                GetName(),
                enumPairs,
                value
            );

            slider.OnValueChange += OnValueChange;
            
            Entry = slider;

            AddToContainer();
            AddDescription();
        }
    }
}