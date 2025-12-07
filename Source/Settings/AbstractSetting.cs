using System;
using System.Reflection;
using Celeste.Mod.GooberHelper.UI;

namespace Celeste.Mod.GooberHelper.Settings {
    public abstract class AbstractSetting {
        public PropertyInfo SettingProperty;
        public object ContainerObject;
        public object ContainerMenu;
        public TextMenu.Item Entry;

        public virtual string GetDescription() {
            var key = $"menu_gooberhelper_setting_description_{GetType().Name}";

            return Dialog.Has(key)
                ? Dialog.Clean(key)
                : null;
        }

        public virtual string GetName()
            => Dialog.Clean($"menu_gooberhelper_setting_{GetType().Name}");

        public virtual void CreateEntry(object container, bool inGame) {}

        public virtual void AddDescription() {
            if(GetDescription() is not string description)
                return;

            if(ContainerMenu is TextMenu menu) {
                Entry.AddDescription(menu, description);
            } else if(ContainerMenu is TextMenuGooberExt.NestableSubMenu subMenu) {
                Entry.AddDescription(subMenu, subMenu.Container, description);
            }
        }

        public virtual void AddStandardDescription() {
            var key = $"menu_gooberhelper_ui_description_{Entry?.GetType().Name}";

            Console.WriteLine(key);

            if(!Dialog.Has(key))
                return;

            var description = Dialog.Clean(key);
            
            if(ContainerMenu is TextMenu menu) {
                Entry.AddDescription(menu, description);
            } else if(ContainerMenu is TextMenuGooberExt.NestableSubMenu subMenu) {
                Entry.AddDescription(subMenu, subMenu.Container, description);
            }
        }
        
        public virtual void AddToContainer() {
            if(ContainerMenu is TextMenu menu) {
                menu.Add(Entry);
            } else if(ContainerMenu is TextMenuGooberExt.NestableSubMenu subMenu) {
                subMenu.Add(Entry);
            }
        }
    }
}