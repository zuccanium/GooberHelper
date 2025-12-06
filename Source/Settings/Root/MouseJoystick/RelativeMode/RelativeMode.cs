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
        
        private void clampVirtualPositionIntoCircle(Monocle.Circle circle, out bool wasOutside, out Vector2 fromCircle) {
            wasOutside = false;

            fromCircle = VirtualMousePosition - circle.Center;

            if(fromCircle.Length() > circle.Radius) {
                fromCircle = fromCircle.SafeNormalize() * circle.Radius;

                wasOutside = true;
            }

            VirtualMousePosition = circle.Center + fromCircle;
        }

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
            clampVirtualPositionIntoCircle(circle, out var wasOutside, out var fromCircle);

            //maybe setting the mouse position and handling lastMousePosition
            var newMousePositionMaybe =
                shouldLockMouse()
                    ? relativeModeSettings.UseRegularMouse && wasOutside
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
            JoystickPosition = rawJoystickPosition.Length() < relativeModeSettings.DeadzoneRadius / 100
                ? Vector2.Zero
                : rawJoystickPosition;
        }

        public override void Render() {
            var relativeModeSettings = GooberHelperModule.Settings.MouseJoystick.RelativeMode;

            var circle = relativeModeSettings.Circle.ToCircle();

            var center = new Vector2(Engine.Viewport.Width, Engine.Viewport.Height) / 2;
            var position = center + circle.Center;

            Utils.DrawHollowCircle(position, circle.Radius, Color.Red, 12);
            Utils.DrawHollowCircle(position, circle.Radius * relativeModeSettings.DeadzoneRadius / 100, Color.Lime, 4);
            
            if(!relativeModeSettings.UseRegularMouse)
                relativeCursor.DrawCentered(VirtualMousePosition + center);
        }
    }
}