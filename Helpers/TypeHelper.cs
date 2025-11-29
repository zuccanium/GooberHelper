using System;
using System.Collections.Generic;
using System.Reflection;
using Celeste.Mod.GooberHelper.Attributes;

#nullable enable

namespace Celeste.Mod.GooberHelper.Helpers {
    public static class TypeHelper {
        public static readonly Dictionary<string, Type> TypeMap = [];
        
        [OnLoad]
        public static void Load()
            => LoadAssemblies(
                typeof(Player).Assembly,
                typeof(Scene).Assembly
            );

        [OnUnload]
        public static void Unload()
            => TypeMap.Clear();

        public static void LoadAssemblies(params Assembly[] assemblies) {
            foreach(var assembly in assemblies) {
                foreach(var type in assembly.GetTypes()) {
                    if(type.IsNested)
                        continue;
                    
                    TypeMap[type.Name] = type;
                }
            }
        }

        public static Type? GetTypeByName(string name) {
            var components = name.Split(".");

            foreach(var component in components) {
                if(Type.GetType($"{name}, {component}, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null") is Type type)
                    return type;
            }

            return null;
        }
    }
}