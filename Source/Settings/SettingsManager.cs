using System;
using System.Collections.Generic;
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

        public static void CreateModMenuSection(TextMenu menu, bool inGame, EventInstance eventInstance) {
            foreach(var member in typeof(GooberHelperModuleSettings).GetMembers(Utils.BindingFlagsAll)) {
                if(member.IsDefined(typeof(SettingIgnoreAttribute), false))
                    continue;
                
                if(settingClasses.TryGetValue(member.Name, out var type)) {
                    var instance = Activator.CreateInstance(type) as AbstractSetting;

                    instance.CreateEntry(menu, inGame);
                }
            }
        }
    }
}