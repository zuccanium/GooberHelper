using Celeste.Mod.GooberHelper.Options;

namespace Celeste.Mod.GooberHelper.UI.OptionsMenu {
    public class OptionsProfileImportButton : TextMenuExt.ButtonExt {
        private TextMenuExt.EaseInSubHeaderExt description;
        public static OptionsProfileImportButton Instance;

        public OptionsProfileImportButton() : base(Dialog.Clean("menu_gooberhelper_import_create_profile")) {
            ConfirmSfx = null;

            Instance = this;

            OnPressed += onPressed;
        }

        public override void Added() {
            //literally just so i dont have to write MenuManager.CurrentMenu everywhere
            if(MenuManager.CurrentMenu is not TextMenu menu)
                return;
            
            this.AddDescription(menu, Dialog.Clean("menu_gooberhelper_import_create_profile_description"));

            description = menu.Items[menu.Items.IndexOf(this) + 1] as TextMenuExt.EaseInSubHeaderExt;
        }

        private void onPressed() {
            try {
                if(MenuManager.CurrentMenu is not OptionsMenu optionsMenu)
                    return;

                var imported = OptionsProfile.CreateFromImport();

                optionsMenu.QueueOptionsProfile(imported.Name);

                description.Title = Dialog.Clean("menu_gooberhelper_import_create_profile_success").Replace("name", imported.Name);
                description.TextColor = OptionsMenu.ImportSuccessColor;

                Audio.Play(SFX.ui_main_whoosh_savefile_in);
            } catch {
                description.Title = Dialog.Clean("menu_gooberhelper_import_profile_error");
                description.TextColor = OptionsMenu.ImportErrorColor;

                Audio.Play(SFX.ui_main_button_invalid);
            }
        }
    }
}