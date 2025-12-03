using System.Reflection;

namespace Celeste.Mod.GooberHelper.Settings.Toggles {
    public abstract class AbstractToggle : AbstractSetting {
        public AbstractToggle() {}

        public virtual void OnValueChange(bool value)
            => SettingProperty.SetValue(SettingContainer, value);


        public override void CreateEntry(object container, bool inGame) {
            Utils.Log($"creating toggle for {GetType()}");

            if(SettingProperty.GetValue(SettingContainer) is not bool value) {
                Logger.Error("GooberHelper", "hwfehowjhefoiawjeofjawiojefioawj");

                value = false;
            }

            var toggle = new TextMenu.OnOff(
                Dialog.Clean($"menu_gooberhelper_setting_{GetType().Name}"),
                value
            );

            toggle.OnValueChange += OnValueChange;
            
            Entry = toggle;

            if(container is TextMenu menu) {
                menu.Add(toggle);

                if(GetDescription() is string description)
                    toggle.AddDescription(menu, description);
            } else if(container is TextMenuExt.SubMenu subMenu) {
                subMenu.Add(toggle);

                if(GetDescription() is string description)
                    toggle.AddDescription(subMenu, subMenu.Container, description);
            }
        }
    }
}