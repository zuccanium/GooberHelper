namespace Celeste.Mod.GooberHelper.Settings.Buttons {
    public abstract class AbstractButton : AbstractSetting {
        public virtual void OnPressed() {}

        public override void CreateEntry(object container, bool inGame) {
            var item = new TextMenu.Button(GetName());

            item.OnPressed += OnPressed;

            Entry = item;

            AddToContainer();
            AddDescription();
            AddStandardDescription();
        }
    }
}