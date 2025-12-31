using System;

namespace Celeste.Mod.GooberHelper {
    public static partial class Utils {
        public static Action<Action<string>, Action, string> OpenTextInputField;

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
    }
}