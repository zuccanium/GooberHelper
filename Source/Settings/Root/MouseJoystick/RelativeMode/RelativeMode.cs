using System;
using Celeste.Mod.GooberHelper.Attributes;

namespace Celeste.Mod.GooberHelper.Settings.Root.MouseJoystick.RelativeMode {
    public class RelativeMode : Mode.MouseJoystickModeHandler {
        public static readonly RelativeMode Instance = new();
        
        private static MTexture relativeCursor;

        public Vector2 VirtualMousePosition = Vector2.Zero;
        private Vector2? lastMousePosition;

        [OnLoadContent]
        public static void LoadContent()
            => relativeCursor = GFX.Gui["GooberHelper/relativeCursor"];

        private static bool shouldLockMouse()
            => !Engine.Scene.Paused;

        private Vector2 getMouseDelta()
            => lastMousePosition is Vector2 lastMousePositionActual
                ? MInput.Mouse.Position - lastMousePositionActual
                : MInput.Mouse.Position;

        //ternary expression menace
        public override void Update() {
            var relativeModeSettings = GooberHelperModule.Settings.MouseJoystick.RelativeMode;

            var center = new Vector2(Engine.Viewport.Width, Engine.Viewport.Height) / 2f;
            var circle = relativeModeSettings.Circle.ToCircle();

            //updating the virtual mouse position
            VirtualMousePosition =
                MInput.Mouse.Position == -Engine.Viewport.Bounds.Location.ToVector2()
                    ? circle.Center
                
                : relativeModeSettings.UseRegularMouse
                    ? MInput.Mouse.Position - center
                
                : !relativeModeSettings.UseRegularMouse
                    ? VirtualMousePosition + getMouseDelta()
                
                : throw new InvalidOperationException();


            //processing the virtual mouse position
            var needsReset = false;
            var fromCircle = VirtualMousePosition - circle.Center;

            //i love object oriented programming
            ClampBehavior.ModifyVirtualMousePosition(circle, ref needsReset, ref fromCircle);
            ClickBehavior.ModifyVirtualMousePosition(circle, ref needsReset, ref fromCircle);

            VirtualMousePosition = fromCircle + circle.Center;

            //maybe setting the mouse position and handling lastMousePosition
            var newMousePositionMaybe =
                shouldLockMouse()
                    ? relativeModeSettings.UseRegularMouse && needsReset
                        ? VirtualMousePosition + center

                    : !relativeModeSettings.UseRegularMouse
                        ? center

                    : null
                : null as Vector2?;
            
            if(newMousePositionMaybe is Vector2 newMousePosition)
                MInput.Mouse.Position = newMousePosition;

            lastMousePosition = newMousePositionMaybe ?? MInput.Mouse.Position;

            var rawJoystickPosition = fromCircle / circle.Radius;

            //joystick stuff
            JoystickPosition = rawJoystickPosition.Length() < relativeModeSettings.DeadzoneRadius / 100f
                ? Vector2.Zero
                : rawJoystickPosition;
        }

        public override void Render() {
            var relativeModeSettings = GooberHelperModule.Settings.MouseJoystick.RelativeMode;
            var center = new Vector2(Engine.Viewport.Width, Engine.Viewport.Height) / 2f;

            var circle = relativeModeSettings.Circle.ToCircle();
            var radius = circle.Radius;
            var deadzoneRadius = circle.Radius * relativeModeSettings.DeadzoneRadius / 100f;

            var position = center + circle.Center;

            var outerColor = relativeModeSettings.OuterColor.MultiplyByAlpha();
            var borderColor = relativeModeSettings.BorderColor.MultiplyByAlpha();
            var innerColor = relativeModeSettings.InnerColor.MultiplyByAlpha();
            var deadzoneBorderColor = relativeModeSettings.DeadzoneBorderColor.MultiplyByAlpha();

            var borderThickness = relativeModeSettings.BorderThickness;
            var deadzoneBorderThickness = relativeModeSettings.DeadzoneBorderThickness;

            Utils.DrawTexturedCircleOuter(position, radius, outerColor);
            Utils.DrawTexturedCircleInner(position, radius, innerColor);

            Utils.DrawHollowCircle(position, radius, borderColor, borderThickness);
            
            Utils.DrawHollowCircle(position, deadzoneRadius, deadzoneBorderColor, deadzoneBorderThickness);
            
            if(!relativeModeSettings.UseRegularMouse)
                relativeCursor.DrawCentered(VirtualMousePosition + center);
        }
    }
}