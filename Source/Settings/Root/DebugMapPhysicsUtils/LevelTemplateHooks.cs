using System.Collections.Generic;
using Celeste.Editor;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Extensions;

namespace Celeste.Mod.GooberHelper.Settings.Root.DebugMapPhysicsUtils {
    public static class LevelTemplateHooks {
        [OnHook]
        private static void patch_LevelTemplate_RenderContents(On.Celeste.Editor.LevelTemplate.orig_RenderContents orig, LevelTemplate self, Camera camera, List<LevelTemplate> allLevels) {
            orig(self, camera, allLevels);

            if(!GooberHelperModule.Settings.DebugMapPhysics) return;

            if(
                self.Right < camera.Left ||
                self.Left > camera.Right ||
                self.Bottom < camera.Top ||
                self.Top > camera.Bottom
            ) return;

            self.GetExtensionFields().Fluid.Render(self.Rect.Location.ToVector2(), DebugMapThings.FluidStaticColor, DebugMapThings.FluidMovingColor);
        }

        [OnHook]
        private static void patch_LevelTemplate_Move(On.Celeste.Editor.LevelTemplate.orig_Move orig, LevelTemplate self, Vector2 relativeMove, List<LevelTemplate> allLevels, bool snap)
            => orig(self, relativeMove, allLevels, !GooberHelperModule.Settings.DebugMapPhysics);
    }
}