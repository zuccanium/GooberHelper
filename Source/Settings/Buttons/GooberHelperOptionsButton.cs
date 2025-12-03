using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.UI;

namespace Celeste.Mod.GooberHelper.Settings.Buttons {
    [GooberHelperSetting]
    public class GooberHelperOptionsButton : AbstractButton {
        public override void CreateEntry(object container, bool inGame) {
            if(container is not TextMenu menu)
                return;
            
            menu.Add(Entry = OuiGooberHelperOptions.CreateOptionsButton(menu, inGame, inGame));
        }
    }
}