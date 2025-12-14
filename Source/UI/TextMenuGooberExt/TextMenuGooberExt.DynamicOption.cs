using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

//copied and modified from the decomp of Option<T>
namespace Celeste.Mod.GooberHelper.UI {
    public static partial class TextMenuGooberExt {
        public class DynamicOption<T> : TextMenu.Item where T : struct, System.Numerics.INumber<T> {
            public string Label;
            public int Index = -1;
            public Action<T>? OnValueChange;
            public int PreviousIndex = -1;
            public List<Tuple<string, T>> Values = [];
            private float sine;
            private int lastDir;

            public T? LeftMin;
            public T? LeftMax;
            public T? RightMin;
            public T? RightMax;
            public T Step;
            public string Suffix = "";

            public bool SkipLeftMax;
            public bool SkipRightMin;
            
            public T? DynamicValue;
            public int MovementSpeed = 0;

            private float cachedRightWidth = 0f;

            public Color UnselectedColor = Color.White;

            public T Current {
                get => Index == Values.Count || Index == -1
                    ? DynamicValue ?? T.Zero
                    : Values[Index].Item2;
                set {
                    Utils.Log($"setting {Label} to {value}");

                    var i = 0;
                    
                    foreach(var item in Values) {
                        if(item.Item2 == value) {
                            Utils.Log($"found a match! {item} matches {value}");

                            Index = i;
                            DynamicValue = null;

                            return;
                        }

                        i++;
                    }

                    Utils.Log($"setting the dynamic value to {value}");

                    DynamicValue = value;
                    Index = 
                        RightMin is T rightMin && DynamicValue >= rightMin
                            ? Values.Count

                        : LeftMax is T leftMax && DynamicValue <= leftMax
                            ? -1

                        : 0; //what
                }
            }

            public string CurrentString
                => Index == Values.Count || Index == -1
                    ? (DynamicValue ?? T.Zero).ToString() + Suffix
                    : Values[Index].Item1;

            public DynamicOption(string label, string? suffix, T step, T? leftMin = null, T? leftMax = null, T? rightMin = null, T? rightMax = null) {
                Label = label;
                Selectable = true;

                Step = step;
                Suffix = suffix ?? "";

                LeftMin = leftMin;
                LeftMax = leftMax;
                RightMin = rightMin;
                RightMax = rightMax;
            }

            public DynamicOption()
                : this("", null, T.One) {}

            public DynamicOption<T> Add(string label, T value, bool selected = false) {
                Values.Add(new Tuple<string, T>(label, value));

                if(selected)
                    PreviousIndex = Index = Values.Count - 1;

                return this;
            }

            public DynamicOption<T> AddEnumerable(IEnumerable<KeyValuePair<T, string>> enumerable, T? select = null) {
                var i = Values.Count;

                foreach(var item in enumerable) {
                    Values.Add(new Tuple<string, T>(item.Value, item.Key));

                    if(item.Key == select)
                        Index = i;
                    
                    i++;
                }
                
                return this;
            }

            public DynamicOption<T> AddEnumerable(IEnumerable<T> enumerable, T? select = null) {
                var i = Values.Count;

                foreach(var item in enumerable) {
                    Values.Add(new Tuple<string, T>(item.ToString() ?? "", item));

                    if(item == select)
                        Index = i;
                    
                    i++;
                }
                
                return this;
            }

            public DynamicOption<T> Change(Action<T> action) {
                OnValueChange = action;
                return this;
            }

            public void RecalculateCachedRightWidth() {
                cachedRightWidth = 0;

                foreach(var item in Values) {
                    var itemWidth = ActiveFont.Measure(item.Item1).X;

                    cachedRightWidth = Math.Max(cachedRightWidth, itemWidth);
                }

                cachedRightWidth += 120f;
            }

            public override void Added() {
                Container.InnerContent = TextMenu.InnerContentMode.TwoColumn;
            }

            public override void LeftPressed()
                => MoveInDirection(-1);

            public override void RightPressed()
                => MoveInDirection(1);

            private void updateValue(int dir) {
                Audio.Play(
                    dir == 1
                        ? SFX.ui_main_button_toggle_on
                        : SFX.ui_main_button_toggle_off
                );
                
                lastDir = dir;
                ValueWiggler.Start();

                OnValueChange?.Invoke(Current);
            }

            private T getRealStep(int dir)
                => Step * T.CreateChecked(dir * MathF.Pow(2, MovementSpeed));

            private bool maybeInitializeDynamicValue(int dir) {
                if(DynamicValue is not null)
                    return false;

                if(dir == 1 && Index == Values.Count - 1 && RightMin is T)
                    DynamicValue = RightMin + (SkipRightMin ? getRealStep(dir) : T.Zero);
                
                else if(dir == -1 && Index == 0 && LeftMax is T)
                    DynamicValue = LeftMax + (SkipLeftMax ? getRealStep(dir) : T.Zero);

                return DynamicValue is not null;
            }

            public void MoveInDirection(int dir) {
                //initialization
                if(maybeInitializeDynamicValue(dir)) {
                    Utils.Log($"initialized to {DynamicValue}!");

                    PreviousIndex = Index;
                    Index += dir;

                    updateValue(dir);

                    return;
                }

                //regular update
                if(DynamicValue is null) {
                    if(Index + dir == Values.Count || Index + dir == -1)
                        return;
                    
                    Utils.Log("doing the regular update");

                    PreviousIndex = Index;
                    Index += dir;

                    updateValue(dir);

                    return;
                }

                //dynamic value update
                var oldDynamicValue = DynamicValue;

                DynamicValue += getRealStep(dir);
                
                Utils.Log($"updating the dynamic value {oldDynamicValue} -> {DynamicValue}");

                //high bounds enforcement
                if(RightMax is T && DynamicValue > RightMax)
                    DynamicValue = RightMax;

                if(LeftMin is T && DynamicValue < LeftMin)
                    DynamicValue = LeftMin;

                //low bounds enforcement
                var crossedRightBoundary = 
                    RightMin is T
                    && oldDynamicValue >= RightMin
                    && (DynamicValue < RightMin || SkipRightMin && DynamicValue == RightMin)
                    && Index == Values.Count;
                
                var crossedLeftBoundary =
                    LeftMax is T
                    && oldDynamicValue <= LeftMax
                    && (DynamicValue > LeftMax || SkipLeftMax && DynamicValue == LeftMax)
                    && Index == -1;

                if(crossedRightBoundary || crossedLeftBoundary) {
                    Utils.Log($"crossed over the {Utils.JoinList(new List<string>() {crossedLeftBoundary ? "left" : "", crossedRightBoundary ? "right" : ""}.Where(str => str is not ""), "and")} boundary!");

                    if(Values.Count > 0) {
                        DynamicValue = null;

                        PreviousIndex = Index;
                        Index += dir;
                    }

                    else if(RightMin == LeftMax)
                        {}

                    else if(crossedRightBoundary)
                        DynamicValue = RightMin;

                    else if(crossedLeftBoundary)
                        DynamicValue = LeftMax;
                }

                //dont update if nothing changed
                if(DynamicValue == oldDynamicValue)
                    return;

                updateValue(dir);
            }

            public override void Update() {
                sine += Engine.RawDeltaTime;

                if(Container.Current != this)
                    return;

                if(Input.Jump.Pressed) {
                    Input.Jump.ConsumeBuffer();
                    
                    MovementSpeed++;
                }
                
                if(Input.Grab.Pressed) {
                    Input.Grab.ConsumeBuffer();
                    
                    MovementSpeed--;
                }
            }

            public override float LeftWidth()
                => ActiveFont.Measure(Label).X + 32f;

            public override float RightWidth()
                => Utils.SignedAbsMax(240f, ActiveFont.Measure(CurrentString).X + 120f, cachedRightWidth);

            public override float Height()
                => ActiveFont.LineHeight;

            public override void Render(Vector2 position, bool highlighted) {
                var alpha = Container.Alpha;
                var strokeColor = Color.Black * (alpha * alpha * alpha);
                var color = Disabled
                    ? Color.DarkSlateGray
                    : highlighted
                        ? Container.HighlightColor * alpha
                        : UnselectedColor * alpha;

                ActiveFont.DrawOutline(Label, position, new Vector2(0f, 0.5f), Vector2.One, color, 2f, strokeColor);
                    
                var rightWidth = RightWidth();
                var sineOffset = Vector2.UnitX * (highlighted ? ((float)Math.Sin(sine * 4f) * 4f) : 0f);

                ActiveFont.DrawOutline(
                    CurrentString,
                    position + new Vector2(Container.Width - rightWidth * 0.5f + lastDir * ValueWiggler.Value * 8f, 0f),
                    new Vector2(0.5f, 0.5f),
                    Vector2.One * 0.8f,
                    color,
                    2f,
                    strokeColor
                );

                var atLeftBorder = Index <= 0;
                var atRightBorder = Index >= Values.Count - 1;
                
                //subtract 1 if its negative to not skip directly from > to <<
                for(var i = Math.Abs(MovementSpeed) - (MovementSpeed < 0 ? 1 : 0); i >= 0; i--) {
                    var movementSpeedOffset = (MathF.Log2(i + 1f) - 1f) * 10f;
                    var flipped = MovementSpeed < 0;

                    ActiveFont.DrawOutline(
                        flipped ? ">" : "<",
                        position + new Vector2(Container.Width - movementSpeedOffset - rightWidth + 40f + (lastDir < 0 ? -ValueWiggler.Value * 8f : 0f), 0f) - (atLeftBorder ? Vector2.Zero : sineOffset),
                        new Vector2(0.5f, 0.5f),
                        Vector2.One,
                        atLeftBorder
                            ? Color.DarkSlateGray * alpha
                            : color,
                        2f,
                        strokeColor
                    );
                    
                    ActiveFont.DrawOutline(
                        flipped ? "<" : ">",
                        position + new Vector2(Container.Width + movementSpeedOffset - 40f + (lastDir > 0 ? ValueWiggler.Value * 8f : 0f), 0f) + (atRightBorder ? Vector2.Zero : sineOffset),
                        new Vector2(0.5f, 0.5f),
                        Vector2.One,
                        atRightBorder
                            ? Color.DarkSlateGray * alpha
                            : color,
                        2f,
                        strokeColor
                    );
                }
            }

            public override string SearchLabel()
                => Label;
        }
    }
}