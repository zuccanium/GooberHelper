using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers;
using Monocle;

namespace Celeste.Mod.GooberHelper {
    public static class SyncedMusicHelper {
        private static bool resumedMusic = false;
        private static bool resumeMusicNextFrame = false;
        public static bool PlayingSyncedMusic = false;

        public static void Load() {
            On.Celeste.Level.Pause += modLevelPause;
            On.Celeste.Level.Update += modLevelUpdate;
            On.Celeste.Audio.SetMusic += modAudioSetMusic;
        }

        public static void Unload() {
            On.Celeste.Level.Pause -= modLevelPause;
            On.Celeste.Level.Update -= modLevelUpdate;
            On.Celeste.Audio.SetMusic -= modAudioSetMusic;
        }
        
        private static bool modAudioSetMusic(On.Celeste.Audio.orig_SetMusic orig, string path, bool startPlaying, bool allowFadeOut) {
            PlayingSyncedMusic = false;
            
            return orig(path, startPlaying, allowFadeOut);
        }

        private static void modLevelPause(On.Celeste.Level.orig_Pause orig, Level self, int startIndex, bool minimal, bool quickReset) {
            orig(self, startIndex, minimal, quickReset);

            if(!PlayingSyncedMusic) return;

            Audio.CurrentMusicEventInstance.setPaused(true);

            resumedMusic = false;
            resumeMusicNextFrame = false;
        }

        private static void modLevelUpdate(On.Celeste.Level.orig_Update orig, Level self) {
            orig(self);
            
            if(!PlayingSyncedMusic) return;

            if(resumeMusicNextFrame && !resumedMusic) {
                Audio.CurrentMusicEventInstance.setPaused(false);

                Console.WriteLine("resetting");

                resumedMusic = true;
            }

            if(self.unpauseTimer < 0 && !resumedMusic && !self.Paused) 
                resumeMusicNextFrame = true;
        }

        public static int GetTimelinePosition() {
            Audio.CurrentMusicEventInstance.getTimelinePosition(out var timelinePosition);
            
            return timelinePosition;
        }

        public static IEnumerator PlaySyncedMusic(string path) {            
            Audio.SetMusic(path);

            while(true) {
                Audio.CurrentMusicEventInstance.getPlaybackState(out var playbackState);
                if(playbackState != FMOD.Studio.PLAYBACK_STATE.STARTING) break;

                Logger.Debug("GooberHelper", "waiting for playback state...");

                yield return null;
            }

            while(true) {
                Audio.CurrentMusicEventInstance.getTimelinePosition(out var timelinePosition);
                if(timelinePosition > 0) break;

                Logger.Debug("GooberHelper", "waiting for timeline position...");

                yield return null;
            }

            Logger.Debug("GooberHelper", "done waiting for audio!");

            PlayingSyncedMusic = true;

            yield break;
        }
    }
}

/*
- be able to pause and unpause music when pausing and unpausing the game without delay
    - use setvolume
*/

/*
var stopwatch = new Stopwatch();
var targetPosition = Random.Shared.Range(0, 100000);
var counter = 0;

Audio.CurrentMusicEventInstance.setTimelinePosition(targetPosition);

var timer = new System.Timers.Timer(100);
timer.AutoReset = true;
timer.Elapsed += (source, args) => {
    Audio.CurrentMusicEventInstance.getTimelinePosition(out var timelinePosition);

    Console.WriteLine($"target: {targetPosition}, actual: {timelinePosition}");

    if(timelinePosition == targetPosition) {
        stopwatch.Stop();
        timer.Stop();

        timespans.Add(stopwatch.Elapsed);

        Console.WriteLine($"it took {stopwatch.Elapsed}ms");
    }

    if(counter > 10) {
        timer.Stop();
        stopwatch.Stop();

        Console.WriteLine("bail");
    }

    counter++;
};

timer.Start();
stopwatch.Start();
*/

/*
var stopwatch = new Stopwatch();
var counter = 0;

Audio.CurrentMusicEventInstance.setPaused(true);

var timer = new System.Timers.Timer(100);
timer.AutoReset = true;
timer.Elapsed += (source, args) => {
    Audio.CurrentMusicEventInstance.getPaused(out var paused);

    if(paused == true) {
        stopwatch.Stop();
        timer.Stop();

        Console.WriteLine($"it took {stopwatch.Elapsed}");
    }

    if(counter > 10) {
        timer.Stop();
        stopwatch.Stop();

        Console.WriteLine("bail");
    }

    counter++;
};

timer.Start();
stopwatch.Start();
*/