using System.Reflection;

namespace Celeste.Mod.GooberHelper.Settings.Infrastructure.Modes {
    public abstract class SettingToggle : Setting {
        public PropertyInfo SettingProperty;

        public SettingToggle()
            => SettingProperty = typeof(GooberHelperModuleSettings).GetProperty(GetType().Name);

        public virtual void OnValueChange(bool value)
            => SettingProperty.SetValue(GooberHelperModule.Settings, value);

        public override void CreateEntry(TextMenu menu, bool inGame) {            
            if(SettingProperty.GetValue(GooberHelperModule.Settings) is not bool value) {
                Logger.Error("GooberHelper", "hwfehowjhefoiawjeofjawiojefioawj");

                value = false;
            }

            var toggle = new TextMenu.OnOff(
                Dialog.Clean($"menu_gooberhelper_setting_{GetType().Name}"),
                value
            );

            toggle.OnValueChange += OnValueChange;

            Entry = toggle;
            menu.Add(toggle);
        }
    }
}