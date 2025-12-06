using System;
using Celeste.Mod.GooberHelper.Attributes;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Mod.GooberHelper.Settings.Root.MouseJoystick.AbsoluteMode {
    public static class AbsoluteMode {
        public static readonly float BorderResolutionScaling = 0.2f;
        private static MTexture largeCircle;
        private static MTexture largeCircleMask;

        [OnLoadContent]
        public static void LoadContent() {
            largeCircle = GFX.Gui["GooberHelper/largeCircle"];
            largeCircleMask = GFX.Gui["GooberHelper/largeCircleMask"];
        }

        public static float OverrideAxis(On.Monocle.Binding.orig_Axis orig, Binding self, int gamepadIndex, float threshold) {
            var settings = global::Celeste.Settings.Instance; //what the fuck??
    
            var circle = GooberHelperModule.Settings.MouseJoystick.AbsoluteMode.Circle.ToCircle();
            var toCircleCenter = MInput.Mouse.Position - new Vector2(Engine.Viewport.Width, Engine.Viewport.Height) / 2f - circle.Center;

            if(MInput.Mouse.Position == -Engine.Viewport.Bounds.Location.ToVector2())
                toCircleCenter = Vector2.Zero;

            if(toCircleCenter.Length() < circle.Radius)
                return orig(self, gamepadIndex, threshold);

            var normalizedVector = toCircleCenter.SafeNormalize();

            return 
                self == settings.Right
                    ? normalizedVector.X > threshold
                        ? Math.Max(normalizedVector.X, 0)
                        : 0

                : self == settings.Left
                    ? normalizedVector.X < -threshold
                        ? Math.Max(-normalizedVector.X, 0)
                        : 0
                
                : self == settings.Down
                    ? normalizedVector.Y > threshold
                        ? Math.Max(normalizedVector.Y, 0)
                        : 0

                : self == settings.Up
                    ? normalizedVector.Y < -threshold
                        ? Math.Max(-normalizedVector.Y, 0)
                        : 0
                
                : orig(self, gamepadIndex, threshold);
        }

        public static void Render() {
            var absoluteModeSettings = GooberHelperModule.Settings.MouseJoystick.AbsoluteMode;

            var circle = absoluteModeSettings.Circle.ToCircle();

            var radius = circle.Radius;

            var scale = radius / 256f;
            var position = new Vector2(Engine.Viewport.Width, Engine.Viewport.Height) / 2 + circle.Center;

            var outerColor = absoluteModeSettings.OuterColor.MultiplyByAlpha();
            var innerColor = absoluteModeSettings.InnerColor.MultiplyByAlpha();
            var borderColor = absoluteModeSettings.BorderColor.MultiplyByAlpha();
            var borderThickness = absoluteModeSettings.BorderThickness;

            largeCircle.DrawCentered(position, innerColor, scale);
            
            largeCircleMask.DrawCentered(position, outerColor, scale);
            
            //left panel
            Draw.Rect(
                new Rectangle(
                    0,
                    0,
                    (int)(position.X - radius),
                    Engine.Viewport.Height
                ), 
                outerColor
            );

            //top panel
            Draw.Rect(
                new Rectangle(
                    (int)(position.X - radius),
                    0,
                    (int)(radius * 2f),
                    (int)(position.Y - radius)
                ), 
                outerColor
            );

            //bottom panel
            Draw.Rect(
                new Rectangle(
                    (int)(position.X - radius),
                    (int)(position.Y + radius),
                    (int)(radius * 2f),
                    (int)(Engine.Viewport.Height - (position.Y - radius))
                ), 
                outerColor
            );

            //right panel
            Draw.Rect(
                new Rectangle(
                    (int)(position.X + radius),
                    0,
                    (int)(Engine.Viewport.Width - (position.X - radius)),
                    Engine.Viewport.Height
                ), 
                outerColor
            );

            Draw.Circle(position, radius, borderColor, borderThickness, (int)(radius * BorderResolutionScaling));
        }
    }
}