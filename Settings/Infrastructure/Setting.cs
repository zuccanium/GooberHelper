namespace Celeste.Mod.GooberHelper.Settings.Infrastructure {
    public abstract class Setting {
        public TextMenu.Item Entry;

        public virtual void CreateEntry(TextMenu menu, bool inGame) {}
    }
}