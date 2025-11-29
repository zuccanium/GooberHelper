using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Settings.Infrastructure.Modes;

namespace Celeste.Mod.GooberHelper.Settings {
    [GooberHelperSetting]
    public class ResetAllOptionsButton : SettingButton {
        public override void CreateEntry(TextMenu menu, bool inGame)
            => menu.Add(
                Entry = new TextMenu.Button(Dialog.Clean("menu_gooberhelper_reset_all_options"))
                    .Pressed(() => ResetAll(OptionSetter.User))
            );
    }
}