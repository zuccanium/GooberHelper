using System;
using System.Reflection;
using MonoMod.RuntimeDetour;

#nullable enable

namespace Celeste.Mod.GooberHelper.Attributes.Hooks {
    //i dont know why you would ever want to use multiple on hooks but okay have fun
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class OnHookAttribute : BaseHookAttribute {
        public OnHookAttribute()
            => Targets = [];
    
        public OnHookAttribute(Type declaringType, string methodName)
            => Targets = [(declaringType, methodName)];

        protected override void ApplyHooks(MethodInfo method) {
            foreach(var target in ResolvedTargets) {
                Utils.Log($"applying {method} to target {target}...");
                
                AppliedHooks.Add(new Hook(target, method));
            }
        }
    }
}