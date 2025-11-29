using System;

namespace Celeste.Mod.GooberHelper.Attributes {
    [AttributeUsage(AttributeTargets.Method)]
    public class OnUnloadAttribute : Attribute {
        public static void Unload() {
            foreach(var type in typeof(OnUnloadAttribute).Assembly.GetTypes()) {
                foreach(var method in type.GetMethods(Utils.BindingFlagsAll)) {
                    if(!IsDefined(method, typeof(OnUnloadAttribute)))
                        continue;
                
                    method.Invoke(null, []);
                
                    Utils.Log($"unloading {method} on {method.DeclaringType}...");
                }
            }
        }
    }
}