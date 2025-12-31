using System;

namespace Celeste.Mod.GooberHelper.Settings {
    public abstract class AbstractValuedSetting<TStored, TReal> : AbstractSetting {
        public virtual void OnValueChange(TReal value)
            => SettingProperty.SetValue(ContainerObject, value);

        public virtual TStored GetValue() {
            if(SettingProperty.GetValue(ContainerObject) is not TStored value)
                throw new InvalidOperationException();

            return value;
        }

        public abstract TextMenu.Item CreateItem(TStored value);

        public override void CreateEntry(object container, bool inGame) {            
            Entry = CreateItem(GetValue());

            AddToContainer();
            AddDescription();
            AddStandardDescription();
        }
    }
}