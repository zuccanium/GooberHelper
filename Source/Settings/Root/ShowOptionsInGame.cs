using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.UI.OptionsMenu;

namespace Celeste.Mod.GooberHelper.Settings.Root {
    [GooberHelperSetting]
    public class ShowOptionsInGame : AbstractToggle {
        [SubscribeToEvent(typeof(Everest.Events.Level), "OnCreatePauseMenuButtons")]
        public static void CreatePauseMenuButtons(Level level, TextMenu menu, bool minimal) {
            if(!GooberHelperModule.Settings.ShowOptionsInGame)
                return;

            var index = menu.items.FindIndex(item => item is TextMenu.Button && (item as TextMenu.Button).Label == Dialog.Clean("menu_pause_options"));
            menu.Insert(index, new PauseMenuOptionsButton(menu, true));
        }
    }
}