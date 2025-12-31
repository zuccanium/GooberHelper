using System;
using Celeste.Mod.GooberHelper.Attributes;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.ModInterop;

namespace Celeste.Mod.GooberHelper.ModImports {
    [ModImportName("FrostHelper")]
    public static class FrostHelper {
        [OnLoad]
        public static void Load() {
            if(Loaded)
                return;

            typeof(FrostHelper).ModInterop();

            Loaded = true;
        }

        public static bool Loaded = false;
        public static Func<string, Effect> GetEffectOrNull;
        public static Func<string, Type> EntityNameToType;
    }
}