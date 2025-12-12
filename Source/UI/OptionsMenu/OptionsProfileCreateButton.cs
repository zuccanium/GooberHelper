using Celeste.Mod.GooberHelper.Options;

namespace Celeste.Mod.GooberHelper.UI.OptionsMenu {
    public class OptionsProfileCreateButton : TextMenuExt.ButtonExt {
        public OptionsProfileCreateButton() : base(Dialog.Clean("menu_gooberhelper_create_profile")) {
            ConfirmSfx = null;

            OnPressed += onPressed;
        }

        private void onPressed() {
            Utils.OpenTextInputField(onFinishCreating, null, Dialog.Clean("menu_gooberhelper_name_profile_dialog"));

            Audio.Play(SFX.ui_main_savefile_rename_start);
        }

        private void onFinishCreating(string name) {
            if(OptionsProfile.GetExists(name)) {
                OptionsProfile.Save(name);

                return;
            }

            OptionsProfile.Create(name);

            if(MenuManager.CurrentMenu is not OptionsMenu optionsMenu)
                return;

            optionsMenu.QueueOptionsProfile(name);

            Audio.Play(SFX.ui_main_rename_entry_accept);
        }
    }
}