using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Celeste.Mod.GooberHelper.Components;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Cil;
using static Celeste.Mod.GooberHelper.OptionsManager;

namespace Celeste.Mod.GooberHelper {
    public static partial class Utils {
        public class InputState {
            private bool jump;
            private bool grab;

            public InputState() {
                jump = Input.Jump.Check;
                grab = Input.Grab.Check;
            }

            public bool FarEnoughFrom(InputState other) {
                return (false)
                    || jump != other.jump || Input.Jump.Pressed
                    || grab != other.grab || Input.Grab.Pressed
                    || Input.Dash.Pressed
                    || Input.CrouchDash.Pressed
                    || GooberHelperModule.Settings.ExitGameSuspension.Pressed
                    || Input.Pause.Pressed;
            }
        }

        public const BindingFlags BindingFlagsAll =
            BindingFlags.NonPublic |
            BindingFlags.Public |
            BindingFlags.Instance |
            BindingFlags.Static;

    }
}