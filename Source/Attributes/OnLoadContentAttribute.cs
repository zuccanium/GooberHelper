using System;

namespace Celeste.Mod.GooberHelper.Attributes {
    [AttributeUsage(AttributeTargets.Method)]
    public class OnLoadContentAttribute : Attribute {
        public static void Load() {
            foreach(var type in typeof(OnLoadContentAttribute).Assembly.GetTypes()) {
                foreach(var method in type.GetMethods(Utils.BindingFlagsAll)) {
                    if(!IsDefined(method, typeof(OnLoadContentAttribute)))
                        continue;
                
                    method.Invoke(null, []);
                
                    Utils.Log($"loading content {method} on {method.DeclaringType}...");
                }
            }
        }
    }
}