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

            [OnLoadLevel]
            public static void LoadLevel(Level level, Player.IntroTypes playerIntro, bool isFromLoader) {
                if(level.Tracker.GetEntity<MouseJoystickDisplay>() is null)
                    level.Add(new MouseJoystickDisplay());
            }

            // public override void Update()
            //     => updateVirtualPosition();

            public override void Render() {
                var mode = GooberHelperModule.Settings.MouseJoystick.Mode;

                if(mode == Mode.MouseJoystickMode.Absolute)
                    AbsoluteMode.Render();
            }
        }

        // public static readonly float Sensitivity = 0.005f;
        // public static readonly float MouseThreshold = 0.75f;

        // public static Vector2 IntegratedMousePosition = Vector2.Zero;
        // public static Vector2 VirtualPosition;
        // public static Vector2 Center;

        // private static float transformMagnitude(float magnitude)
        //     // => 1f - (1f / (1f + 0.01f * magnitude));
        //     => Math.Min(magnitude, 1f);

        // private static void updateVirtualPosition() {
        //     if(!GooberHelperModule.Settings.MouseJoystick.Enabled)
        //         return;

        //     IntegratedMousePosition += MInput.Mouse.Position - new Vector2(Engine.Viewport.Bounds.Width, Engine.Viewport.Bounds.Height) / 2;

        //     // Console.WriteLine(MInput.Mouse.Position);
        //     // Console.WriteLine(MInput.Mouse.Position - new Vector2(Engine.Viewport.Bounds.Width, Engine.Viewport.Bounds.Height) / 2);

        //     Mouse.SetPosition(Engine.Viewport.Bounds.Center.X, Engine.Viewport.Bounds.Center.Y);

        //     var fromCenter = IntegratedMousePosition - Center;

        //     if(fromCenter.Length() > 1f / Sensitivity)
        //         Center += fromCenter.SafeNormalize() * (fromCenter.Length() - 1f / Sensitivity);

        //     var transformedPosition = fromCenter.SafeNormalize() * transformMagnitude(fromCenter.Length() * Sensitivity);

        //     VirtualPosition = transformedPosition;
        // }

        [OnHook]
        private static float patch_Binding_Axis(On.Monocle.Binding.orig_Axis orig, Binding self, int gamepadIndex, float threshold)
            => GooberHelperModule.Settings.MouseJoystick.Mode switch {
                Mode.MouseJoystickMode.Absolute => AbsoluteMode.OverrideAxis(orig, self, gamepadIndex, threshold),
                Mode.MouseJoystickMode.Relative => RelativeMode.OverrideAxis(orig, self, gamepadIndex, threshold),
                _ => orig(self, gamepadIndex, threshold)
            };
    }
}