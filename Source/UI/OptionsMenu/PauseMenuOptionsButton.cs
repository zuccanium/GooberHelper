namespace Celeste.Mod.GooberHelper.UI.OptionsMenu {
    public class PauseMenuOptionsButton : TextMenuExt.ButtonExt {
        private TextMenu rootMenu;

        public PauseMenuOptionsButton(TextMenu menu, bool inGame) : base(Dialog.Clean("menu_gooberhelper_options")) {
            TextColor = GetGlobalColor();

            rootMenu = menu;

            if(!inGame)
                GooberHelperModule.Instance._Session = null;

            OnPressed += onPressed;
        }

        private void onPressed() {
            MenuManager.SetRootMenu(rootMenu);
            MenuManager.OpenMenu<OptionsMenu>();
        }
    }
}