using System;

namespace Celeste.Mod.GooberHelper.Attributes {
    [AttributeUsage(AttributeTargets.Method)]
    public class OnLoadAttribute : Attribute {
        public static void Load() {
            foreach(var type in typeof(OnLoadAttribute).Assembly.GetTypes()) {
                foreach(var method in type.GetMethods(Utils.BindingFlagsAll)) {
                    if(!IsDefined(method, typeof(OnLoadAttribute)))
                        continue;
                
                    method.Invoke(null, []);
                
                    Utils.Log($"loading {method} on {method.DeclaringType}...");
                }
            }
        }
    }
}