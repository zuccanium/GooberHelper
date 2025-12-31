using Celeste.Mod.GooberHelper.Settings.Root.MouseJoystick;
using Celeste.Mod.GooberHelper.UI;

namespace Celeste.Mod.GooberHelper.Helpers {
    public static class MouseHelper {
        public static void UpdateMouseVisibility() {
            var settings = GooberHelperModule.Settings;

            var visibility = null
                ?? TextMenuGooberExt.ColorInput.MouseVisiblity
                ?? TextMenuGooberExt.ScreenCircle.MouseVisibility
                ?? ((Engine.Scene.Paused || Engine.Scene is not Level) && settings?.MouseJoystick?.Mode != Mode.ModeValue.None ? false as bool? : null)
                ?? (settings?.MouseJoystick?.Mode == Mode.ModeValue.Absolute ? true as bool? : null)
                ?? (settings?.MouseJoystick?.Mode == Mode.ModeValue.Relative && settings?.MouseJoystick?.RelativeMode?.UseRegularMouse == true ? true as bool? : null)
                ?? null;

            if(visibility != null)
                Engine.Instance.IsMouseVisible = visibility.Value;
        }
    }
}

// using System;
// using System.Collections.Generic;
// using System.Linq;

// namespace Celeste.Mod.GooberHelper.Helpers {
//     public static class MouseHelper {
//         public struct StackItem(string id, bool visibility) {
//             public string Id = id;
//             public bool Visibility = visibility;
//         }

//         public static List<StackItem> Stack = [];

//         public static void SetMouseVisibility(string id, bool visibility) {
//             Stack.Add(new StackItem(id, visibility));

//             Engine.Instance.IsMouseVisible = visibility;

//             Utils.Log($"{id} ADDED {visibility}");
//             Utils.Log("MOUSE STACK:");

//             for(var i = Stack.Count - 1; i >= 0; i--)
//                 Utils.Log($"Id: {Stack[i].Id}, Visibility: {Stack[i].Visibility}");
//         }

//         public static void ResetMouseVisibility(string id) {
//             for(var i = 0; i < Stack.Count; i++)
//                 if(Stack[i].Id == id) {
//                     Stack.RemoveAt(i);
                    
//                     break;
//                 }
            
//             Engine.Instance.IsMouseVisible = Stack.Last().Visibility;

//             Utils.Log($"{id} RESET ITSELF");
//             Utils.Log("MOUSE STACK:");

//             for(var i = Stack.Count - 1; i >= 0; i--)
//                 Utils.Log($"Id: {Stack[i].Id}, Visibility: {Stack[i].Visibility}");
//         }
//     }
// }