using System;
using Celeste.Mod.GooberHelper.Attributes;
using MonoMod.ModInterop;

namespace Celeste.Mod.GooberHelper.ModImports {
    [ModImportName("ExtendedVariantMode")]
    public static class ExtendedVariantMode {
        [OnLoad]
        public static void Load() {
            if(Loaded)
                return;

            typeof(ExtendedVariantMode).ModInterop();

            Loaded = true;
        }

        public static bool Loaded = false;
        public static Func<int> GetJumpCount;
        public static Action<int> SetJumpCount;
    }
}