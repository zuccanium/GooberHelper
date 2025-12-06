using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.GooberHelper.Options;

namespace Celeste.Mod.GooberHelper.UI {
    public class OuiGooberHelperOptions : Oui {
        public class NumericSliderOptions : IEnumerable<KeyValuePair<float, string>> {
            private OptionData optionData;

            public NumericSliderOptions(OptionData optionData)
                => this.optionData = optionData;

            public IEnumerator<KeyValuePair<float, string>> GetSideEnumerator(int dir, float start) {
                var n = start;
                var mag = optionData.Step;

                for(var i = 0; n <= Math.Abs(dir == 1 ? optionData.Max : optionData.Min); i++) {
                    if(n != 0 || dir == 1) //dont have a -0
                        yield return new KeyValuePair<float, string>(n * dir, (n * dir).ToString() + optionData.Suffix);

                    if(optionData.ExponentialIncrease) {
                        if(n == mag * 100) mag *= 10;

                        n += mag * (
                            n < mag * 20 ? 1 :
                            n < mag * 50 ? 2 :
                            5
                        );
                    } else {
                        n += optionData.Step;
                    }
                    
                    n = MathF.Round(n / optionData.Step) * optionData.Step;
                }
            }

            public IEnumerator<KeyValuePair<float, string>> GetEnumerator() {
                if(optionData.EnumType != null)
                    for(var i = -1; i >= -optionData.EnumMax; i--)
                        yield return new KeyValuePair<float, string>(i, Dialog.Clean($"gooberhelper_enum_{optionData.EnumType.GetEnumName(i)}"));

                var leftEnumerator = GetSideEnumerator(-1, 0);
                var rightEnumerator = GetSideEnumerator(1, 0);

                //enumerate the left side
                List<KeyValuePair<float, string>> left = [];

                while(leftEnumerator.MoveNext())
                    left.Add(leftEnumerator.Current);

                for(var i = left.Count - 1; i >= 0; i--)
                    yield return left[i];

                //enumerate the right side
                while(rightEnumerator.MoveNext())
                    yield return rightEnumerator.Current;

                yield break;
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }
        }

        //holy c#slop
        public static IEnumerable<KeyValuePair<float, string>> BooleanSliderOptions = new List<KeyValuePair<float, string>>([
            new KeyValuePair<float, string>(0, Dialog.Clean("options_off")),
            new KeyValuePair<float, string>(1, Dialog.Clean("options_on"))
        ]);

        public class EnumSliderOptions : IEnumerable<KeyValuePair<float, string>> {
            public Type EnumType;
            public string EnumName;

            public EnumSliderOptions(Type enumType) {
                EnumType = enumType;
                EnumName = enumType.Name;
            }

            public IEnumerator<KeyValuePair<float, string>> GetEnumerator() {
                var i = 0;
                
                while(Enum.IsDefined(EnumType, i)) {
                    yield return new KeyValuePair<float, string>(i, Dialog.Clean($"gooberhelper_enum_{EnumType.GetEnumName(i)}"));

                    i++;
                }

                yield break;
            }

            IEnumerator IEnumerable.GetEnumerator()
                => GetEnumerator();
        }

        private TextMenu menu = null;
        private static TextMenu backgroundMenu = null;
        private static bool inGame = true;
        private static Dictionary<string, TextMenuExt.ButtonExt> categoryButtons = [];
        // public static Dictionary<Option, Tuple<TextMenuExt.EnumerableSlider<float>, int>> optionSliders = [];
        private static Dictionary<TextMenuExt.EnumerableSlider<float>, Option> optionSliders = [];

        private static string queuedOptionsProfileName;
        private static bool wasAllowingHudHide = true;
        private static bool wasPauseMainMenuOpen = true;
        private static int optionsProfileStartIndex;
        private static TextMenuGooberExt.Combo combo;
        private static TextMenuExt.Modal comboModal;

        public enum OptionsProfileAction {
            Load,
            Save,
            Rename,
            Duplicate,
            Export,
            Import,
            Delete
        }

        private static Color importErrorColor = Color.Red;
        private static Color importSuccessColor = Color.Lime;

        public OuiGooberHelperOptions() : base() {}

        public static TextMenuExt.ButtonExt CreateOptionsButton(TextMenu backgroundMenu, bool fromPause, bool inGame = true) {
            OuiGooberHelperOptions.backgroundMenu = backgroundMenu;
            OuiGooberHelperOptions.inGame = inGame;

            var button = new TextMenuExt.ButtonExt(Dialog.Clean("menu_gooberhelper_options"));

            //dont display information about session options
            //this should probably go somewhere else but it works here
            if(!inGame)
                GooberHelperModule.Instance._Session = null;

            button.TextColor = GetGlobalColor();
            
            button.OnPressed = () => {
                var options = CreateMenu(fromPause);

                backgroundMenu.Scene.Add(options);
            };

            return button;
        }

        private static void createOnPauseAction(TextMenu menu)
            => menu.OnPause = () => {
                menu.Close();

                GooberHelperModule.Instance.SaveSettings();

                (Engine.Scene as Level).Paused = false;
                (Engine.Scene as Level).AllowHudHide = wasAllowingHudHide;
                (Engine.Scene as Level).unpauseTimer = 0.15f;

                Audio.Play(SFX.ui_main_button_back);
            };

        private static TextMenuExt.ButtonExt addCategoryButton(string categoryName, TextMenu menu) {
            var button = new TextMenuExt.ButtonExt(Dialog.Clean($"menu_gooberhelper_category_{categoryName}"));

            button.TextColor = GetCategoryColor(categoryName);
            
            button.OnPressed = () => {
                var returnIndex = menu.IndexOf(button);
                menu.RemoveSelf();

                var categoryMenu = createCategoryMenu(categoryName); //this

                categoryMenu.OnESC = categoryMenu.OnCancel = () => {
                    categoryMenu.CloseAndRun(null, () => {
                        categoryMenu.Scene.Add(CreateMenu(false, returnIndex));

                        Audio.Play(SFX.ui_main_button_back);
                    });
                };

                createOnPauseAction(categoryMenu);

                menu.Scene.Add(categoryMenu);
            };

            button.OnAltPressed = () => {
                ResetCategory(categoryName, OptionSetter.User);

                Audio.Play(SFX.ui_main_button_toggle_on);

                button.TextColor = GetCategoryColor(categoryName);
            };

            categoryButtons[categoryName] = button;

            menu.Add(button);

            return button;
        }

        private static TextMenuExt.EnumerableSlider<float> addOptionSlider(OptionData optionData, TextMenu menu) {
            var startValue = GetOptionValue(optionData.Id);

            //do this but for enums
            if(optionData.Type == OptionType.Boolean)
                startValue = startValue >= 1
                    ? 1
                    : 0;
            
            if(optionData.Type == OptionType.Enum)
                startValue = startValue > optionData.EnumMax
                    ? 0
                    : MathF.Floor(Math.Max(startValue, 0));

            var optionSlider = new TextMenuExt.EnumerableSlider<float>(
                label: optionData.GetDialogName(),
                options:
                    optionData.Type == OptionType.Boolean
                        ? BooleanSliderOptions
                    
                    : optionData.Type == OptionType.Enum
                        ? new EnumSliderOptions(optionData.EnumType)

                    : new NumericSliderOptions(optionData),
                startValue
            );

            menu.Add(optionSlider);
            optionSliders[optionSlider] = optionData.Id;

            var description = optionData.GetDialogDescription();

            if(description != "")
                optionSlider.AddDescription(menu, description);
            
            if(optionSlider.Values[optionSlider.Index].Item2 != startValue) {
                updateOptionSlider(optionSlider);
            } else {
                optionSlider.UnselectedColor = GetOptionColor(optionData.Id);
            }

            optionSlider.OnValueChange = value => {
                SetOptionValue(optionData.Id, value, OptionSetter.User);

                optionSlider.UnselectedColor = GetOptionColor(optionData.Id);
            };

            optionSlider.OnAltPressed = () => {
                ResetOptionValue(optionData.Id, OptionSetter.User);

                Audio.Play(SFX.ui_main_button_toggle_on);

                updateOptionSlider(optionSlider);
            };

            optionSlider.IncludeWidthInMeasurement = false;

            return optionSlider;
        }

        private static void updateOptionSlider(TextMenuExt.EnumerableSlider<float> optionSlider) {
            var option = optionSliders[optionSlider];
            var newValue = GetOptionValue(option);

            optionSlider.UnselectedColor = GetOptionColor(option);

            if(OptionsManager.Options[option].Type == OptionType.Boolean) {
                if(newValue < 0 || newValue > 1) { //qhat the fuck this isnt a boolean
                    optionSlider.Index = optionSlider.PreviousIndex = newValue >= 1 ? 1 : 0;

                    return;
                }
            }

            if(optionSlider.Values.Last().Item2 < newValue) {
                optionSlider.Add(newValue.ToString() + OptionsManager.Options[option].Suffix, newValue, true);

                return;
            }

            var min = 0;
            var max = optionSlider.Values.Count - 1;

            while(min <= max) {
                var mid = (int)Math.Floor((min + max)/2f);

                if(optionSlider.Values[mid].Item2 > newValue) {
                    max = mid - 1;
                } else if(optionSlider.Values[mid].Item2 < newValue) {
                    min = mid + 1;
                } else {
                    optionSlider.Index = optionSlider.PreviousIndex = mid;

                    return;
                }
            }

            optionSlider.Values.Insert(min, new Tuple<string, float>(newValue.ToString() + OptionsManager.Options[option].Suffix, newValue));

            optionSlider.Index = optionSlider.PreviousIndex = min;
        }

        private static TextMenu createCategoryMenu(string categoryName) {
            var menu = new TextMenu();

            optionSliders.Clear();
            categoryButtons.Clear();

            menu.Add(new TextMenu.Header(Dialog.Clean($"menu_gooberhelper_category_{categoryName}")));

            foreach(var optionData in Categories[categoryName])
                addOptionSlider(optionData, menu);

            menu.Add(new TextMenu.SubHeader(""));

            var resetAllButton = new TextMenu.Button(Dialog.Clean("menu_gooberhelper_reset_all_options"));
                menu.Add(resetAllButton);

                resetAllButton.AddDescription(menu, Dialog.Clean("menu_gooberhelper_reset_all_options_description"));
                
                resetAllButton.OnPressed = () => {
                    ResetCategory(categoryName, OptionSetter.User);

                    foreach(var pair in optionSliders)
                        updateOptionSlider(pair.Key);
                };

            menu.MoveSelection(1);
            menu.MoveSelection(-1);

            menu.CompactWidthMode = true;
            menu.MinWidth = 1700;
            menu.Width = 1700;

            return menu;
        }

        private static void createOptionsProfileButton(string name, TextMenu menu, int insertionIndex = -1) {
            var profileItem = new TextMenuExt.EnumSlider<OptionsProfileAction>(name);
            TextMenuExt.EaseInSubHeaderExt description;

            if(insertionIndex == -1) {
                menu.Add(profileItem);

                profileItem.AddDescription(menu, "");
                description = menu.Items.Last() as TextMenuExt.EaseInSubHeaderExt;
            } else {
                menu.Insert(insertionIndex, profileItem);

                profileItem.AddDescription(menu, "");
                description = menu.Items[insertionIndex + 1] as TextMenuExt.EaseInSubHeaderExt;
            }


            profileItem.OnPressed = () => {
                switch(profileItem.Index) {
                    case (int)OptionsProfileAction.Load:
                        OptionsProfile.Load(name);

                        Audio.Play(SFX.ui_main_button_select);

                        updateMenu();
                    break;
                    case (int)OptionsProfileAction.Save:
                        OptionsProfile.Save(name);

                        Audio.Play(SFX.ui_main_button_select);

                        updateMenu();
                    break;
                    case (int)OptionsProfileAction.Rename:
                        Action<string> finish = (newName) => {
                            Audio.Play(SFX.ui_main_rename_entry_accept);

                            if(name == newName) return;

                            OptionsProfile.Rename(name, newName);

                            profileItem.Label = newName;
                            name = newName;
                        };

                        Audio.Play(SFX.ui_main_savefile_rename_start);

                        Utils.OpenTextInputField(finish, null, "Rename the options profile");
                    break;
                    case (int)OptionsProfileAction.Duplicate:
                        var duplicate = OptionsProfile.Duplicate(name, out var insertionIndex);

                        createOptionsProfileButton(duplicate.Name, menu, optionsProfileStartIndex + insertionIndex * 2);

                        Audio.Play(SFX.ui_main_whoosh_savefile_in);
                    break;
                    case (int)OptionsProfileAction.Export:
                        OptionsProfile.Export(name);

                        Audio.Play(SFX.ui_main_whoosh_savefile_out);
                    break;
                    case (int)OptionsProfileAction.Import:
                        try {
                            OptionsProfile.Import(name);

                            description.Title = Dialog.Clean("menu_gooberhelper_import_profile_success");
                            description.TextColor = importSuccessColor;

                            Audio.Play(SFX.ui_main_whoosh_savefile_in);
                        } catch {
                            description.Title = Dialog.Clean("menu_gooberhelper_import_profile_error");
                            description.TextColor = importErrorColor;

                            Audio.Play(SFX.ui_main_button_invalid);
                        }
                    break;
                    case (int)OptionsProfileAction.Delete:
                        OptionsProfile.Delete(name);

                        menu.items.Remove(profileItem);
                        menu.items.Remove(description);

                        combo.Increase();
                        comboModal.Visible = true;

                        if(menu.Selection >= menu.items.Count) {
                            menu.Selection = menu.LastPossibleSelection;

                            menu.MoveSelection(-1);
                            menu.MoveSelection(1);
                        }

                        Audio.Play(SFX.ui_main_savefile_delete);
                    break;
                }
            };

            profileItem.OnValueChange = (value) => {
                description.TextColor = Color.Gray;

                switch(profileItem.Index) {
                    case (int)OptionsProfileAction.Export:
                        description.Title = Dialog.Clean("menu_gooberhelper_export_profile_description");
                    break;
                    case (int)OptionsProfileAction.Import:
                        description.Title = Dialog.Clean("menu_gooberhelper_import_profile_description");
                    break;
                    case (int)OptionsProfileAction.Delete:
                        description.Title = Dialog.Clean("menu_gooberhelper_delete_profile_description");
                        description.TextColor = Color.Red;
                    break;
                    default:
                        description.Title = "";
                    break;
                }
            };
        }

        private static void updateMenu() {
            foreach(var pair in categoryButtons)
                pair.Value.TextColor = GetCategoryColor(pair.Key);

            foreach(var pair in optionSliders)
                updateOptionSlider(pair.Key);
        }

        public static TextMenu CreateMenu(bool fromPause = false, int startIndex = 3) { //2 because title and input field modal thing
            var menu = new TextMenu();

            backgroundMenu.Visible = false;
            backgroundMenu.Active = false;
            backgroundMenu.Focused = false;

            if(fromPause && inGame) {
                wasAllowingHudHide = (Engine.Scene as Level).AllowHudHide;
                wasPauseMainMenuOpen = (Engine.Scene as Level).PauseMainMenuOpen;

                (Engine.Scene as Level).AllowHudHide = false;
                (Engine.Scene as Level).PauseMainMenuOpen = false;
            }

            Utils.CreateTextInputField(menu);

            combo = new TextMenuGooberExt.Combo(1f, 2) { Container = menu };
            comboModal = new TextMenuExt.Modal(combo, 120, 1000) { BorderThickness = 0 };

            menu.Add(comboModal);

            menu.OnUpdate = () => {
                if(queuedOptionsProfileName != null) {
                    createOptionsProfileButton(queuedOptionsProfileName, menu);

                    queuedOptionsProfileName = null;
                }
            };

            menu.OnESC = menu.OnCancel = () => {
                menu.CloseAndRun(null, () => {
                    GooberHelperModule.Instance.SaveSettings();

                    if(inGame) {
                        (menu.Scene as Level).AllowHudHide = wasAllowingHudHide;
                        (menu.Scene as Level).PauseMainMenuOpen = wasPauseMainMenuOpen;
                    }

                    backgroundMenu.Visible = true;
                    backgroundMenu.Active = true;
                    backgroundMenu.Focused = true;
                    backgroundMenu.Position.Y = backgroundMenu.ScrollTargetY;

                    Audio.Play(SFX.ui_main_button_back);
                });
            };

            createOnPauseAction(menu);

            optionSliders.Clear();
            categoryButtons.Clear();

            menu.Add(new TextMenu.Header(Dialog.Clean("menu_gooberhelper_title")));

            var resetAllButton = new TextMenu.Button(Dialog.Clean("menu_gooberhelper_reset_all_options"));
                menu.Add(resetAllButton);

                resetAllButton.AddDescription(menu, Dialog.Clean("menu_gooberhelper_reset_all_options_description"));

                resetAllButton.OnPressed = () => {
                    ResetAll(OptionSetter.User);

                    updateMenu();
                };

                // resetAllButton.OnAltPressed = () => {
                //     combo.Increase();
                //     comboModal.Visible = true;
                // };

            menu.Add(new TextMenuExt.SubHeaderExt(Dialog.Clean("menu_gooberhelper_category_physics")));
                addCategoryButton("Jumping",  menu);
                addCategoryButton("Dashing", menu);
                addCategoryButton("Moving", menu);
                addCategoryButton("Other", menu);


            menu.Add(new TextMenuExt.SubHeaderExt(Dialog.Clean("menu_gooberhelper_category_visuals")));
                addCategoryButton("Visuals", menu);


            menu.Add(new TextMenuExt.SubHeaderExt(Dialog.Clean("menu_gooberhelper_category_miscellaneous")));
                addCategoryButton("Miscellaneous", menu);


            menu.Add(new TextMenuExt.SubHeaderExt(Dialog.Clean("menu_gooberhelper_category_general")));
                addOptionSlider(OptionsManager.Options[Option.ShowActiveOptions], menu); //should this not be reset along with the others??


            menu.Add(new TextMenuExt.SubHeaderExt(Dialog.Clean("menu_gooberhelper_category_profiles")));
            
            
            var createButton = new TextMenu.Button(Dialog.Clean("menu_gooberhelper_create_profile"));
                menu.Add(createButton);

                createButton.ConfirmSfx = null;
                createButton.OnPressed = () => {
                    static void finish(string name) {
                        if (OptionsProfile.GetExists(name)) {
                            OptionsProfile.Save(name);

                            return;
                        }

                        OptionsProfile.Create(name);

                        //this code is running while the textmenu is enumerating all of its items
                        //trying to add a new one causes a crash
                        queuedOptionsProfileName = name;

                        Audio.Play(SFX.ui_main_rename_entry_accept);
                    }

                    Utils.OpenTextInputField(finish, null, "Name the profile");

                    Audio.Play(SFX.ui_main_savefile_rename_start);
                };

            var importButton = new TextMenu.Button(Dialog.Clean("menu_gooberhelper_import_create_profile"));
                menu.Add(importButton);
                importButton.AddDescription(menu, Dialog.Clean("menu_gooberhelper_import_create_profile_description"));

                var description = menu.items.Last() as TextMenuExt.EaseInSubHeaderExt;

                importButton.ConfirmSfx = null;
                importButton.OnPressed = () => {
                    try {
                        queuedOptionsProfileName = OptionsProfile.CreateFromImport().Name;

                        description.Title = Dialog.Clean("menu_gooberhelper_import_create_profile_success").Replace("name", queuedOptionsProfileName);
                        description.TextColor = importSuccessColor;

                        Audio.Play(SFX.ui_main_whoosh_savefile_in);
                    } catch {
                        description.Title = Dialog.Clean("menu_gooberhelper_import_profile_error");
                        description.TextColor = importErrorColor;

                        Audio.Play(SFX.ui_main_button_invalid);
                    }
                };

            
            optionsProfileStartIndex = menu.Items.Count;

            // if(GooberHelperModule.Settings.OptionsProfileOrder.Count == 0) {
            //     foreach(var pair in GooberHelperModule.Settings.OptionsProfiles) {
            //         GooberHelperModule.Settings.OptionsProfileOrder.Add(pair.Key);
            //     }
            // }

            foreach(var optionProfileName in GooberHelperModule.Settings.OptionsProfileOrder)
                createOptionsProfileButton(optionProfileName, menu);

            menu.Selection = startIndex;

            menu.MoveSelection(1);
            menu.MoveSelection(-1);

            menu.CompactWidthMode = true;
            menu.MinWidth = 1500;
            menu.Width = 1500;

            return menu;
        }

        public override IEnumerator Enter(Oui from) {
            menu = CreateMenu();
            Visible = true;
            menu.Focused = true;

            yield break;
        }

        public override IEnumerator Leave(Oui from) {
            Visible = false;
            menu.RemoveSelf();
            menu = null;

            yield break;
        }
    } 
}