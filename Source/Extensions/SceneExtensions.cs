using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Options.Miscellaneous;
using MonoMod.Utils;

namespace Celeste.Mod.GooberHelper.Extensions {
    public static class SceneExtensions {
        public class SceneExtensionFields {
            public int Counter = 0;
            public int LastPauseCounterValue = 0;
            public float StunningWatchTimer = 0f;
            public float StunningOffset = 0f;
            public int StunningGroup = 0;

            public bool FreezeFrameFrozen = false;
            public RefillFreezeGameSuspension.InputState FreezeFrameFrozenInputs;
        }

        private static readonly string f_Scene_GooberHelperExtensionFields = nameof(f_Scene_GooberHelperExtensionFields);

        public static SceneExtensionFields GetExtensionFields(this Scene player)
            => DynamicData.For(player).Get<SceneExtensionFields>(f_Scene_GooberHelperExtensionFields);
        
        [OnHook]
        private static void patch_Scene_ctor(On.Monocle.Scene.orig_ctor orig, Scene self) {
            orig(self);

            DynamicData.For(self).Set(f_Scene_GooberHelperExtensionFields, new SceneExtensionFields());
        }
    }
}