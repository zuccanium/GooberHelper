using System.Reflection;

namespace Celeste.Mod.GooberHelper.Settings {
    public abstract class AbstractSetting {
        public PropertyInfo SettingProperty;
        public object SettingContainer;
        public TextMenu.Item Entry;

        public virtual string GetDescription() {
            var key = $"menu_gooberhelper_setting_description_{GetType().Name}";

            return Dialog.Has(key)
                ? Dialog.Clean(key)
                : null;
        }

        public virtual void CreateEntry(object container, bool inGame) {}
    }
}