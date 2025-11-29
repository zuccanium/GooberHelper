using Celeste.Mod.GooberHelper.Attributes;

namespace Celeste.Mod.GooberHelper.Settings.Buttons {
    [GooberHelperSetting]
    public class ResetAllOptionsButton : AbstractButton {
        public override void CreateEntry(TextMenu menu, bool inGame)
            => menu.Add(
                Entry = new TextMenu.Button(Dialog.Clean("menu_gooberhelper_reset_all_options"))
                    .Pressed(() => ResetAll(OptionSetter.User))
            );
    }
}