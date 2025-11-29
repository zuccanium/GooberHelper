namespace Celeste.Mod.GooberHelper.Settings {
    public abstract class AbstractSetting {
        public TextMenu.Item Entry;

        public virtual void CreateEntry(TextMenu menu, bool inGame) {}
    }
}