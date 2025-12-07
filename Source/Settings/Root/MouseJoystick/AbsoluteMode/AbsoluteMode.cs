namespace Celeste.Mod.GooberHelper.Settings.Root.MouseJoystick.AbsoluteMode {
    public class AbsoluteMode : Mode.MouseJoystickModeHandler {
        public static readonly AbsoluteMode Instance = new();

        public override void Update() {
            var circle = GooberHelperModule.Settings.MouseJoystick.AbsoluteMode.Circle.ToCircle();
            var fromCircle = MInput.Mouse.Position - new Vector2(Engine.Viewport.Width, Engine.Viewport.Height) / 2f - circle.Center;

            if(MInput.Mouse.Position == -Engine.Viewport.Bounds.Location.ToVector2())
                fromCircle = Vector2.Zero;

            JoystickPosition = fromCircle.SafeNormalize();

            if(fromCircle.Length() < circle.Radius)
                JoystickPosition = Vector2.Zero;
        }

        public override void Render() {
            var absoluteModeSettings = GooberHelperModule.Settings.MouseJoystick.AbsoluteMode;
            var center = new Vector2(Engine.Viewport.Width, Engine.Viewport.Height) / 2f;

            var circle = absoluteModeSettings.Circle.ToCircle();
            var radius = circle.Radius;

            var position = center + circle.Center;

            var outerColor = absoluteModeSettings.OuterColor.MultiplyByAlpha();
            var innerColor = absoluteModeSettings.InnerColor.MultiplyByAlpha();
            var borderColor = absoluteModeSettings.BorderColor.MultiplyByAlpha();

            var borderThickness = absoluteModeSettings.BorderThickness;

            Utils.DrawTexturedCircleOuter(position, radius, outerColor);
            Utils.DrawTexturedCircleInner(position, radius, innerColor);

            Utils.DrawHollowCircle(position, radius, borderColor, borderThickness);
        }
    }
}