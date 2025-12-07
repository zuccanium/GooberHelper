using System;
using System.Collections.Generic;
using System.Reflection;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.UI;
using FMOD.Studio;

namespace Celeste.Mod.GooberHelper.Settings {
    public static class SettingsManager {
        private static Dictionary<string, Type> settingClasses = [];

        [OnLoad]
        public static void Load() {
            var namespacePrefix = $"{typeof(SettingsManager).Namespace}.Root.";

            foreach(var type in typeof(SettingsManager).Assembly.GetTypes())
                if(type.GetCustomAttribute(typeof(GooberHelperSettingAttribute), true) is GooberHelperSettingAttribute) {
                    var id = type.Namespace.Length < namespacePrefix.Length
                        ? type.Name
                        : $"{type.Namespace[namespacePrefix.Length..]}.{type.Name}";
                    
                    Utils.Log($"{type.Name} -> {id}");
                    
                    settingClasses[id] = type;
                }
        }

        [OnUnload]
        public static void Unload() {
            settingClasses.Clear();
        }

        public static void PopulateMenu(object containerObject, object containerMenu, bool inGame, string prefix = "") {
            var containerType = containerObject.GetType();
            
            Utils.Log($"going through type {containerType}");

            foreach(var property in containerType.GetProperties(Utils.BindingFlagsAll)) {
                if(property.IsDefined(typeof(SettingIgnoreAttribute), false))
                    continue;
                
                Utils.Log($"found property {property} with declaring type {property.PropertyType.DeclaringType}");

                if(settingClasses.TryGetValue(prefix + property.Name, out var memberType)) {
                    var instance = Activator.CreateInstance(memberType) as AbstractSetting;

                    instance.SettingProperty = property;
                    instance.ContainerObject = containerObject;
                    instance.ContainerMenu = containerMenu;
                    instance.CreateEntry(containerMenu, inGame);

                    continue;
                }

                if(property.PropertyType.DeclaringType == containerType) {
                    Utils.Log("found submenu");

                    var newSubMenu = new TextMenuGooberExt.NestableSubMenu(Dialog.Clean($"menu_gooberhelper_setting_submenu_{property.Name}"), false);

                    if(containerMenu is TextMenu menu)
                        menu.Add(newSubMenu);

                    if(containerMenu is TextMenuGooberExt.NestableSubMenu subMenu)
                        subMenu.Add(newSubMenu);

                    PopulateMenu(property.GetValue(containerObject), newSubMenu, inGame, $"{prefix}{property.Name}.");
                }
            }
        }

        public static void CreateModMenuSection(TextMenu menu, bool inGame, EventInstance eventInstance)
            => PopulateMenu(GooberHelperModule.Settings, menu, inGame);
    }
}