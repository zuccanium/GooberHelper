using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Settings.Buttons;

namespace Celeste.Mod.GooberHelper.Settings.Root {
    [GooberHelperSetting]
    public class ResetAllOptionsButton : AbstractButton {
        public override void CreateEntry(object container, bool inGame) {
            if(container is not TextMenu menu)
                return;
            
            menu.Add(
                Entry = new TextMenu.Button(Dialog.Clean("menu_gooberhelper_reset_all_options"))
                    .Pressed(() => ResetAll(OptionSetter.User))
            );
        }
    }
}