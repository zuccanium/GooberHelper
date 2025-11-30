namespace Celeste.Mod.GooberHelper.Settings {
    public abstract class AbstractSetting {
        public TextMenu.Item Entry;

        public virtual string GetDescription() {
            var key = $"menu_gooberhelper_setting_description_{GetType().Name}";

            return Dialog.Has(key)
                ? Dialog.Clean(key)
                : null;
        }

        public virtual void CreateEntry(TextMenu menu, bool inGame) {}
    }
}