using System;
using Celeste.Mod.Entities;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Entities;
using Celeste.Mod.GooberHelper.Extensions;

namespace Celeste.Mod.GooberHelper.Options.Miscellaneous {
    [GooberHelperOption]
    public class RefillFreezeGameSuspension : AbstractOption {
        public class InputState {
            private bool jump;
            private bool grab;

            public InputState() {
                jump = Input.Jump.Check;
                grab = Input.Grab.Check;
            }

            public bool FarEnoughFrom(InputState other)
                => jump != other.jump || Input.Jump.Pressed
                || grab != other.grab || Input.Grab.Pressed
                || Input.Dash.Pressed
                || Input.CrouchDash.Pressed
                || GooberHelperModule.Settings.ExitGameSuspension.Pressed
                || Input.Pause.Pressed;
        }

        [OnHook]
        private static void patch_Level_Update(On.Celeste.Level.orig_Update orig, Level self) {
            var ext = self.GetExtensionFields();

            if(ext != null && GetOptionBool(Option.RefillFreezeGameSuspension) && ext.FreezeFrameFrozen) {
                var newInputs = new InputState();
                
                if(ext.FreezeFrameFrozenInputs.FarEnoughFrom(newInputs)) {
                    ext.FreezeFrameFrozen = false;

                    Celeste.Freeze(0.01f);
                } else {
                    if(self.Tracker.GetEntity<Player>() is Player player)
                        self.Camera.Position = self.Camera.position + (player.CameraTarget - self.Camera.position) * (1f - (float)Math.Pow(0.01f, Engine.DeltaTime)); //inaccurate yeah yeah i watched the freya holmer thing

                    //CODE DIRECTLY COPIED FROM SPEEDRUNTOOL StateManager.cs
                    self.Wipe?.Update(self);
                    self.HiresSnow?.Update(self);
                    self.Foreground.Update(self);
                    self.Background.Update(self);
                    
                    Engine.Scene.Tracker.GetEntity<CassetteBlockManager>()?.Update();

                    foreach(var entity in Engine.Scene.Tracker.GetEntities<CassetteBlock>())
                        entity.Update();

                    foreach(var listener in Engine.Scene.Tracker.GetComponents<CassetteListener>())
                        listener.Entity.Update();

                    GameSuspensionIgnore.UpdateEntities();

                    return;
                }
            }

            orig(self);
        }
    }
}