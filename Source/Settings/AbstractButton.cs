using System;
using System.Reflection;

namespace Celeste.Mod.GooberHelper.Settings.Buttons {
    public abstract class AbstractButton : AbstractSetting {
        public AbstractButton() {}

        public virtual void OnPressed() {}

        public override void CreateEntry(object container, bool inGame) {
            base.CreateEntry(container, inGame);

            Utils.Log($"creating button for {GetType()}");

            var button = new TextMenu.Button(GetName());

            button.OnPressed += OnPressed;

            Entry = button;

            AddToContainer();
            AddDescription();
        }
    }
}