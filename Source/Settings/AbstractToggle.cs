using System.Reflection;

namespace Celeste.Mod.GooberHelper.Settings.Toggles {
    public abstract class AbstractToggle : AbstractSetting {
        public virtual void OnValueChange(bool value)
            => SettingProperty.SetValue(ContainerObject, value);

        public override void CreateEntry(object container, bool inGame) {
            base.CreateEntry(container, inGame);

            Utils.Log($"creating toggle for {GetType()}");

            if(SettingProperty.GetValue(ContainerObject) is not bool value) {
                Logger.Error("GooberHelper", "hwfehowjhefoiawjeofjawiojefioawj");

                value = false;
            }

            var toggle = new TextMenu.OnOff(
                GetName(),
                value
            );

            toggle.OnValueChange += OnValueChange;
            
            Entry = toggle;

            AddToContainer();
            AddDescription();
        }
    }
}