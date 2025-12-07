using System;
using Celeste.Mod.GooberHelper.DataStructures;
using Celeste.Mod.GooberHelper.Settings.Toggles;
using Celeste.Mod.GooberHelper.UI;

namespace Celeste.Mod.GooberHelper.Settings {
    public abstract class AbstractScreenCircle : AbstractValuedSetting<DummyCircle, Circle> {
        public override void OnValueChange(Circle value)
            => SettingProperty.SetValue(ContainerObject, new DummyCircle(value));

        public override TextMenu.Item CreateItem(DummyCircle value) {
            var item = new TextMenuGooberExt.ScreenCircle(GetName(), value.ToCircle());
            
            item.OnValueChange += OnValueChange;

            return item;
        }
    }
}