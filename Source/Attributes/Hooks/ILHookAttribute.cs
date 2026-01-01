using System;
using System.Linq;
using System.Reflection;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

namespace Celeste.Mod.GooberHelper.Attributes.Hooks {
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ILHookAttribute : AbstractHookAttribute {
        public ILHookAttribute()
            => Targets = [];
    
        public ILHookAttribute(Type declaringType, string methodName)
            => Targets = [(declaringType, methodName)];

        public ILHookAttribute(string assemblyName, string typeName, string methodName) {
            //ik this is slow and stupid but i really want the warning to go away
            //theres only a few hooks that do this, so it shouldnt be too bad
            if(!Everest.Modules.Any(item => item.GetType().Assembly.FullName.StartsWith(assemblyName))) {
                Utils.Log($"{assemblyName} not loaded while loading an ilhook! returning");

                return;
            }

            if(Type.GetType($"{typeName}, {assemblyName}, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null") is not Type type) {
                Utils.Log($"couldnt find type {typeName}!");

                return;
            }

            Targets = [(type, methodName)];
        }

        protected override void ApplyHooks(MethodInfo method) {
            foreach(var target in ResolvedTargets) {
                Utils.Log($"applying {method} to target {target}...");

                if(method.TryCreateDelegate<ILContext.Manipulator>() is ILContext.Manipulator manipulator) {
                    AppliedHooks.Add(new ILHook(target, manipulator));
                } else {
                    Logger.Error("GooberHelper", $"failed to create ILContext.Manipulator from {method} in {method.DeclaringType}!");
                }
            }
        }

        protected override MethodBase TransformMethod(MethodBase method) {
            if(method is not MethodInfo methodInfo)
                return method;

            if(methodInfo.GetStateMachineTarget() is MethodInfo stateMachineTarget)
                return stateMachineTarget;

            return method;
        }
    }
}