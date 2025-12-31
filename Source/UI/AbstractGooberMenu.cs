namespace Celeste.Mod.GooberHelper.UI {
    public abstract class AbstractGooberMenu : TextMenu, IRefreshable {
        public virtual void Refresh() {
            foreach(var item in Items)
                if(item is IRefreshable refreshableItem)
                    refreshableItem.Refresh();
        }

        public virtual void Added() {}
    }
}