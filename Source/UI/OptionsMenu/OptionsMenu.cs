using System.Collections.Generic;

namespace Celeste.Mod.GooberHelper.UI.OptionsMenu {
    public class OptionsMenu : AbstractGooberMenu {
        public static readonly Color ImportErrorColor = Color.Red;
        public static readonly Color ImportSuccessColor = Color.Lime;

        public TextMenuGooberExt.Combo Combo;
        public TextMenuExt.Modal ComboModal;

        private Queue<string> queuedOptionsProfiles = [];

        public OptionsMenu() : base() {
            CompactWidthMode = true;
            MinWidth = 1500;
            Width = 1500;

            OnUpdate += onUpdate;
            OnESC += onLeave;
            OnCancel += onLeave;
            OnPause += onPause;

            if(Engine.Scene is not Level level)
                return;
            
            level.PauseMainMenuOpen = false;
            level.AllowHudHide = false;
        }

        public override void Added() {
            //general
            Utils.CreateTextInputField(this);

            Combo = new TextMenuGooberExt.Combo(1f, 2) { Container = this };
            ComboModal = new TextMenuExt.Modal(Combo, 120, 1000) { BorderThickness = 0 };
            Add(ComboModal);

            //actual menu
            Add(new Header(Dialog.Clean("menu_gooberhelper_title")));

            Add(new ResetAllButton());

            //categories
            Add(new TextMenuExt.SubHeaderExt(Dialog.Clean("menu_gooberhelper_category_physics")));
                Add(new OptionCategoryButton(OptionCategory.Jumping));
                Add(new OptionCategoryButton(OptionCategory.Dashing));
                Add(new OptionCategoryButton(OptionCategory.Moving));
                Add(new OptionCategoryButton(OptionCategory.Entities));
                Add(new OptionCategoryButton(OptionCategory.Other));

            Add(new TextMenuExt.SubHeaderExt(Dialog.Clean("menu_gooberhelper_category_visuals")));
                Add(new OptionCategoryButton(OptionCategory.Visuals));

            Add(new TextMenuExt.SubHeaderExt(Dialog.Clean("menu_gooberhelper_category_miscellaneous")));
                Add(new OptionCategoryButton(OptionCategory.Miscellaneous));

            Add(new TextMenuExt.SubHeaderExt(Dialog.Clean("menu_gooberhelper_category_general")));
                Add(new OptionSlider(Option.ShowActiveOptions));

            //options profiles
            Add(new TextMenuExt.SubHeaderExt(Dialog.Clean("menu_gooberhelper_profiles")));
            
            Add(new OptionsProfileCreateButton());
            Add(new OptionsProfileImportButton());

            foreach(var optionProfileName in GooberHelperModule.Settings.OptionsProfileOrder)
                Add(new OptionsProfileButton(optionProfileName));
        }

        public override void Removed(Scene scene) {
            if(Engine.Scene is not Level level)
                return;
            
            level.PauseMainMenuOpen = true;
            level.AllowHudHide = true;
        }

        public void QueueOptionsProfile(string name)
            => queuedOptionsProfiles.Enqueue(name);

        private void onUpdate() {
            while(queuedOptionsProfiles.TryDequeue(out var profileName))
                Add(new OptionsProfileButton(profileName));
        }

        private void onLeave() {
            GooberHelperModule.Instance.SaveSettings();

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