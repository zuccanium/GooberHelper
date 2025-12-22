using System;
using System.Linq;
using Celeste.Editor;
using Celeste.Mod.Core;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Extensions;
using Microsoft.Xna.Framework.Input;

namespace Celeste.Mod.GooberHelper.Settings.Root.DebugMapPhysicsUtils {
    public static class MapEditorHooks {
        private static string[] generateInfoText()
            => [
                $"""
                Left Click:           Fling rooms
                Right Click:          Attract to mouse
                Right Click + Scroll: Change attraction
                
                P:                    Place fluid
                P + Scroll:           Change fluid blob size
                O:                    Randomize fluid color
                """,

                $"""
                Attraction:           {string.Format("{0:N2}", DebugMapThings.AttractStrength)}
                Fluid Blob Size:      {string.Format("{0:N0}", DebugMapThings.FluidBlobSize)}
                """
            ];

        [OnHook]
        private static void patch_MapEditor_ctor(On.Celeste.Editor.MapEditor.orig_ctor orig, MapEditor self, AreaKey area, bool reloadMapData) {
            orig(self, area, reloadMapData);

            DebugMapThings.ResetThings();
        }

        [OnHook]
        private static void patch_MapEditor_Update(On.Celeste.Editor.MapEditor.orig_Update orig, MapEditor self) {
            var previousMouseMode = self.mouseMode;
            var previousZoom = MapEditor.Camera.Zoom;

            orig(self);

            if(!GooberHelperModule.Settings.DebugMapPhysics)
                return;

            foreach(var level in self.levels)
                level.Update();


            if(self.mouseMode == MapEditor.MouseModes.Move) {
                foreach(var level in self.selection) {
                    var ext = level.GetExtensionFields();

                    ext.Velocity = MouseSmoother.GetOffset() * Engine.DeltaTime;
                    ext.BeingDragged = true;
                }
            }

            if(previousMouseMode == MapEditor.MouseModes.Move && self.mouseMode == MapEditor.MouseModes.Hover) {
                foreach(var level in self.selection) {
                    var ext = level.GetExtensionFields();

                    ext.BeingDragged = false;
                }
            }

            if(MInput.Keyboard.Check(Keys.P)) {
                DebugMapThings.FluidBlobSize = Math.Max(0, DebugMapThings.FluidBlobSize + Math.Sign(MInput.Mouse.WheelDelta));
                MapEditor.Camera.Zoom = previousZoom;

                //why did i name it that
                var oggy = self.TestCheck(self.mousePosition);
                
                oggy?.GetExtensionFields().Fluid.AddFluid(self.mousePosition - oggy.Rect.Location.ToVector2(), 0.05f, (int)DebugMapThings.FluidBlobSize);
            }

            if(MInput.Keyboard.Pressed(Keys.O))
                DebugMapThings.RandomizeFluidColor();

            if(MInput.Mouse.CheckRightButton) {
                DebugMapThings.AttractStrength += MInput.Mouse.WheelDelta * 0.5f * (1 + Math.Abs(DebugMapThings.AttractStrength)/1000);
                MapEditor.Camera.Zoom = previousZoom;

                foreach(var level in self.levels)
                    level.ApproachMouse(self.mousePosition, DebugMapThings.AttractStrength);
            }
        }

        [OnHook]
        private static void patch_MapEditor_Render(On.Celeste.Editor.MapEditor.orig_Render orig, MapEditor self) {
            orig(self);

            if(!CoreModule.Settings.ShowManualTextOnDebugMap || !GooberHelperModule.Settings.DebugMapPhysics)
                return;

            Draw.SpriteBatch.Begin();

            var offset = 72;
            var padding = 10;

            var panels = generateInfoText();
            var panelSizes = panels
                .Select(Draw.DefaultFont.MeasureString)
                .ToArray();

            var maxWidth = (int)panelSizes.Select(panel => panel.X).Max();

            for(var i = 0; i < panels.Length; i++) {
                var size = panelSizes[i];
                var text = panels[i];

                Draw.Rect(1920 - maxWidth, offset, maxWidth, size.Y, Color.Black * 0.8f);
                Draw.Text(Draw.DefaultFont, text, new Vector2(1920 - maxWidth, offset), Color.White);

                offset += (int)size.Y + padding;
            }
            
            Draw.SpriteBatch.End();
        }
    }
}