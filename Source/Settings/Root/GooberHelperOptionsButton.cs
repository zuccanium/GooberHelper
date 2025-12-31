using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.UI.OptionsMenu;

namespace Celeste.Mod.GooberHelper.Settings.Root {
    [GooberHelperSetting]
    public class GooberHelperOptionsButton : AbstractButton {
        public override void CreateEntry(object container, bool inGame) {
            Utils.Log("BONG");
            
            if(container is not TextMenu menu)
                return;
            
            menu.Add(Entry = new PauseMenuOptionsButton(menu, inGame));
        }
    }
}