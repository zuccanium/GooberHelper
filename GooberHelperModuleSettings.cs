using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.GooberHelper.UI;
using Microsoft.Xna.Framework.Input;

namespace Celeste.Mod.GooberHelper {
    public class GooberHelperModuleSettings : EverestModuleSettings {
        [SettingIgnore]
        public Dictionary<OptionsManager.Option, float> UserDefinedOptions { get; set; } = new Dictionary<OptionsManager.Option, float>();

        [SettingIgnore]
        public Dictionary<string, OptionsManager.OptionsProfile> OptionsProfiles { get; set; } = new Dictionary<string, OptionsManager.OptionsProfile>();

        [SettingIgnore]
        public List<string> OptionsProfileOrder { get; set; } = new List<string>();

        private bool fastMenuing = false;


        //actual settings
        [SettingName("GooberHelper_ShowOptionsInGame")]
        public bool ShowOptionsInGame { get; set; } = false;

        public bool DebugMapPhysics { get; set; } = false;

        public void CreateDebugMapPhysicsEntry(TextMenu menu, bool inGame) {
            TextMenu.OnOff debugMapPhysics;

            menu.Add(debugMapPhysics = new TextMenu.OnOff(
                Dialog.Clean("GooberHelper_DebugMapPhysics"),
                DebugMapPhysics
            ));

            debugMapPhysics.AddExplodingDescription(menu, Dialog.Clean("GooberHelper_DebugMapPhysics_description"));

            var explodingDescription = menu.items[^1] as ExplodingDescription;

            debugMapPhysics.OnValueChange += value => {
                if(value)
                    explodingDescription.Explode();
                else
                    explodingDescription.Unexplode();
            };
        }

        [SettingName("GooberHelper_FastMenuing")]
        public bool FastMenuing {
            get => fastMenuing;
            set {
                fastMenuing = value;

                GooberHelperModule.UpdateFastMenuing();
            }
        }

        public bool GooberHelperOptionsButton { get; set; } = false;
        public void CreateGooberHelperOptionsButtonEntry(TextMenu menu, bool inGame) =>
            menu.Add(OuiGooberHelperOptions.CreateOptionsButton(menu, inGame, inGame));

        public bool ResetAllOptionsButton { get; set; } = false;
        public void CreateResetAllOptionsButtonEntry(TextMenu menu, bool inGame) =>
            menu.Add(new TextMenu.Button(Dialog.Clean("menu_gooberhelper_reset_all_options")).Pressed(() => {
                OptionsManager.ResetAll(OptionsManager.OptionSetter.User);
            }));

        //binds (the single one)
        [DefaultButtonBinding(Buttons.LeftTrigger, Keys.Tab)]
        public ButtonBinding ExitGameSuspension { get; set; } = new ButtonBinding(Buttons.LeftTrigger, Keys.Tab);
    }
}
