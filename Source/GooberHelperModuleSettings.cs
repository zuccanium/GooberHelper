using System.Collections.Generic;
using Celeste.Mod.GooberHelper.Options;
using Microsoft.Xna.Framework.Input;

namespace Celeste.Mod.GooberHelper {
    public class GooberHelperModuleSettings : EverestModuleSettings {
        [SettingIgnore]
        public Dictionary<Option, float> UserDefinedOptions { get; set; } = [];

        [SettingIgnore]
        public Dictionary<string, OptionsProfile> OptionsProfiles { get; set; } = [];

        [SettingIgnore]
        public List<string> OptionsProfileOrder { get; set; } = [];

        public class MouseJoystickSubMenu {
            public bool Enabled { get; set; } = false;

            public MouseJoystickSubMenu() {}
        }

        //actual settings
        public bool ShowOptionsInGame { get; set; } = false;
        public bool DebugMapPhysics { get; set; } = false;
        public bool FastMenuing { get; set; } = false;
        public bool CustomSwimmingAnimation { get; set; } = true; 
        public MouseJoystickSubMenu MouseJoystick { get; set; } = new();

        //buttons
        //theyre not public because i dont want them to be stored in the mod settings
        //theyre not private because my ide will get mad at me for having private members with PascalCase names
        //first ever use of protected
        //awesome
        protected object GooberHelperOptionsButton;
        protected object ResetAllOptionsButton;

        //binds (the single one)
        [DefaultButtonBinding(Buttons.LeftTrigger, Keys.Tab)]
        public ButtonBinding ExitGameSuspension { get; set; } = new ButtonBinding(Buttons.LeftTrigger, Keys.Tab);
    }
}
