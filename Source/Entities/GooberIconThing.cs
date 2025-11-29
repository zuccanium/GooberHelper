using Celeste.Mod.GooberHelper.Attributes;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Entities {
    [Tracked(false)]
    public class GooberIconThing : Entity {
        private MTexture icon = GFX.Gui["GooberHelper/indicator"];

        public GooberIconThing() {
            Tag = Tags.HUD | Tags.Global;
            Depth = -1000;
        }

        [OnLoad]
        public static void Load()
            => Everest.Events.Level.OnLoadLevel += addToScene;

        [OnUnload]
        public static void Unload()
            => Everest.Events.Level.OnLoadLevel -= addToScene;

        private static void addToScene(Level level, Player.IntroTypes playerIntro, bool isFromLoader) {
            if(level.Tracker.GetEntity<GooberIconThing>() is null)
                level.Add(new GooberIconThing());
        }

        public override void Render() {
            var x = 0f;

            if(GetUserEnabledEvilOption())
                icon.Draw(new Vector2(x++ * 32, 1080 - 32), Vector2.Zero, UserDefinedEvilColor);

            if(GetUserEnabledCoolOption())
                icon.Draw(new Vector2(x++ * 32, 1080 - 32), Vector2.Zero, UserDefinedCoolColor);
        }
    }
}