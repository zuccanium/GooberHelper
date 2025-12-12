namespace Celeste.Mod.GooberHelper.UI.OptionsMenu {
    public class ResetAllButton : TextMenuExt.ButtonExt {
        public ResetAllButton() : base(Dialog.Clean("menu_gooberhelper_reset_all_options")) {
            OnPressed += onPressed;
        }

        public override void Added() {
            base.Added();

            this.AddDescription(MenuManager.CurrentMenu, Dialog.Clean("menu_gooberhelper_reset_all_options_description"));
        }

        private void onPressed() {
            ResetAll(OptionSetter.User);

            if(MenuManager.CurrentMenu is OptionsMenu optionsMenu)
                optionsMenu.Refresh();
        }
    }
}