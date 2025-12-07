namespace Celeste.Mod.GooberHelper.Settings.Toggles {
    public abstract class AbstractToggle : AbstractValuedSetting<bool, bool> {
        public override TextMenu.Item CreateItem(bool value) {
            var item = new TextMenu.OnOff(GetName(), value);
            
            item.OnValueChange += OnValueChange;

            return item;
        }
    }
}