using System.Reflection;

namespace Celeste.Mod.GooberHelper.Settings.Buttons {
    public abstract class AbstractButton : AbstractSetting {
        public PropertyInfo SettingProperty;

        public AbstractButton()
            => SettingProperty = typeof(GooberHelperModuleSettings).GetProperty(GetType().Name);

        public virtual void OnPressed() {}

        public override void CreateEntry(TextMenu menu, bool inGame) {            
            var button = new TextMenu.Button(Dialog.Clean($"menu_gooberhelper_setting_{GetType().Name}"));

            button.OnPressed += OnPressed;

            Entry = button;
            menu.Add(button);
        }
    }
}