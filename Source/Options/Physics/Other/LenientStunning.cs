using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Extensions;

namespace Celeste.Mod.GooberHelper.Options.Physics.Other {
    [GooberHelperOption(Option.LenientStunning)]
    public static class LenientStunning {
        //code stolen from https://github.com/EverestAPI/CelesteTAS-EverestInterop/blob/c3595e5af47bde0bca28e4693c80c180434c218c/CelesteTAS-EverestInterop/Source/EverestInterop/Hitboxes/CycleHitboxColor.cs
        private static int getOffsetGroup(float offset) {
            var time = Engine.Scene.TimeActive;
            var timeDist = 0;

            while(Math.Floor(((double) time - offset - Engine.DeltaTime) / 0.05f) >= Math.Floor(((double) time - offset) / 0.05f) && timeDist < 3) {
                time += Engine.DeltaTime;
                timeDist++;
            }

            return timeDist < 3 ? (timeDist + Engine.Scene.GetExtensionFields().Counter) % 3 : 3;
        }

        private static float getGroupOffset(int targetGroup) {
            //terrible
            //terrible
            for(float i = 0; i < 1f; i += Engine.DeltaTime * 0.5f) {
                if(getOffsetGroup(i) == targetGroup)
                    return i;
            }

            return -1;
        }

        private static void setStunnableEntityOffset(float offset) {
            using(var enumerator = Engine.Scene.Tracker.GetComponents<PlayerCollider>().GetEnumerator()) {
				while (enumerator.MoveNext()) {
                    if (enumerator.Current.Entity is CrystalStaticSpinner spinner && !spinner.Collidable)
                        spinner.offset = offset;

                    if (enumerator.Current.Entity is Lightning lightning && !lightning.Collidable)
                        lightning.toggleOffset = offset;

                    if (enumerator.Current.Entity is DustStaticSpinner dust && !dust.Collidable)
                        dust.offset = offset;
                }
			}
        }

        [OnHook]
        private static void patch_Level_Pause(On.Celeste.Level.orig_Pause orig, Level self, int startIndex, bool minimal, bool quickReset) {
            orig(self, startIndex, minimal, quickReset);

            if(!GetOptionBool(Option.LenientStunning)) return;

            var ext = self.GetExtensionFields();
            
            if(ext == null) return;

            //dont let the player pause buffer to mimic spinner stunning
            //11 because unpausing time still adds to the counter
            if(ext.Counter <= ext.LastPauseCounterValue + 11) {
                ext.LastPauseCounterValue = ext.Counter;

                return;
            }

            ext.LastPauseCounterValue = ext.Counter;

            var offset = 0f;

            //i dont think it should ever reach 1 but better to be safe than to receive a surprise modding feedback ping
            while (!self.OnInterval(0.05f, offset) && offset < 5f)
                offset += Engine.DeltaTime / 2f;

            ext.StunningWatchTimer = 0.2f;
            ext.StunningOffset = offset;
            ext.StunningGroup = getOffsetGroup(offset);

            setStunnableEntityOffset(offset);
        }

        [OnHook]
        private static void patch_Level_Update(On.Celeste.Level.orig_Update orig, Level self) {
            orig(self);

            var ext = self.GetExtensionFields();

            if(ext != null && GetOptionBool(Option.LenientStunning) && !self.Paused && ext.StunningWatchTimer > 0f) {
                var offsetGroup = getOffsetGroup(ext.StunningOffset);
                var drifted = offsetGroup != ext.StunningGroup;

                if(drifted) {
                    ext.StunningOffset = getGroupOffset(ext.StunningGroup);
                    
                    setStunnableEntityOffset(ext.StunningOffset);
                }

                ext.StunningWatchTimer -= Engine.DeltaTime;
            }
        }

        //code stolen from https://github.com/EverestAPI/CelesteTAS-EverestInterop/blob/c3595e5af47bde0bca28e4693c80c180434c218c/CelesteTAS-EverestInterop/Source/EverestInterop/Hitboxes/CycleHitboxColor.cs
        //very helpful resource for this
        [OnHook]
        private static void modSceneBeforeUpdate(On.Monocle.Scene.orig_BeforeUpdate orig, Scene self) {
            if(self is not Level) {
                orig(self);

                return;
            }

            var timeActive = self.TimeActive;
            var ext = self.GetExtensionFields();

            orig(self);

            if(Math.Abs(timeActive - self.TimeActive) > 0.000001f && ext != null)
                ext.Counter++;
        }
    }
}