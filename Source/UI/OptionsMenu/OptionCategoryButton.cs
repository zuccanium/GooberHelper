namespace Celeste.Mod.GooberHelper.UI.OptionsMenu {
    public class OptionCategoryButton : TextMenuExt.ButtonExt {
        public OptionCategory Category;

        public OptionCategoryButton(OptionCategory category) : base(Dialog.Clean($"menu_gooberhelper_category_{category}")) {
            Category = category;
            TextColor = GetCategoryColor(category);

            OnAltPressed += onAltPressed;
            OnPressed += onPressed;
        }

        private void onAltPressed() {
            ResetCategory(Category, OptionSetter.User);

            Audio.Play(SFX.ui_main_button_toggle_on);

            Refresh();
        }

        private void onPressed()
            => MenuManager.OpenMenu<OptionCategoryMenu.OptionCategoryMenu>(Category);

        public void Refresh() {
            TextColor = GetCategoryColor(Category);
        }
    }
}