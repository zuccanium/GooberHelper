namespace Celeste.Mod.GooberHelper.UI.OptionsMenu.OptionCategoryMenu {
    public class OptionCategoryMenu : AbstractGooberMenu {
        public OptionCategory Category;

        public OptionCategoryMenu(OptionCategory category) : base() {
            Category = category;
            
            CompactWidthMode = true;
            MinWidth = 1700;
            Width = 1700;

            OnESC += onLeave;
            OnCancel += onLeave;
            OnPause += onPause;
        }

        public override void Added() {
            Add(new Header(Dialog.Clean($"menu_gooberhelper_category_{Category}")));

            foreach(var option in CategoryToOptions[Category]) {
                var headGroup = OptionToInstance[option].HeadGroup;

                if(headGroup != OptionGroup.None)
                    Add(new SubHeader(Dialog.Clean($"menu_gooberhelper_group_{headGroup}")));

                Add(new OptionSlider(option));
            }

            Add(new SubHeader(""));

            Add(new ResetCategoryButton(Category));
        }

        private void onLeave() {
            MenuManager.GoBack();
            Audio.Play(SFX.ui_main_button_back);
        }
        
        private void onPause() {
            GooberHelperModule.Instance.SaveSettings();
            
            MenuManager.GotoRoot();
            
            if(Engine.Scene is Level level)
                level.Unpause();
        }
    }
}