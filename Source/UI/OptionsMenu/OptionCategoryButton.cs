namespace Celeste.Mod.GooberHelper.UI.OptionsMenu {
    public class OptionCategoryButton : TextMenuExt.ButtonExt, IRefreshable {
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

        private void onPressed() {
            MenuManager.OpenMenu<OptionCategoryMenu.OptionCategoryMenu>(Category);
            
            //dont immediately trigger the dynamic slider speed increase thing when opening onto an option
            Input.Jump.ConsumeBuffer();
        }

        public void Refresh() {
            TextColor = GetCategoryColor(Category);
        }
    }
}