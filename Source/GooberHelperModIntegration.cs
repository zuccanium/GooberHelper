using System;
using Celeste.Mod.GooberHelper.Attributes;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.ModInterop;

namespace Celeste.Mod.GooberHelper.ModIntegration {
    [ModImportName("FrostHelper")]
    public static class FrostHelperAPI {
        [OnLoad]
        public static void Load() {
            if(Loaded)
                return;

            typeof(FrostHelperAPI).ModInterop();

            Loaded = true;
        }

        public static bool Loaded = false;
        public static Func<string, Effect> GetEffectOrNull;
        public static Func<string, Type> EntityNameToType;
    }

    [ModImportName("ExtendedVariantMode")]
    public static class ExtendedVariantModeAPI {
        [OnLoad]
        public static void Load() {
            if(Loaded)
                return;

            typeof(ExtendedVariantModeAPI).ModInterop();

            Loaded = true;
        }

        public static bool Loaded = false;
        public static Func<int> GetJumpCount;
        public static Action<int> SetJumpCount;
    }
}