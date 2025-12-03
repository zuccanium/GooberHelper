using System;
using System.Reflection;

namespace Celeste.Mod.GooberHelper.Settings.Buttons {
    public abstract class AbstractButton : AbstractSetting {
        public AbstractButton() {}

        public virtual void OnPressed() {}

        public override void CreateEntry(object container, bool inGame) {
            Utils.Log($"creating button for {GetType()}");

            var button = new TextMenu.Button(Dialog.Clean($"menu_gooberhelper_setting_{GetType().Name}"));

            button.OnPressed += OnPressed;

            Entry = button;

            if(container is TextMenu menu)
                menu.Add(button);

            if(container is TextMenuExt.SubMenu subMenu)
                subMenu.Add(button);
        }
    }
}