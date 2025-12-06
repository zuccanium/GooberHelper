using System;
using Celeste.Mod.GooberHelper.Attributes;

namespace Celeste.Mod.GooberHelper.Settings.Root.MouseJoystick.AbsoluteMode {
    public class AbsoluteMode : Mode.MouseJoystickModeHandler {
        public static readonly AbsoluteMode Instance = new();

        private static MTexture largeCircle;
        private static MTexture largeCircleMask;

        [OnLoadContent]
        public static void LoadContent() {
            largeCircle = GFX.Gui["GooberHelper/largeCircle"];
            largeCircleMask = GFX.Gui["GooberHelper/largeCircleMask"];
        }

        public override void Update() {
            var circle = GooberHelperModule.Settings.MouseJoystick.AbsoluteMode.Circle.ToCircle();
            var fromCircle = MInput.Mouse.Position - new Vector2(Engine.Viewport.Width, Engine.Viewport.Height) / 2f - circle.Center;

            if(MInput.Mouse.Position == -Engine.Viewport.Bounds.Location.ToVector2())
                fromCircle = Vector2.Zero;

            if(fromCircle.Length() < circle.Radius)
                JoystickPosition = Vector2.Zero;

            JoystickPosition = fromCircle.SafeNormalize();
        }

        public override void Render() {
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

            Utils.DrawHollowCircle(position, radius, borderColor, borderThickness);
        }
    }
}