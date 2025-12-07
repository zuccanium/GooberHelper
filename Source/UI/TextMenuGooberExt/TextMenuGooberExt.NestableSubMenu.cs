using System;
using System.Collections.Generic;
using Celeste.Mod.Core;

//CODE COPIED STRAIGHT FROM THE EVEREST GITHUB
//SORRY EVEREST TEAM I KNOW THIS IS STUPID BUT YOU DONT HAVE NESTABLE SUBMENU SUPPORT AND I REALLY NEED THAT
//I MIGHT MAKE A PR WITH THIS EVENTUALLY IDK
//I TRIED TO NOT CHANGE MUCH EVEN THOUGH THIS FORMATTING MAKES MY EDITORCONFIG REALLY UPSET
namespace Celeste.Mod.GooberHelper.UI {
    public static partial class TextMenuGooberExt {
        public class NestableSubMenu : TextMenu.Item {
            public string Label;
            MTexture Icon;

            /// <inheritdoc cref="patch_TextMenu.Items"/>
            public List<TextMenu.Item> Items { get; private set; }

            private List<TextMenu.Item> delayedAddItems;

            /// <inheritdoc cref="TextMenu.Selection"/>
            public int Selection;

            /// <inheritdoc cref="TextMenu.Current"/>
            public TextMenu.Item Current {
                get {
                    if (Items.Count <= 0 || Selection < 0 || Selection >= Items.Count) {
                        return null;
                    }
                    return Items[Selection];
                }
                set {
                    Selection = Items.IndexOf(value);
                }
            }

            /// <inheritdoc cref="TextMenu.FirstPossibleSelection"/>
            public int FirstPossibleSelection {
                get {
                    for (int i = 0; i < Items.Count; i++) {
                        if (Items[i] != null && Items[i].Hoverable) {
                            return i;
                        }
                    }
                    return 0;
                }
            }

            /// <inheritdoc cref="TextMenu.LastPossibleSelection"/>
            public int LastPossibleSelection {
                get {
                    for (int i = Items.Count - 1; i >= 0; i--) {
                        if (Items[i] != null && Items[i].Hoverable) {
                            return i;
                        }
                    }
                    return 0;
                }
            }

            /// <inheritdoc cref="TextMenu.ScrollTargetY"/>
            public float ScrollTargetY {
                get {
                    float min = Engine.Height - 150f - Container.Height * Container.Justify.Y;
                    float max = 150f + Container.Height * Container.Justify.Y;

                    float y_offset = Current is not null ? GetYOffsetOf(Current) : GetYOffsetOf(Items[Calc.Clamp(Selection, 0, Items.Count-1)]);
                    return Calc.Clamp((Engine.Height / 2) + Container.Height * Container.Justify.Y - y_offset, min, max);
                }
            }

            /// <inheritdoc cref="TextMenu.ItemSpacing"/>
            public float ItemSpacing;
            public float ItemIndent;
            /// <inheritdoc cref="TextMenu.HighlightColor"/>
            private Color HighlightColor;
            public string ConfirmSfx;

            public bool AlwaysCenter;

            public float LeftColumnWidth;
            public float RightColumnWidth;

            public float TitleHeight { get; private set; }
            public float MenuHeight { get; private set; }

            public bool Focused;
            public bool InNestedSubMenu;

            private bool enterOnSelect;
            private bool entering;
            private float ease;

            private bool containerAutoScroll;

            public NestableSubMenu ParentSubMenu;

            /// <summary>
            /// Create a new SubMenu.
            /// </summary>
            /// <param name="label"></param>
            /// <param name="enterOnSelect">Expand submenu when selected</param>
            public NestableSubMenu(string label, bool enterOnSelect) : base() {
                // Item Constructor
                ConfirmSfx = SFX.ui_main_button_select;
                Label = label;
                Icon = GFX.Gui["downarrow"];
                Selectable = true;
                IncludeWidthInMeasurement = true;

                this.enterOnSelect = enterOnSelect;

                OnEnter = delegate {
                    if (this.enterOnSelect) {
                        ConfirmPressed();
                    }
                };


                // Menu Constructor
                Items = new List<TextMenu.Item>();
                delayedAddItems = new List<TextMenu.Item>();
                Selection = -1;
                ItemSpacing = 4f;
                ItemIndent = 20f;
                HighlightColor = Color.White;

                RecalculateSize();
            }

            #region Menu

            /// <summary>
            /// Add any non-submenu <see cref="TextMenu.Item"/> to the Submenu
            /// </summary>
            /// <param name="item">Item to be added</param>
            /// <returns></returns>
            public NestableSubMenu Add(TextMenu.Item item) {
                if (Container != null) {
                    Items.Add(item);
                    item.Container = Container;
                    Container.Add(item.ValueWiggler = Wiggler.Create(0.25f, 3f, null, false, false));
                    Container.Add(item.SelectWiggler = Wiggler.Create(0.25f, 3f, null, false, false));
                    item.ValueWiggler.UseRawDeltaTime = (item.SelectWiggler.UseRawDeltaTime = true);

                    if(item is NestableSubMenu nestableSubMenuItem) {
                        nestableSubMenuItem.ParentSubMenu = this;
                    }

                    RecalculateSize();
                    item.Added();
                    return this;
                } else {
                    delayedAddItems.Add(item);
                    return this;
                }
            }

            /// <summary>
            /// Insert any non-submenu <see cref="TextMenu.Item"/> into the Submenu at <paramref name="index"/>
            /// </summary>
            /// <param name="index"></param>
            /// <param name="item">Item to be inserted</param>
            /// <returns></returns>v
            public NestableSubMenu Insert(int index, TextMenu.Item item) {
                if (Container != null) {
                    Items.Insert(index, item);
                    item.Container = Container;
                    Container.Add(item.ValueWiggler = Wiggler.Create(0.25f, 3f, null, false, false));
                    Container.Add(item.SelectWiggler = Wiggler.Create(0.25f, 3f, null, false, false));
                    item.ValueWiggler.UseRawDeltaTime = (item.SelectWiggler.UseRawDeltaTime = true);
                    RecalculateSize();
                    item.Added();
                    return this;
                } else {
                    delayedAddItems.Insert(index, item);
                    return this;
                }
            }

            public bool ContainsDelayedAddItem(TextMenu.Item item) {
                return Container == null && delayedAddItems.Contains(item);
            }

            public NestableSubMenu InsertDelayedAddItem(TextMenu.Item item, TextMenu.Item after) {
                if (Container == null && delayedAddItems.Contains(after))
                    delayedAddItems.Insert(delayedAddItems.IndexOf(after) + 1, item);
                return this;
            }

            /// <summary>
            /// Remove any non-submenu <see cref="TextMenu.Item"/> from the Submenu
            /// </summary>
            /// <param name="item">Item to be removed</param>
            /// <returns></returns>v
            public NestableSubMenu Remove(TextMenu.Item item) {
                if (Container != null) {
                    if (!Items.Remove(item)) {
                        return this;
                    }
                    item.Container = null;
                    Container.Remove(item.ValueWiggler);
                    Container.Remove(item.SelectWiggler);
                    RecalculateSize();
                    return this;
                } else {
                    delayedAddItems.Remove(item);
                    return this;
                }
            }

            /// <inheritdoc cref="TextMenu.Clear"/>
            public void Clear() {
                Items = new List<TextMenu.Item>();
            }

            /// <inheritdoc cref="TextMenu.IndexOf(TextMenu.Item)"/>
            public int IndexOf(TextMenu.Item item) {
                return Items.IndexOf(item);
            }

            /// <inheritdoc cref="TextMenu.FirstSelection"/>
            public void FirstSelection() {
                Selection = -1;
                MoveSelection(1, false);
            }

            /// <summary>
            /// Set the selection to the last possible <see cref="TextMenu.Item"/>.
            /// </summary>
            public void LastSelection() {
                Selection = Items.Count;
                MoveSelection(-1, false);
            }

            /// <inheritdoc cref="TextMenu.MoveSelection(int, bool)"/>
            public void MoveSelection(int direction, bool wiggle = false) {
                int selection = Selection;
                direction = Math.Sign(direction);
                int count = 0;
                foreach (TextMenu.Item item in Items) {
                    if (item.Hoverable)
                        count++;
                }
                do {
                    Selection += direction;
                    if (enterOnSelect && !entering) {
                        if (Selection < 0 || Selection >= Items.Count) {
                            // Avoid crash when getting Current item
                            Selection = selection;
                            Exit();
                            Container.MoveSelection(direction, true);
                            return;
                        }
                    }
                    if (count > 2) {
                        if (Selection < 0) {
                            Selection = Items.Count - 1;
                        } else if (Selection >= Items.Count) {
                            Selection = 0;
                        }
                    } else if (Selection < 0 || Selection > Items.Count - 1) {
                        Selection = Calc.Clamp(Selection, 0, Items.Count - 1);
                        break;
                    }
                }
                while (!Current.Hoverable);

                if (!Current.Hoverable) {
                    Selection = selection;
                }
                if (Selection != selection && Current != null) {
                    if (selection >= 0 && selection < Items.Count && Items[selection] != null && Items[selection].OnLeave != null) {
                        Items[selection].OnLeave();
                    }
                    Current.OnEnter?.Invoke();
                    if (wiggle) {
                        Audio.Play(direction > 0 ? SFX.ui_main_roll_down : SFX.ui_main_roll_up);
                        Current.SelectWiggler.Start();
                    }
                }
            }

            /// <inheritdoc cref="TextMenu.RecalculateSize"/>
            public void RecalculateSize() {
                TitleHeight = ActiveFont.LineHeight;
                if (Items.Count < 1)
                    return;

                LeftColumnWidth = RightColumnWidth = MenuHeight = 0f;
                foreach (TextMenu.Item item in Items) {
                    if (item.IncludeWidthInMeasurement) {
                        LeftColumnWidth = Math.Max(LeftColumnWidth, item.LeftWidth());
                    }
                }
                foreach (TextMenu.Item item in Items) {
                    if (item.IncludeWidthInMeasurement) {
                        RightColumnWidth = Math.Max(RightColumnWidth, item.RightWidth());
                    }
                }
                foreach (TextMenu.Item item in Items) {
                    if (item.Visible) {
                        MenuHeight += item.Height() + Container.ItemSpacing;
                    }
                }
                MenuHeight -= Container.ItemSpacing;
            }

            /// <inheritdoc cref="TextMenu.GetYOffsetOf(TextMenu.Item)"/>
            public float GetYOffsetOf(TextMenu.Item item) {
                float offset = (ParentSubMenu is not null ? ParentSubMenu.GetYOffsetOf(this) : Container.GetYOffsetOf(this)) - Height() * 0.5f;
                if (item == null) {
                    // common case is all items in submenu are disabled when item is null
                    return offset + TitleHeight * 0.5f;
                }
                offset += TitleHeight;
                foreach (TextMenu.Item child in Items) {
                    if (child.Visible) {
                        offset += child.Height() + ItemSpacing;
                    }
                    if (child == item) {
                        break;
                    }
                }
                return offset - item.Height() * 0.5f - ItemSpacing;
            }

            public void Exit() {
                Current?.OnLeave?.Invoke();
                Focused = false;
                if (!Input.MenuUp.Repeating && !Input.MenuDown.Repeating)
                    Audio.Play(SFX.ui_main_button_back);
                
                if(ParentSubMenu is not null) {
                    ParentSubMenu.InNestedSubMenu = false;
                    ParentSubMenu.containerAutoScroll = containerAutoScroll;
                } else {
                    Container.AutoScroll = containerAutoScroll;
                    Container.Focused = true;
                }
            }

            public override string SearchLabel() {
                return Label;
            }

            #endregion

            #region TextMenu.Item

            public override void ConfirmPressed() {
                if (Items.Count > 0) {
                    Focused = true;

                    if(ParentSubMenu is not null) {
                        ParentSubMenu.InNestedSubMenu = true;
                        containerAutoScroll = ParentSubMenu.containerAutoScroll;
                        ParentSubMenu.containerAutoScroll = false;
                    } else {
                        containerAutoScroll = Container.AutoScroll;
                        Container.Focused = false;
                        Container.AutoScroll = false;
                    }

                    entering = true;
                    if (Input.MenuUp.Pressed)
                        LastSelection();
                    else
                        FirstSelection();
                    entering = false;

                    if (!Input.MenuUp.Repeating && !Input.MenuDown.Repeating)
                        Audio.Play(ConfirmSfx);

                    RecalculateSize();

                    base.ConfirmPressed();
                }
            }

            public override float LeftWidth() {
                return ActiveFont.Measure(Label).X;
            }

            public override float RightWidth() {
                return Icon.Width;
            }

            public override float Height() {
                // If there are no items, MenuHeight will actually be a negative number
                if (Items.Count > 0)
                    return TitleHeight + (MenuHeight * Ease.QuadOut(ease));
                else
                    return TitleHeight;
            }

            public override void Added() {
                base.Added();
                foreach (TextMenu.Item item in delayedAddItems) {
                    Add(item);
                }
            }

            public override void Update() {
                if (Focused)
                    ease = Calc.Approach(ease, 1f, Engine.RawDeltaTime * 4f);
                else
                    ease = Calc.Approach(ease, 0f, Engine.RawDeltaTime * 4f);
                base.Update();

                // ease check needed to eat the first input from Container
                if (Focused && !InNestedSubMenu && ease > 0.9f) {
                    if (Input.MenuDown.Pressed && (!Input.MenuDown.Repeating || Selection != LastPossibleSelection || enterOnSelect)) {
                        MoveSelection(1, true);
                    } else if (Input.MenuUp.Pressed && (!Input.MenuUp.Repeating || Selection != FirstPossibleSelection || enterOnSelect)) {
                        MoveSelection(-1, true);
                    }
                    if (Current != null) {
                        if (Input.MenuLeft.Pressed) {
                            Current.LeftPressed();
                        }
                        if (Input.MenuRight.Pressed) {
                            Current.RightPressed();
                        }
                        if (Input.MenuConfirm.Pressed) {
                            Current.ConfirmPressed();
                            Current.OnPressed?.Invoke();
                        }
                        if (Input.MenuJournal.Pressed && Current.OnAltPressed != null) {
                            Current.OnAltPressed();
                        }
                    }
                    if (!Input.MenuConfirm.Pressed) {
                        if (Input.MenuCancel.Pressed || Input.ESC.Pressed || Input.Pause.Pressed) {
                            Exit();
                        }
                    }
                }

                foreach (TextMenu.Item item in Items) {
                    item.OnUpdate?.Invoke();
                    item.Update();
                }

                if (!CoreModule.Settings.AllowTextHighlight) {
                    HighlightColor = TextMenu.HighlightColorA;
                } else if (Engine.Scene.OnRawInterval(0.1f)) {
                    if (HighlightColor == TextMenu.HighlightColorA) {
                        HighlightColor = TextMenu.HighlightColorB;
                    } else {
                        HighlightColor = TextMenu.HighlightColorA;
                    }
                }

                if (Focused && containerAutoScroll) {
                    if (Container.Height > Container.ScrollableMinSize) {
                        Container.Position.Y += (ScrollTargetY - Container.Position.Y) * (1f - (float) Math.Pow(0.01f, Engine.RawDeltaTime));
                        return;
                    }
                    Container.Position.Y = 540f;
                }
            }

            public override void Render(Vector2 position, bool highlighted) {
                Vector2 top = new Vector2(position.X, position.Y - (Height() / 2));

                float alpha = Container.Alpha;
                Color color = Disabled ? Color.DarkSlateGray : ((highlighted && !InNestedSubMenu ? Container.HighlightColor : Color.White) * alpha);
                Color strokeColor = Color.Black * (alpha * alpha * alpha);

                bool uncentered = Container.InnerContent == TextMenu.InnerContentMode.TwoColumn && !AlwaysCenter;

                Vector2 titlePosition = top + (Vector2.UnitY * TitleHeight / 2) + (uncentered ? Vector2.Zero : new Vector2(Container.Width * 0.5f, 0f));
                Vector2 justify = uncentered ? new Vector2(0f, 0.5f) : new Vector2(0.5f, 0.5f);
                Vector2 iconJustify = uncentered ? new Vector2(ActiveFont.Measure(Label).X + Icon.Width, 5f) : new Vector2(ActiveFont.Measure(Label).X / 2 + Icon.Width, 5f);
                NestableSubMenu.DrawIcon(titlePosition, Icon, iconJustify, true, (Disabled || Items.Count < 1 ? Color.DarkSlateGray : (Focused ? Container.HighlightColor : Color.White)) * alpha, 0.8f);
                ActiveFont.DrawOutline(Label, titlePosition, justify, Vector2.One, color, 2f, strokeColor);

                if (Focused && ease > 0.9f) {
                    Vector2 menuPosition = new Vector2(top.X + ItemIndent, top.Y + TitleHeight + ItemSpacing);
                    RecalculateSize();
                    foreach (TextMenu.Item item in Items) {
                        if (item.Visible) {
                            float height = item.Height();
                            Vector2 itemPosition = menuPosition + new Vector2(0f, height * 0.5f + item.SelectWiggler.Value * 8f);
                            if (itemPosition.Y + height * 0.5f > 0f && itemPosition.Y - height * 0.5f < Engine.Height) {
                                item.Render(itemPosition, Focused && !InNestedSubMenu && Current == item);
                            }
                            menuPosition.Y += height + ItemSpacing;
                        }
                    }
                }
            }

            private static void DrawIcon(Vector2 position, MTexture icon, Vector2 justify, bool outline, Color color, float scale) {
                if (outline) {
                    icon.DrawOutlineCentered(position + justify, color);
                } else {
                    icon.DrawCentered(position + justify, color, scale);
                }
            }
        }

        #endregion TextMenu.Item

        /// <summary>
        /// Add an Enter and Leave handler, displaying a description if selected.
        /// </summary>
        /// <param name="option">The input TextMenu.Item option.</param>
        /// <param name="containingSubMenu">The submenu containing the TextMenu.Item option.</param>
        /// <param name="parentContainer">The menu that the submenu is or will be part of.</param>
        /// <param name="description"></param>
        /// <returns>The passed option.</returns>
        public static TextMenu.Item AddDescription(this TextMenu.Item option, NestableSubMenu containingSubMenu, TextMenu parentContainer, string description) {
            // build the description menu entry
            TextMenuExt.EaseInSubHeaderExt descriptionText = new TextMenuExt.EaseInSubHeaderExt(description, false, parentContainer) {
                TextColor = Color.Gray,
                HeightExtra = 0f
            };

            if (containingSubMenu.Items.Contains(option)) {
                // insert the description into item list after the option.
                containingSubMenu.Insert(containingSubMenu.Items.IndexOf(option) + 1, descriptionText);
            } else if (containingSubMenu.ContainsDelayedAddItem(option)) {
                // insert the description into "delayed add" item list, when necessary
                containingSubMenu.InsertDelayedAddItem(descriptionText, option);
            }

            option.OnEnter += delegate {
                // make the description appear.
                descriptionText.FadeVisible = true;
            };
            option.OnLeave += delegate {
                // make the description disappear.
                descriptionText.FadeVisible = false;
            };

            return option;
        }
    }
}