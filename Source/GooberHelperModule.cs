global using static Celeste.Mod.GooberHelper.Options.OptionsManager;
global using Microsoft.Xna.Framework;
global using Monocle;

using System;
using Celeste.Mod.GooberHelper.Attributes;
using FMOD.Studio;
using Celeste.Mod.GooberHelper.Settings;

namespace Celeste.Mod.GooberHelper {
    public class GooberHelperModule : EverestModule {
        public static GooberHelperModule Instance { get; private set; }

        public override Type SettingsType => typeof(GooberHelperModuleSettings);
        public static GooberHelperModuleSettings Settings => (GooberHelperModuleSettings)Instance._Settings;

        public override Type SessionType => typeof(GooberHelperModuleSession);
        public static GooberHelperModuleSession Session => (GooberHelperModuleSession)Instance._Session;

        public GooberHelperModule() {
            Instance = this;

#if DEBUG
            // debug builds use verbose logging
            Logger.SetLogLevel("GooberHelper", LogLevel.Verbose);
#else
            // release builds use info logging to reduce spam in log files
            Logger.SetLogLevel("GooberHelper", LogLevel.Info);
#endif
        }

        public override void Load()
            => OnLoadAttribute.InvokeOnTargets();

        public override void LoadContent(bool firstLoad)
            => OnLoadContentAttribute.InvokeOnTargets();

        public override void Unload()
            => OnUnloadAttribute.InvokeOnTargets();

        public override void CreateModMenuSection(TextMenu menu, bool inGame, EventInstance pauseSnapshot) {
            CreateModMenuSectionHeader(menu, inGame, pauseSnapshot);
            
            SettingsManager.CreateModMenuSection(menu, inGame, pauseSnapshot);

            CreateModMenuSectionKeyBindings(menu, inGame, pauseSnapshot);
        }
    }
}