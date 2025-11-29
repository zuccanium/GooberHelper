using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.UI;

namespace Celeste.Mod.GooberHelper.Settings.Buttons {
    [GooberHelperSetting]
    public class GooberHelperOptionsButton : AbstractButton {
        public override void CreateEntry(TextMenu menu, bool inGame)
            => menu.Add(Entry = OuiGooberHelperOptions.CreateOptionsButton(menu, inGame, inGame));
    }
}