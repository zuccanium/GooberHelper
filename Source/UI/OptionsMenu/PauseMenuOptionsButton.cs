namespace Celeste.Mod.GooberHelper.UI.OptionsMenu {
    public class PauseMenuOptionsButton : TextMenuExt.ButtonExt {
        private TextMenu rootMenu;

        public PauseMenuOptionsButton(TextMenu menu, bool inGame) : base(Dialog.Clean("menu_gooberhelper_options")) {
            TextColor = GetGlobalColor();

            rootMenu = menu;

            if(!inGame)
                GooberHelperModule.Instance._Session = null;

            OnPressed += onPressed;
            OnLeave += onLeave;
        }

        private void onPressed() {
            MenuManager.SetRootMenu(rootMenu);
            MenuManager.OpenMenu<OptionsMenu>();
        }

        //this is a stupid workaround for the IRefreshable interface not doing anything since this button wont be in a goober-managed menu
        //it works though
        //i dont really want to rework the entire refreshing system rn
        //this will suffice
        private void onLeave()
            => TextColor = GetGlobalColor();
    }
}