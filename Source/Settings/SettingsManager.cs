using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Celeste.Mod.GooberHelper.Attributes;
using FMOD.Studio;

namespace Celeste.Mod.GooberHelper.Settings {
    public static class SettingsManager {
        private static Dictionary<string, Type> settingClasses = [];

        [OnLoad]
        public static void Load() {
            foreach(var type in typeof(SettingsManager).Assembly.GetTypes())
                if(type.GetCustomAttribute(typeof(GooberHelperSettingAttribute), true) is GooberHelperSettingAttribute)
                    settingClasses[type.Name] = type;
        }

        [OnUnload]
        public static void Unload() {
            settingClasses.Clear();
        }

        public static void PopulateMenu(object containerObject, object containerMenu, bool inGame) {
            var containerType = containerObject.GetType();
            
            Utils.Log($"going through type {containerType}");

            foreach(var property in containerType.GetProperties(Utils.BindingFlagsAll)) {
                if(property.IsDefined(typeof(SettingIgnoreAttribute), false))
                    continue;
                
                if(settingClasses.TryGetValue(property.Name, out var memberType)) {
                    var instance = Activator.CreateInstance(memberType) as AbstractSetting;

                    instance.SettingProperty = property;
                    instance.SettingContainer = containerObject;
                    instance.CreateEntry(containerMenu, inGame);

                    continue;
                }

                if(property.PropertyType.DeclaringType == containerType) {
                    var subMenu = new TextMenuExt.SubMenu(Dialog.Clean($"menu_gooberhelper_submenu_{property.Name}"), false);

                    if(containerMenu is TextMenu menu)
                        menu.Add(subMenu);

                    PopulateMenu(property.GetValue(containerObject), subMenu, inGame);
                }
            }
        }

        public static void CreateModMenuSection(TextMenu menu, bool inGame, EventInstance eventInstance)
            => PopulateMenu(GooberHelperModule.Settings, menu, inGame);
    }
}