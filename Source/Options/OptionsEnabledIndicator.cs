using Celeste.Mod.GooberHelper.Attributes;

namespace Celeste.Mod.GooberHelper.Options {
    [Tracked(false)]
    public class OptionsEnabledIndicator : Entity {
        private MTexture icon = GFX.Gui["GooberHelper/indicator"];

        public OptionsEnabledIndicator() {
            Tag = Tags.HUD | Tags.Global;
            Depth = -1000;
        }

        [OnLoad]
        public static void Load()
            => Everest.Events.Level.OnLoadLevel += addToLevel;

        [OnUnload]
        public static void Unload()
            => Everest.Events.Level.OnLoadLevel -= addToLevel;

        private static void addToLevel(Level level, Player.IntroTypes playerIntro, bool isFromLoader) {
            if(level.Tracker.GetEntity<OptionsEnabledIndicator>() is null)
                level.Add(new OptionsEnabledIndicator());
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