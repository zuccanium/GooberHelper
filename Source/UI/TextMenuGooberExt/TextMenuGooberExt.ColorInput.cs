using System;

namespace Celeste.Mod.GooberHelper.UI {
    public static partial class TextMenuGooberExt {
        public class ColorInput : TextMenu.Item {
            public abstract class AbstractDraggableThing {
                public readonly float Threshold = 10f;

                public MTexture Texture;
                public MTexture MoverTexture;

                public Vector2 ScreenPosition = Vector2.Zero;

                public bool Dragging = false;
                public bool Selected = false;

                public Vector2 MoverPosition;

                public AbstractDraggableThing(Vector2 moverPosition, MTexture texture, MTexture moverTexture) {
                    Texture = texture;
                    MoverTexture = moverTexture;
                    MoverPosition = moverPosition;
                }

                public abstract Vector2 FromScreen(Vector2 position, out bool outOfBounds);
                public abstract Vector2 ToScreen(Vector2 position);
                public abstract bool GetSelected(Vector2 mousePosition, Vector2 moverToScreen);

                public virtual void Update() {
                    var mousePosition = MInput.Mouse.Position;
                    var mouseFromScreen = FromScreen(mousePosition, out var outOfBounds);
                    var moverToScreen = ToScreen(MoverPosition);

                    Selected = GetSelected(mousePosition, moverToScreen);

                    if(!draggingSomething && MInput.Mouse.CheckLeftButton && !outOfBounds)
                        Dragging = draggingSomething = true;

                    if(Dragging && !MInput.Mouse.CheckLeftButton)
                        Dragging = draggingSomething = false;

                    if(Dragging)
                        MoverPosition = mouseFromScreen;
                }

                public virtual void Render(Vector2 position, float easedEase, ref float offset, Color? color = null, float gap = 0) {
                    ScreenPosition = position + Vector2.UnitX * offset;
                    
                    Draw.SpriteBatch.Draw(
                        Texture.Texture.Texture,
                        ScreenPosition,
                        new Rectangle(0, 0, Texture.Width, (int)(Texture.Height * easedEase)),
                        color ?? Color.White
                    );

                    if(ToScreen(MoverPosition).Y <= position.Y + Texture.Height * easedEase + 2) //2 for leniency
                        MoverTexture.DrawCentered(ToScreen(MoverPosition), Color.White, Selected ? 1.2f : 1f);

                    offset += Texture.Width + gap;
                }
            }

            public class DraggableCircle : AbstractDraggableThing {
                public DraggableCircle(Vector2 moverPosition, MTexture texture, MTexture moverTexture) : base(moverPosition, texture, moverTexture) {}

                public override Vector2 FromScreen(Vector2 position, out bool outOfBounds) {
                    var raw = (position - ScreenPosition) / new Vector2(Texture.Width, Texture.Height) * 2 - new Vector2(1, 1);

                    outOfBounds = raw.Length() > 1f;

                    return raw.SafeNormalize() * Math.Clamp(raw.Length(), 0f, 1f);
                }

                public override Vector2 ToScreen(Vector2 position) 
                    => (position * 0.5f + new Vector2(0.5f, 0.5f)) * new Vector2(Texture.Width, Texture.Height) + ScreenPosition;

                public override bool GetSelected(Vector2 mousePosition, Vector2 moverToScreen)
                    => Vector2.Distance(ToScreen(MoverPosition), mousePosition) < Threshold || Dragging;
            }

            public class DraggableSlider : AbstractDraggableThing {
                public DraggableSlider(float moverPosition, MTexture texture, MTexture moverTexture) : base(new Vector2(0, moverPosition), texture, moverTexture) {}

                public override Vector2 FromScreen(Vector2 position, out bool outOfBounds) {
                    var raw = (position - ScreenPosition) / new Vector2(Texture.Width, Texture.Height);

                    outOfBounds = raw.X < 0f || raw.X > 1f || raw.Y < 0f || raw.Y > 1f;

                    return new Vector2(0, Math.Clamp(1f - raw.Y, 0f, 1f));
                }

                public override Vector2 ToScreen(Vector2 position) 
                    => new Vector2(0f, (1f - position.Y) * Texture.Height) + ScreenPosition + new Vector2(Texture.Width / 2, 0f);

                public override bool GetSelected(Vector2 mousePosition, Vector2 toScreen) =>
                    Math.Abs(toScreen.X - mousePosition.X) < Texture.Width / 2 + Threshold && Math.Abs(toScreen.Y - mousePosition.Y) < Threshold || Dragging;
            }

            public static bool? MouseVisiblity;

            public enum ColorDiplayMode {
                Rgba,
                Hex,
                Hsva
            }

            public static readonly int GapWidth = 20;

            public Color UnselectedColor = Color.White;
            public string Label;
            public Vector4 VectorColor;
            
            public bool Editing = false;
            private float ease = 0;
            private float easedEase = 0;
            private ColorDiplayMode displayMode = ColorDiplayMode.Rgba;

            private MTexture hueWheel = GFX.Gui["GooberHelper/hueWheel"];
            private MTexture brightnessSlider = GFX.Gui["GooberHelper/brightnessSlider"];
            private MTexture alphaSlider = GFX.Gui["GooberHelper/alphaSlider"];
            
            private MTexture sliderMover = GFX.Gui["GooberHelper/sliderMover"];
            private MTexture hueWheelMover = GFX.Gui["GooberHelper/hueWheelMover"];
            
            private DraggableCircle draggableHueWheel;
            private DraggableSlider draggableBrightnessSlider;
            private DraggableSlider draggableAlphaSlider;
            private static bool draggingSomething = false;

            public Action<Color> OnValueChange;
            
            public ColorInput(string label, Color color) {
                Label = label;
                VectorColor = color.ToVector4();

                Selectable = true;

                OnLeave += onLeave;
            }

            public override float Height()
                => ActiveFont.LineHeight + hueWheel.Height * easedEase;

            public override float LeftWidth()
                => ActiveFont.Measure(Label).X;

            public override float RightWidth()
                => ColorCodePartWidth() + ColorDisplayPartWidth();

            //color code
            public string GetColorCodeString()
                => displayMode switch {
                    ColorDiplayMode.Rgba => VectorColor.ToStringRgba(),
                    ColorDiplayMode.Hex => VectorColor.ToStringHex(),
                    ColorDiplayMode.Hsva => VectorColor.RgbToHsv().ToStringHsva(),
                    _ => "what the hell????"
                };

            public float ColorCodePartWidth()
                => ActiveFont.Measure(GetColorCodeString()).X * 0.8f;

            //color square thing
            public float ColorDisplayPartWidth()
                => ActiveFont.LineHeight;

            //functionality
            public override void LeftPressed() {
                displayMode = Utils.RotateEnum(displayMode, -1);

                Audio.Play(SFX.ui_main_button_select);
            }
            
            public override void RightPressed() {
                displayMode = Utils.RotateEnum(displayMode, 1);
                
                Audio.Play(SFX.ui_main_button_select);
            }

            public override void ConfirmPressed()
                => StartEditing();

            private void onLeave()
                => StopEditing();

            public void StartEditing() {
                Editing = true;

                if(draggableHueWheel == null) {
                    var hsv = VectorColor.RgbToHsv();

                    draggableHueWheel = new DraggableCircle(Calc.AngleToVector(-hsv.X * MathF.Tau, hsv.Y), hueWheel, hueWheelMover);
                    draggableBrightnessSlider = new DraggableSlider(hsv.Z, brightnessSlider, sliderMover);
                    draggableAlphaSlider = new DraggableSlider(VectorColor.W, alphaSlider, sliderMover);
                }

                MouseVisiblity = true;

                Audio.Play(SFX.ui_main_button_select);
            }

            public void StopEditing() {
                Editing = false;
                draggingSomething = false;

                MouseVisiblity = null;
            }

            public override void Update() {
                if(Input.MenuCancel.Pressed) {
                    Utils.Log("AAAAA EXITING");

                    StopEditing();

                    ease = 0;
                    easedEase = 0;
                    
                    return;
                }

                ease = Calc.Approach(ease, Editing ? 1f : 0f, Engine.RawDeltaTime * 4f);
                easedEase = Ease.QuadOut(ease);

                if(ease > 0f && draggableHueWheel != null) {
                    draggableHueWheel.Update();
                    draggableBrightnessSlider.Update();
                    draggableAlphaSlider.Update();

                    var hsv = new Vector4(
                        -draggableHueWheel.MoverPosition.Angle() / MathF.Tau + 1,
                        draggableHueWheel.MoverPosition.Length(),
                        draggableBrightnessSlider.MoverPosition.Y,
                        draggableAlphaSlider.MoverPosition.Y
                    );

                    VectorColor = hsv.HsvToRgb();
                    OnValueChange(VectorColor.ToColor());
                }
            }

            public override void Render(Vector2 position, bool highlighted) {
                var alpha = Container.Alpha;
                var strokeColor = Color.Black * (alpha * alpha * alpha);
                var color = Disabled
                    ? Color.DarkSlateGray
                    : highlighted
                        ? Container.HighlightColor * alpha
                        : UnselectedColor * alpha;
                
                position.Y = position.Y - Height() / 2 + ActiveFont.LineHeight / 2;

                ActiveFont.DrawOutline(Label, position, new Vector2(0f, 0.5f), Vector2.One, color, 2f, strokeColor);

                var colorCodePartWidth = ColorCodePartWidth();
                var colorDisplayPartWidth = ColorDisplayPartWidth();

                var left = Container.Width - colorDisplayPartWidth - GapWidth - colorCodePartWidth;

                var colorDisplayRectangleSize = new Vector2(ColorDisplayPartWidth(), ActiveFont.LineHeight) * 0.8f;

                ActiveFont.DrawOutline(
                    GetColorCodeString(),
                    position + new Vector2(left + colorCodePartWidth / 2f, 0f),
                    new Vector2(0.5f, 0.5f),
                    Vector2.One * 0.8f,
                    Color.Lerp(VectorColor.ToColor().MultiplyByAlpha(), Color.White, highlighted ? 0.25f : 0.5f),
                    2f,
                    strokeColor
                );

                Draw.Rect(
                    position + new Vector2(left + colorCodePartWidth + 20f + GapWidth, 0f) - colorDisplayRectangleSize / 2f,
                    colorDisplayRectangleSize.X,
                    colorDisplayRectangleSize.Y,
                    VectorColor.ToColor().MultiplyByAlpha()
                );

                if(ease > 0f && draggableHueWheel != null) {
                    var gap = 20;

                    var offset = 0f;
                    var offsetPosition = position + new Vector2(0, ActiveFont.LineHeight / 2 + 10);

                    var hsv = VectorColor.RgbToHsv();

                    draggableHueWheel?.Render(offsetPosition, easedEase, ref offset, Calc.HsvToColor(0, 0, hsv.Z), gap);
                    draggableBrightnessSlider?.Render(offsetPosition, easedEase, ref offset, Calc.HsvToColor(hsv.X, hsv.Y, 1), gap);
                    draggableAlphaSlider?.Render(offsetPosition, easedEase, ref offset, Calc.HsvToColor(hsv.X, hsv.Y, hsv.Z), gap);
                }
            }
        }
    }
}