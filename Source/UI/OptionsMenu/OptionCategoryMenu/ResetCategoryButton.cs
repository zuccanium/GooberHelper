using Celeste.Mod.GooberHelper.Options;

namespace Celeste.Mod.GooberHelper.UI.OptionsMenu.OptionCategoryMenu {
    public class ResetCategoryButton : TextMenuExt.ButtonExt {
        public OptionCategory Category;

        public ResetCategoryButton(OptionCategory category) : base(Dialog.Clean("menu_gooberhelper_reset_all_options")) {
            Category = category;

            this.AddDescription(MenuManager.CurrentMenu, Dialog.Clean("menu_gooberhelper_reset_all_options_description"));

            OnPressed += onPressed;
        }

        private void onPressed() {
            ResetCategory(Category, OptionSetter.User);

            foreach(var item in MenuManager.CurrentMenu.Items)
                if(item is OptionSlider optionSlider)
                    optionSlider.Refresh();
        }
    }
}