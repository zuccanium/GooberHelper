using Celeste.Editor;
using Celeste.Mod.GooberHelper.Attributes.Hooks;

namespace Celeste.Mod.GooberHelper.Settings.Toggles.DebugMapPhysics {
    public class MouseSmoother {
        private static Vector2[] mouseSmoothingBuffer = new Vector2[5];
        private static int mouseSmoothingBufferIndex = 0;

        [OnHook]
        private static void patch_MapEditor_UpdateMouse(On.Celeste.Editor.MapEditor.orig_UpdateMouse orig, MapEditor self) {
            var previousMousePosition = self.mousePosition + Vector2.Zero; //dont just copy the pointer

            orig(self);
        
            var mouseOffset = self.mousePosition - previousMousePosition;
            
            mouseSmoothingBuffer[mouseSmoothingBufferIndex] = mouseOffset;
            mouseSmoothingBufferIndex++;
            mouseSmoothingBufferIndex %= mouseSmoothingBuffer.Length;
        }

        public Vector2 GetOffset() {
            var sum = Vector2.Zero;

            foreach(var vector in mouseSmoothingBuffer)
                sum += vector;

            return sum / mouseSmoothingBuffer.Length;
        }
    }
}