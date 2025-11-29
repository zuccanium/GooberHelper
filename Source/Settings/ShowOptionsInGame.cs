using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Settings.Infrastructure.Modes;
using Celeste.Mod.GooberHelper.UI;

namespace Celeste.Mod.GooberHelper.Settings {
    [GooberHelperSetting]
    public class ShowOptionsInGame : SettingToggle {
        [OnLoad]
        public static void Load() {
            Everest.Events.Level.OnCreatePauseMenuButtons += onCreatePauseMenuButtons;
        }

        [OnUnload]
        public static void Unload() {
            Everest.Events.Level.OnCreatePauseMenuButtons -= onCreatePauseMenuButtons;
        }

        //thank you everest!!! i stole a lot of your mod options code here; i hope you dont mind
        private static void onCreatePauseMenuButtons(Level level, TextMenu menu, bool minimal) {
            if(!GooberHelperModule.Settings.ShowOptionsInGame)
                return;

            var index = menu.items.FindIndex(item => item is TextMenu.Button && (item as TextMenu.Button).Label == Dialog.Clean("menu_pause_options"));
            menu.Insert(index, OuiGooberHelperOptions.CreateOptionsButton(menu, true));
        }
    }
}