using System;
using Monocle;

namespace Celeste.Mod.GooberHelper;

public static partial class Utils {
    public static Action<Action<string>, Action, string> OpenTextInputField;
    public static Action IncreaseCombo;

    public static void CreateTextInputField(TextMenu menu) {
        var textBox = new TextMenuExt.TextBox() { Container = menu, };
        var modal = new TextMenuExt.Modal(textBox, null, 85) { Visible = false };

        Action<string> finishCallback = null;
        Action cancelCallback = null;

        void exitTextBox() {
            textBox.StopTyping();
            modal.Visible = false;
            Input.Pause.ConsumePress();
        }

        textBox.OnTextInputCharActions['\n'] = (_) => {};
        textBox.OnTextInputCharActions['\r'] = (_) => {
            exitTextBox();

            if(textBox.Text.Length > 0) {
                finishCallback?.Invoke(textBox.Text);
            } else {
                cancelCallback?.Invoke();
            }
        };

        textBox.AfterInputConsumed = () => {
            if(textBox.Typing) {
                if(Input.ESC.Pressed) {
                    exitTextBox();

                    Input.ESC.ConsumePress();

                    cancelCallback?.Invoke();
                }
            }
        };

        menu.Add(modal);

        OpenTextInputField = (Action<string> finish, Action cancel, string placeholder) => {
            textBox.PlaceholderText = placeholder;
            textBox.ClearText();
            textBox.StartTyping();
            modal.Visible = true;

            finishCallback = finish;
            cancelCallback = cancel;
        };
    }

    public static void CreateComboModal(TextMenu menu, float expireTime = 1) {
        var label = new TextMenu.Header("") { Container = menu };
        var modal = new TextMenuExt.Modal(label, 85, 500) { Visible = false };

        var timeSinceLastInput = 0f;
        var counter = 0;

        menu.Add(modal);

        modal.OnUpdate = () => {
            if(counter == 0) return;

            timeSinceLastInput += Engine.DeltaTime;

            if(timeSinceLastInput > expireTime) {
                counter = 0;

                modal.Visible = false;
            }
        };

        IncreaseCombo = () => {
            counter++;
            modal.Visible = true;
            label.Title = "x" + counter;
        };
    }
}