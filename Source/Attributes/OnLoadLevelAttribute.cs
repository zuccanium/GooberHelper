using System;
using System.Collections.Generic;
using System.Reflection;

namespace Celeste.Mod.GooberHelper.Attributes {
    [AttributeUsage(AttributeTargets.Method)]
    public class OnLoadLevelAttribute : Attribute {
        private static List<MethodInfo> methods = [];

        [OnLoad]
        public static void Load() {
            foreach(var type in typeof(OnLoadLevelAttribute).Assembly.GetTypes()) {
                foreach(var method in type.GetMethods(Utils.BindingFlagsAll)) {
                    if(!IsDefined(method, typeof(OnLoadLevelAttribute)))
                        continue;

                    methods.Add(method);

                    Utils.Log($"registering on load level for {method} on {method.DeclaringType}...");
                }
            }

            Everest.Events.Level.OnLoadLevel += OnLoadLevel;
        }

        [OnUnload]
        public static void Unload() {
            methods.Clear();

            Everest.Events.Level.OnLoadLevel -= OnLoadLevel;
        }

        public static void OnLoadLevel(Level level, Player.IntroTypes playerIntro, bool isFromLoader) {
            var argsList = new List<object>() { level, playerIntro, isFromLoader };
            
            foreach(var method in methods)
                method.Invoke(null, argsList[..method.GetParameters().Length].ToArray());
        }
    }
}