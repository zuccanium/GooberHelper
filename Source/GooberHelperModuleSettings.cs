using System.Collections.Generic;
using Celeste.Mod.GooberHelper.DataStructures;
using Celeste.Mod.GooberHelper.Options;
using Celeste.Mod.GooberHelper.Settings.Root.MouseJoystick;
using Microsoft.Xna.Framework.Input;

namespace Celeste.Mod.GooberHelper {
    public class GooberHelperModuleSettings : EverestModuleSettings {
        [SettingIgnore]
        public Dictionary<Option, float> UserDefinedOptions { get; set; } = [];

        [SettingIgnore]
        public Dictionary<string, OptionsProfile> OptionsProfiles { get; set; } = [];

        [SettingIgnore]
        public List<string> OptionsProfileOrder { get; set; } = [];

        //actual settings
        //formatting menace
        public bool ShowOptionsInGame { get; set; } = false;
        public bool DebugMapPhysics { get; set; } = false;
        public bool FastMenuing { get; set; } = false;
        public bool CustomSwimmingAnimation { get; set; } = true; 
        public MouseJoystickSubMenu MouseJoystick { get; set; } = new(); public class MouseJoystickSubMenu() {
            public Mode.MouseJoystickMode Mode { get; set; } = Settings.Root.MouseJoystick.Mode.MouseJoystickMode.None;
            public AbsoluteModeSubMenu AbsoluteMode { get; set; } = new(); public class AbsoluteModeSubMenu() {
                public DummyCircle Circle { get; set; } = new DummyCircle(100, 0, 0);
                public Color OuterColor { get; set; } = new Color(1f, 0, 0, 0.2f);
                public Color InnerColor { get; set; } = new Color(0, 1f, 0, 0.2f);
                public Color BorderColor { get; set; } = new Color(0, 0, 1f, 1f);
                public float BorderThickness { get; set; } = 2f;
            }

            public RelativeModeSubMenu RelativeMode { get; set; } = new(); public class RelativeModeSubMenu() {
                public DummyCircle Circle { get; set; } = new DummyCircle(100, 0, 0);
                public float DeadzoneRadius { get; set; } = 50f;
                public float Margin { get; set; } = 5f;
            }
        }

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
