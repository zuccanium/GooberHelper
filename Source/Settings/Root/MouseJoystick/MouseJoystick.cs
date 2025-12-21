using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Helpers;
using Celeste.Mod.Helpers;
using MonoMod.Cil;
using Celeste;
using Microsoft.Xna.Framework.Input;
using System;
using Celeste.Mod.GooberHelper.Settings.Root.MouseJoystick;
using Celeste.Mod.GooberHelper.Settings.Root.MouseJoystick.AbsoluteMode;
using Celeste.Mod.GooberHelper.Settings.Root.MouseJoystick.RelativeMode;

namespace Celeste.Mod.GooberHelper.Settings.Categories.MouseJoystick {
    public static class MouseJoystick {
        [Tracked]
        public class MouseJoystickDisplay : Entity {
            public static readonly int Size = 256;
            public static readonly int Padding = 10;

            public MouseJoystickDisplay() : base() {
                Tag = Tag
                    | Tags.HUD
                    | Tags.Global
                    | Tags.TransitionUpdate
                    | Tags.FrozenUpdate
                    | Tags.PauseUpdate;

                Center = Engine.Viewport.Bounds.Center.ToVector2();
            }

            [SubscribeToEvent(typeof(Everest.Events.Level), "OnLoadLevel")]
            public static void LoadLevel(Level level, Player.IntroTypes playerIntro, bool isFromLoader) {
                if(level.Tracker.GetEntity<MouseJoystickDisplay>() is null)
                    level.Add(new MouseJoystickDisplay());
            }

            public override void Render() {
                var mode = GooberHelperModule.Settings?.MouseJoystick?.Mode;

                if(mode == Mode.ModeValue.Absolute)
                    AbsoluteMode.Instance.Render();

                else if(mode == Mode.ModeValue.Relative)
                    RelativeMode.Instance.Render();
            }
        }

        [OnHook]
        private static void patch_MInput_Update(On.Monocle.MInput.orig_Update orig) {
            orig();

            if(!MInput.Active || Engine.Scene is not Level)
                return;

            MouseHelper.UpdateMouseVisibility();

            var mode = GooberHelperModule.Settings?.MouseJoystick?.Mode;

            switch(mode) {
                case Mode.ModeValue.Absolute:
                    AbsoluteMode.Instance?.Update();

                    break;

                case Mode.ModeValue.Relative:
                    RelativeMode.Instance?.Update();
                    
                    break;
            }
        }

        [OnHook]
        private static float patch_Binding_Axis(On.Monocle.Binding.orig_Axis orig, Binding self, int gamepadIndex, float threshold) {
            var joystickPosition = GooberHelperModule.Settings?.MouseJoystick?.Mode switch {
                Mode.ModeValue.Absolute => AbsoluteMode.Instance.JoystickPosition,
                Mode.ModeValue.Relative => RelativeMode.Instance.JoystickPosition,
                _ => Vector2.Zero,
            };

            if(joystickPosition == Vector2.Zero)
                return orig(self, gamepadIndex, threshold);

            var settings = global::Celeste.Settings.Instance; //what the fuck??

            var result =
                self == settings.Right
                    ? joystickPosition.X > threshold
                        ? Math.Max(joystickPosition.X, 0)
                        : 0

                : self == settings.Left
                    ? joystickPosition.X < -threshold
                        ? Math.Max(-joystickPosition.X, 0)
                        : 0
                
                : self == settings.Down
                    ? joystickPosition.Y > threshold
                        ? Math.Max(joystickPosition.Y, 0)
                        : 0

                : self == settings.Up
                    ? joystickPosition.Y < -threshold
                        ? Math.Max(-joystickPosition.Y, 0)
                        : 0
                
                : 0;
            
            return Math.Max(orig(self, gamepadIndex, threshold), result);
        }
    }
}