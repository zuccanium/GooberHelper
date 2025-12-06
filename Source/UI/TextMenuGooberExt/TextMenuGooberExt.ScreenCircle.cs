
using System;
using System.Collections.Generic;

namespace Celeste.Mod.GooberHelper.UI {
    public static partial class TextMenuGooberExt {
        public class ScreenCircle : TextMenu.Item {
            [Tracked(false)]
            public class ScreenCircleMover : Entity {
                public static readonly Color CircleColor = Color.Red;
                public static readonly Color EdgeColor = Color.Cyan;
                public static readonly Color CornerColor = Color.DeepSkyBlue;
                public static readonly Color ScalingOriginColor = Color.Lime;
                public static readonly Color SelectionHighlightColor = Color.White * 0.2f;
                public static readonly Color SnapColor = Color.Red;

                public static readonly float ProximityThreshold = 10;
                public static readonly float PositionSnapThreshold = 30;
                
                public ScreenCircle Parent;
                public bool Editing = false;
                
                private bool wasMouseVisible = false;

                private bool inRect = false;
                private int selectedSide = -1;
                private int selectedCorner = -1;
                private List<Vector2> vertices;
                
                private Vector2 draggingOffset;
                private bool dragging = false;
                private float? horizontalSnapLine;
                private float? verticalSnapLine;

                private Vector2 scalingOrigin;
                private Vector2 scalingAxis;
                private bool scaling = false;
                private bool isScalingFromCorner = false;
                private int scalingCorner = -1;
                private int scalingSide = -1;

                //cached
                private static Circle circle;
                private static Vector2 centeredMousePosition;
                private static Vector2 screenCenter;
                
                public ScreenCircleMover(ScreenCircle parent) : base() {
                    Parent = parent;
                    Tag = Tags.HUD | Tags.Global | Tags.PauseUpdate;
                    Depth = Depths.Below;

                    setCache();
                    setVertices();
                }

                public void StartEditing() {
                    Utils.Log("started editing");

                    Parent.Container.Visible = false;
                    Parent.Container.Active = false;

                    wasMouseVisible = Engine.Instance.IsMouseVisible;
                    Engine.Instance.IsMouseVisible = true;

                    Editing = true;

                    screenCenter = new Vector2(Engine.Viewport.Width, Engine.Viewport.Height) / 2;

                    Audio.Play(SFX.ui_main_button_select);
                }

                public void StopEditing() {
                    Utils.Log("stopped editing");

                    Parent.Container.Visible = true;
                    Parent.Container.Active = true;

                    Engine.Instance.IsMouseVisible = wasMouseVisible;
                    
                    Editing = false;

                    inRect = false;
                    selectedCorner = -1;
                    selectedSide = -1;
                    scaling = false;
                    dragging = false;

                    Audio.Play(SFX.ui_main_button_back);
                }

                private void setCache() {
                    screenCenter = new Vector2(Engine.Viewport.Width, Engine.Viewport.Height) / 2;
                    circle = Parent.Circle;
                    centeredMousePosition = MInput.Mouse.Position - screenCenter;
                }

                private void setVertices() {
                    vertices = new List<Vector2>() {
                        circle.TopLeft,
                        circle.TopRight,
                        circle.BottomRight,
                        circle.BottomLeft
                    };
                }

                private bool trySnapCirclePositionVertical(out float? snapLine) {
                    var lines = new List<float>() {
                        -Engine.Viewport.Bounds.Width / 2,
                        0,
                        Engine.Viewport.Bounds.Width / 2,
                    };
                    
                    foreach(var line in lines) {
                        snapLine = line;

                        if(Math.Abs(circle.Left - line) < PositionSnapThreshold) {
                            circle.Left = line;

                            return true;
                        }

                        if(Math.Abs(circle.CenterX - line) < PositionSnapThreshold) {
                            circle.CenterX = line;

                            return true;
                        }

                        if(Math.Abs(circle.Right - line) < PositionSnapThreshold) {
                            circle.Right = line;

                            return true;
                        }
                    }

                    snapLine = null;

                    return false;
                }

                private bool trySnapCirclePositionHorizontal(out float? snapLine) {
                    var lines = new List<float>() {
                        -Engine.Viewport.Bounds.Height / 2,
                        0,
                        Engine.Viewport.Bounds.Height / 2,
                    };
                    
                    foreach(var line in lines) {
                        snapLine = line;

                        if(Math.Abs(circle.Top - line) < PositionSnapThreshold) {
                            circle.Top = line;

                            return true;
                        }

                        if(Math.Abs(circle.CenterY - line) < PositionSnapThreshold) {
                            circle.CenterY = line;

                            return true;
                        }

                        if(Math.Abs(circle.Bottom - line) < PositionSnapThreshold) {
                            circle.Bottom = line;

                            return true;
                        }
                    }

                    snapLine = null;

                    return false;
                }

                private void handleDragging() {
                    if(!scaling && MInput.Mouse.CheckLeftButton && !dragging && inRect) {
                        dragging = true;
                        draggingOffset = circle.Center - centeredMousePosition;
                    }

                    if(!MInput.Mouse.CheckLeftButton && dragging) {
                        dragging = false;

                        horizontalSnapLine = null;
                        verticalSnapLine = null;
                    }

                    if(dragging) {
                        circle.Center = centeredMousePosition + draggingOffset;

                        trySnapCirclePositionHorizontal(out horizontalSnapLine);
                        trySnapCirclePositionVertical(out verticalSnapLine);
                    }
                }

                private void handleScaling() {
                    if(!dragging && MInput.Mouse.CheckLeftButton && !scaling) {
                        if(selectedCorner != -1) {
                            scaling = true;
                            isScalingFromCorner = true;
                            scalingOrigin = vertices[(selectedCorner + 2) % 4];
                            scalingAxis = Calc.AngleToVector(-3f * MathF.PI / 4f + selectedCorner * MathF.PI / 2f, 1f);
                            scalingCorner = selectedCorner;
                        } else if(selectedSide != -1) {
                            scaling = true;
                            isScalingFromCorner = false;
                            scalingOrigin = Vector2.Lerp(vertices[(selectedSide + 2) % 4], vertices[(selectedSide + 3) % 4], 0.5f);
                            scalingAxis = Calc.AngleToVector(MathF.PI / 2f + selectedSide * MathF.PI / 2f, 1f);
                            scalingSide = selectedSide;
                        }
                    }

                    if(!MInput.Mouse.CheckLeftButton && scaling)
                        scaling = false;

                    if(scaling) {
                        var flattenedMousePosition = (centeredMousePosition - scalingOrigin).ProjectOnto(scalingAxis) + scalingOrigin;

                        var diameter = Vector2.Distance(flattenedMousePosition, scalingOrigin);

                        circle.Width = diameter / (isScalingFromCorner ? MathF.Sqrt(2) : 1);
                        circle.Center = Vector2.Lerp(scalingOrigin, flattenedMousePosition, 0.5f);
                    }
                }

                private void handleSelection() {
                    var toLeft = centeredMousePosition.X - circle.Left;
                    var toRight = circle.Right - centeredMousePosition.X;
                    var toTop = centeredMousePosition.Y - circle.Top;
                    var toBottom = circle.Bottom - centeredMousePosition.Y;

                    inRect = toLeft > 0 && toRight > 0 && toTop > 0 && toBottom > 0;

                    selectedSide = -1;

                    var bestSideDistance = ProximityThreshold;

                    var candidates = new List<(float distance, bool isPossibility)>() {
                        (toTop, toLeft > -ProximityThreshold && toRight > -ProximityThreshold),
                        (toRight, toTop > -ProximityThreshold && toBottom > -ProximityThreshold),
                        (toBottom, toLeft > -ProximityThreshold && toRight > -ProximityThreshold),
                        (toLeft, toTop > -ProximityThreshold && toBottom > -ProximityThreshold),
                    };

                    for(var i = 0; i < candidates.Count; i++) {
                        var (distance, isPossibility) = candidates[i];
                        
                        if(isPossibility && Math.Abs(distance) < bestSideDistance) {
                            bestSideDistance = distance;
                            selectedSide = i;
                        }
                    }

                    selectedCorner = -1;

                    for(var i = 0; i < vertices.Count; i++) {
                        if(Vector2.Distance(vertices[i], centeredMousePosition) < ProximityThreshold) {
                            selectedCorner = i;
                            
                            break;
                        }
                    }
                }

                public override void Update() {
                    base.Update();

                    if(!Editing)
                        return;

                    if(Input.MenuCancel || Input.ESC) {
                        Input.MenuCancel.ConsumePress();
                        Input.ESC.ConsumePress();
                        
                        StopEditing();

                        return;
                    }

                    setCache();

                    handleScaling();
                    handleDragging();

                    setVertices();

                    handleSelection();

                    //why is one of them static and one of them instance
                    //this pisses me off 😭
                    circle.Radius = MathF.Round(circle.Radius);
                    circle.Position = circle.Position.Round();

                    Parent.OnValueChange(circle);
                }

                public override void Render() {
                    base.Render();

                    //snapping lines
                    if(horizontalSnapLine is float horizontalSnapLineButReal)
                        Draw.Line(
                            new Vector2(Engine.Viewport.Width / 2, horizontalSnapLineButReal) + screenCenter,
                            new Vector2(-Engine.Viewport.Width / 2, horizontalSnapLineButReal) + screenCenter,
                            SnapColor,
                            2
                        );

                    if(verticalSnapLine is float verticalSnapLineButReal)
                        Draw.Line(
                            new Vector2(verticalSnapLineButReal, Engine.Viewport.Height / 2) + screenCenter,
                            new Vector2(verticalSnapLineButReal, -Engine.Viewport.Height / 2) + screenCenter,
                            SnapColor,
                            2
                        );

                    //actual circle stuff
                    Draw.Circle(
                        circle.Position + screenCenter,
                        circle.Radius,
                        CircleColor,
                        4,
                        24
                    );
                    
                    if(inRect || selectedSide != -1 || selectedCorner != -1)
                        Draw.Rect(
                            circle.Left + screenCenter.X,
                            circle.Top + screenCenter.Y,
                            circle.Width,
                            circle.Height,
                            SelectionHighlightColor
                        );

                    for(var i = 0; i < vertices.Count; i++) {
                        var start = vertices[i];
                        var end = vertices[(i + 1) % 4];

                        var thickness = i == selectedSide || scaling && i == scalingSide
                            ? 12
                            : 4;

                        Draw.Line(start + screenCenter, end + screenCenter, EdgeColor, thickness);
                    }

                    for(var i = 0; i < vertices.Count; i++) {
                        var vertex = vertices[i];

                        var radius = i == selectedCorner || scaling && i == scalingCorner
                            ? 12
                            : 8;

                        Utils.DrawFilledCircle(vertex + screenCenter, radius, CornerColor, 20);
                    }

                    if(scaling)
                        Utils.DrawFilledCircle(scalingOrigin + screenCenter, 8, ScalingOriginColor, 20);
                }
            }

            public static readonly int GapWidth = 25;

            public Color UnselectedColor = Color.White;
            public string Label;
            public Circle Circle;
            
            private float sine;
            private ScreenCircleMover moverEntity;
            private bool editing = false;

            public Action<Circle> OnValueChange;

            public ScreenCircle(string label, Circle circle) : base() {
                Label = label;
                Circle = circle;

                Selectable = true;

                OnEnter += onEnter;
                OnLeave += onLeave;
            }

            public override float Height()
                => ActiveFont.LineHeight;

            public override float LeftWidth()
                => ActiveFont.Measure(Label).X;

            public override float RightWidth()
                => PositionPartWidth() + GapWidth + RadiusPartWidth();

            public string GetPositionString()
                => $"{Circle.CenterX}, {Circle.CenterY}";

            public string GetRadiusString()
                => $"r = {Circle.Radius}";

            public float PositionPartWidth()
                => ActiveFont.Measure(GetPositionString()).X + 120;

            public float RadiusPartWidth()
                => ActiveFont.Measure(GetRadiusString()).X;

            public override void Update()
                => sine += Engine.DeltaTime;

            private void onEnter() {
                Utils.Log("onenter");

                moverEntity = new ScreenCircleMover(this);
                Engine.Scene.Add(moverEntity);
            }

            private void onLeave() {
                Utils.Log("onleave");

                moverEntity.RemoveSelf();
                moverEntity = null;
            }

            public override void ConfirmPressed() {
                moverEntity.StartEditing();
            }

            public override void Render(Vector2 position, bool highlighted) {
                var alpha = Container.Alpha;
                var strokeColor = Color.Black * (alpha * alpha * alpha);
                var color = Disabled
                    ? Color.DarkSlateGray
                    : highlighted
                        ? Container.HighlightColor * alpha
                        : UnselectedColor * alpha;
                
                ActiveFont.DrawOutline(Label, position, new Vector2(0f, 0.5f), Vector2.One, color, 2f, strokeColor);

                var radiusPartWidth = RadiusPartWidth();
                var positionPartWidth = PositionPartWidth();

                var left = Container.Width - radiusPartWidth - GapWidth - positionPartWidth;

                ActiveFont.DrawOutline(
                    "<",
                    position + new Vector2(left + 40f, 0f) + (highlighted ? Calc.AngleToVector(sine * 4f, 4f) : Vector2.Zero),
                    new Vector2(0.5f, 0.5f),
                    Vector2.One,
                    color,
                    2f,
                    strokeColor
                );

                ActiveFont.DrawOutline(
                    GetPositionString(),
                    position + new Vector2(left + positionPartWidth * 0.5f, 0f),
                    new Vector2(0.5f, 0.5f),
                    Vector2.One * 0.8f,
                    color,
                    2f,
                    strokeColor
                );
                
                ActiveFont.DrawOutline(
                    ">",
                    position + new Vector2(left + positionPartWidth - 40f, 0f) + (highlighted ? Calc.AngleToVector(MathF.PI - sine * 4f, 4f) : Vector2.Zero),
                    new Vector2(0.5f, 0.5f),
                    Vector2.One,
                    color,
                    2f,
                    strokeColor
                );

                ActiveFont.DrawOutline(
                    GetRadiusString(),
                    position + new Vector2(left + positionPartWidth + GapWidth + radiusPartWidth * 0.5f, 0f),
                    new Vector2(0.5f, 0.5f),
                    Vector2.One * 0.8f,
                    color,
                    2f,
                    strokeColor
                );
            }
        }
    }
}