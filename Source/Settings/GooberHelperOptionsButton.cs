using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Settings.Infrastructure.Modes;
using Celeste.Mod.GooberHelper.UI;

namespace Celeste.Mod.GooberHelper.Settings {
    [GooberHelperSetting]
    public class GooberHelperOptionsButton : SettingButton {
        public override void CreateEntry(TextMenu menu, bool inGame)
            => menu.Add(Entry = OuiGooberHelperOptions.CreateOptionsButton(menu, inGame, inGame));
    }
}