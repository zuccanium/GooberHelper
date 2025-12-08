using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Celeste.Mod.GooberHelper.Helpers;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

#nullable enable

namespace Celeste.Mod.GooberHelper.Attributes.Hooks {
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public abstract class BaseHookAttribute : Attribute {
        public static readonly List<object> AppliedHooks = []; //ILHook or Hook
    
        //i could definitely just split this part but Qhatever
        public (Type DeclaringType, string MethodName)[] Targets = [];
        public List<MethodBase> ResolvedTargets = [];
    
        public static Type? GetType(string[] components, int startIndex, out int endIndex) {
            Utils.Log($"getting type with kinda signature {string.Join("_", components[startIndex..])}...");

            endIndex = default;

            if(!TypeHelper.TypeMap.TryGetValue(components[startIndex], out var current)) {
                Logger.Error("GooberHelper", $"couldnt find type {components[startIndex]}");
                
                return null;
            }

            var index = startIndex + 1;

            while(index < components.Length) {
                Utils.Log($"looking at {components.ElementAtOrDefault(index)}...");

                var component = components.ElementAtOrDefault(index);

                if(component is null || current.GetNestedType(component) is not Type nestedType)
                    break;

                current = nestedType;
                index++;
            }

            endIndex = index;

            return current;
        }

        public MethodBase? GetMethod(Type declaringType, string[] components, int startIndex) {
            Utils.Log($"getting method from type {declaringType} with kinda signature {string.Join("_", components[startIndex..])}...");
            
            var index = startIndex;

            var isProperty = false;

            if(components[index] == "get" || components[index] == "set") {
                index++;

                isProperty = true;
            }

            var name = (isProperty ? components[index - 1] + "_" : null) + components[index];
            var orig_name = $"orig_{name}";

            var methods = Array.Empty<MethodBase>();

            if(components[index] == "ctor") {
                methods = declaringType.GetConstructors(Utils.BindingFlagsAll);
            } else {
                var origAndNormalMethods = declaringType.GetMethods(Utils.BindingFlagsAll)
                    .Where(method => method.Name == name || method.Name == orig_name);
            
                var normalMethods = origAndNormalMethods
                    .Where(method => method.Name == name);

                methods = origAndNormalMethods.Count() == normalMethods.Count()
                    ? [..normalMethods]
                    : [..origAndNormalMethods.Where(method => method.Name == orig_name)];
            }

            if (methods.Length == 1)
                return TransformMethod(methods[0]);

            foreach(var method in methods) {
                Utils.Log($"checking method {method} for compatibility with {string.Join("_", components[startIndex..])}...");

                var parameters = method.GetParameters();
                var failed = false;

                for(var i = 0; i < parameters.Length; i++) {
                    var parameter = parameters[i];
                    var component = components.ElementAtOrDefault(i + index + 1);

                    if(component is null)
                        continue;
                    
                    //i hate you microsoft
                    if(parameter.ParameterType == typeof(float) && component == "float")
                        continue;

                    if(!parameter.ParameterType.Name.Contains(component, StringComparison.OrdinalIgnoreCase)) {
                        failed = true;

                        break;
                    }
                }

                if(failed)
                    continue;
            
                return TransformMethod(method);
            }
        
            return null;
        }

        public void GetTargetFromMethodName(string methodName) {
            var components = methodName.Split("_");

            Utils.Log($"getting target from method name {methodName}");

            if(GetType(components, 1, out var endIndex) is not Type type) {
                Logger.Error("GooberHelper", $"couldnt resolve target type of hooking method {methodName}");

                return;
            }

            Utils.Log($"found {type} to be the type");
        
            if(GetMethod(type, components, endIndex) is not MethodBase method) {
                Logger.Error("GooberHelper", $"couldnt resolve target method of hooking method {methodName}");

                return;
            }

            Utils.Log($"found {method} to be the method");

            ResolvedTargets.Add(method);
        }

        public void ResolveTargets(MethodInfo method) {
            if(Targets.Length == 0)
                Logger.Warn("GooberHelper", $"there arent any targets on the hook attribute for method {method.Name} on {method.DeclaringType} :quoggert: is this a mistake?");

            foreach(var target in Targets) {
                if(GetMethod(target.DeclaringType, target.MethodName.Split("_"), 0) is not MethodBase targetMethod) {
                    Logger.Error("GooberHelper", $"couldnt resolve target method of target {$"({target.DeclaringType}, {target.MethodName})"}");

                    return;
                }

                ResolvedTargets.Add(targetMethod);
            }
        }

        protected virtual void ApplyHooks(MethodInfo method) {}
        protected virtual MethodBase TransformMethod(MethodBase method)
            => method;

        [OnLoad]
        public static void Load() {
            foreach(var type in typeof(BaseHookAttribute).Assembly.GetTypes()) {
                foreach(var method in type.GetMethods(Utils.BindingFlagsAll)) {
                    if(method.GetCustomAttributes<BaseHookAttribute>() is not BaseHookAttribute[] attributes)
                        continue;

                    foreach(var attribute in attributes) {       
                        if(method.Name.StartsWith("patch_")) {
                            attribute.GetTargetFromMethodName(method.Name);
                        } else {
                            attribute.ResolveTargets(method);
                        }

                        attribute.ApplyHooks(method);
                    }
                }
            }
        }

        [OnUnload]
        public static void Unload() {
            foreach(var hook in AppliedHooks) {
                //i love naming variables
                if(hook is Hook hookHook) {
                    hookHook.Dispose();
                } else if(hook is ILHook ilHookHook) {
                    ilHookHook.Dispose();
                } else {
                    throw new Exception("what the actual hell are you doing ⁉⁉⁉ why is the hook neither an ilhook nor a hook");
                }
            }

            AppliedHooks.Clear();
        }
    }
}