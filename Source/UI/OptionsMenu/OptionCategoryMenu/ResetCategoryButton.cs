namespace Celeste.Mod.GooberHelper.UI.OptionsMenu.OptionCategoryMenu {
    public class ResetCategoryButton : TextMenuExt.ButtonExt {
        public OptionCategory Category;

        public ResetCategoryButton(OptionCategory category) : base(Dialog.Clean("menu_gooberhelper_reset_category_options")) {
            Category = category;

            OnPressed += onPressed;
        }

        public override void Added() {
            this.AddDescription(MenuManager.CurrentMenu, Dialog.Clean("menu_gooberhelper_reset_category_options_description"));
        }

        private void onPressed() {
            ResetCategory(Category, OptionSetter.User);

            MenuManager.RefreshAll();
        }
    }
}