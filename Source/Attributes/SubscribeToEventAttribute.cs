using System;
using System.Collections.Generic;
using System.Reflection;

namespace Celeste.Mod.GooberHelper.Attributes {
    [AttributeUsage(AttributeTargets.Method)]
    public class SubscribeToEventAttribute : Attribute {
        public static readonly Dictionary<EventInfo, List<Delegate>> AppliedSubscriptions = [];

        public Type Type;
        public string EventName;
        public EventInfo EventInfo;
        public Delegate DelegateInstance;

        public SubscribeToEventAttribute(Type type, string eventName) {
            Type = type;
            EventName = eventName;
        }

        public void Add(MethodInfo method) {
            EventInfo = Type.GetEvent(EventName, Utils.BindingFlagsAll);
            DelegateInstance = method.CreateDelegate(EventInfo.EventHandlerType, null);

            EventInfo.GetAddMethod().Invoke(null, [DelegateInstance]);

            if(!AppliedSubscriptions.TryGetValue(EventInfo, out var appliedSubscriptionsOnEvent)) {
                appliedSubscriptionsOnEvent = [];

                AppliedSubscriptions.Add(EventInfo, appliedSubscriptionsOnEvent);
            }

            appliedSubscriptionsOnEvent.Add(DelegateInstance);
        }

        [OnLoad]
        public static void Load() {
            foreach(var type in typeof(GooberHelperModule).Assembly.GetTypes()) {
                foreach(var method in type.GetMethods(Utils.BindingFlagsAll)) {
                    if(GetCustomAttributes(method, typeof(SubscribeToEventAttribute)) is not SubscribeToEventAttribute[] attributes)
                        continue;

                    foreach(var attribute in attributes)
                        attribute.Add(method);
                }
            }
        }

        [OnUnload]
        public static void Unload() {
            foreach(var pair in AppliedSubscriptions)
                foreach(var delegateInstance in pair.Value)
                    pair.Key.GetRemoveMethod().Invoke(null, [delegateInstance]);

            AppliedSubscriptions.Clear();
        }
    }
}