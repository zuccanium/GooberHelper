using Celeste.Mod.GooberHelper.UI;

namespace Celeste.Mod.GooberHelper.Settings {
    public abstract class AbstractColor : AbstractValuedSetting<Color, Color> {
        public override TextMenu.Item CreateItem(Color value) {
            var item = new TextMenuGooberExt.ColorInput(GetName(), value);

            item.OnValueChange += OnValueChange;
            
            return item;
        }
    }
}