using System;

namespace Celeste.Mod.GooberHelper.Attributes {
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class OnSomethingAttribute<T> : Attribute {
        public static void InvokeOnTargets() {
            foreach(var type in typeof(GooberHelperModule).Assembly.GetTypes()) {
                foreach(var method in type.GetMethods(Utils.BindingFlagsAll)) {
                    if(!IsDefined(method, typeof(T)))
                        continue;
                
                    method.Invoke(null, []);
                
                    Utils.Log($"{typeof(T)} is invoking {method} on {method.DeclaringType}...");
                }
            }
        }
    }
}