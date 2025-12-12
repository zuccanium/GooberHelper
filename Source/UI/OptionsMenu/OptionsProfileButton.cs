using System;
using System.Collections.Generic;
using Celeste.Mod.GooberHelper.Options;

namespace Celeste.Mod.GooberHelper.UI.OptionsMenu {
    public class OptionsProfileButton : TextMenuExt.EnumerableSlider<OptionsProfileButton.OptionsProfileAction> {
        public string Name;
        private TextMenuExt.EaseInSubHeaderExt description;

        public enum OptionsProfileAction {
            Load,
            Save,
            Rename,
            Duplicate,
            Export,
            Import,
            Delete
        }

        public OptionsProfileButton(string name) : base(name, new List<OptionsProfileAction>(), 0) {
            Name = name;

            foreach(var value in Enum.GetValues<OptionsProfileAction>())
                Add(value.ToString(), value, false);

            OnPressed += onPressed;
            OnValueChange += onValueChange;
        }

        public override void Added() {
            //literally just so i dont have to write MenuManager.CurrentMenu everywhere
            if(MenuManager.CurrentMenu is not TextMenu menu)
                return;
            
            this.AddDescription(menu, "");

            description = menu.Items[menu.Items.IndexOf(this) + 1] as TextMenuExt.EaseInSubHeaderExt;
        }

        private void onPressed() {
            if(MenuManager.CurrentMenu is not OptionsMenu menu)
                return;

            switch(Index) {
                case (int)OptionsProfileAction.Load:
                    OptionsProfile.Load(Name);

                    Audio.Play(SFX.ui_main_button_select);

                    menu.Refresh();
                break;
                case (int)OptionsProfileAction.Save:
                    OptionsProfile.Save(Name);

                    Audio.Play(SFX.ui_main_button_select);

                    menu.Refresh();
                break;
                case (int)OptionsProfileAction.Rename:
                    Audio.Play(SFX.ui_main_savefile_rename_start);

                    Utils.OpenTextInputField(onFinishRenaming, null, Dialog.Clean("menu_gooberhelper_rename_profile_dialog"));
                break;
                case (int)OptionsProfileAction.Duplicate:
                    var duplicate = OptionsProfile.Duplicate(Name, out var insertionIndex);
                    var startIndex = MenuManager.CurrentMenu.Items.IndexOf(OptionsProfileImportButton.Instance) + 1; //+ 1 for the description

                    //* 2 because descriptions
                    menu.Insert(startIndex + insertionIndex * 2 + 1, new OptionsProfileButton(duplicate.Name));

                    Audio.Play(SFX.ui_main_whoosh_savefile_in);
                break;
                case (int)OptionsProfileAction.Export:
                    OptionsProfile.Export(Name);

                    Audio.Play(SFX.ui_main_whoosh_savefile_out);
                break;
                case (int)OptionsProfileAction.Import:
                    try {
                        OptionsProfile.Import(Name);

                        description.Title = Dialog.Clean("menu_gooberhelper_import_profile_success");
                        description.TextColor = OptionsMenu.ImportSuccessColor;

                        Audio.Play(SFX.ui_main_whoosh_savefile_in);
                    } catch {
                        description.Title = Dialog.Clean("menu_gooberhelper_import_profile_error");
                        description.TextColor = OptionsMenu.ImportErrorColor;

                        Audio.Play(SFX.ui_main_button_invalid);
                    }
                break;
                case (int)OptionsProfileAction.Delete:
                    OptionsProfile.Delete(Name);

                    menu.Remove(this);
                    menu.Remove(description);

                    menu.Combo.Increase();
                    menu.ComboModal.Visible = true;

                    Utils.Log($"my selection is {menu.Selection}; the last possible selection is {menu.LastPossibleSelection}");
                    
                    if(menu.Selection >= menu.items.Count) {
                        Utils.Log("TOO LARGE");
                        
                        menu.Selection = menu.LastPossibleSelection;
                    }

                    Audio.Play(SFX.ui_main_savefile_delete);
                break;
            }
        }

        private void onValueChange(OptionsProfileAction value) {
            description.TextColor = Color.Gray;

            var action = (OptionsProfileAction)Index;

            description.Title = action switch {
                OptionsProfileAction.Export => Dialog.Clean("menu_gooberhelper_export_profile_description"),
                OptionsProfileAction.Import => Dialog.Clean("menu_gooberhelper_import_profile_description"),
                OptionsProfileAction.Delete => Dialog.Clean("menu_gooberhelper_delete_profile_description"),
                _ => ""
            };

            description.TextColor = action == OptionsProfileAction.Delete
                ? Color.Red
                : description.TextColor;
        }

        private void onFinishRenaming(string newName) {
            Audio.Play(SFX.ui_main_rename_entry_accept);

            if(Name == newName)
                return;

            OptionsProfile.Rename(Name, newName);

            Label = newName;
            Name = newName;
        }
    }
}