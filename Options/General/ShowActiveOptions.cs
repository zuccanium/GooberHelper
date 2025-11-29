using System.Collections.Generic;
using Celeste.Mod.GooberHelper.Attributes;
using MonoMod.Utils;

namespace Celeste.Mod.GooberHelper.Options.General {
    [GooberHelperOption(Option.ShowActiveOptions)]
    public static class ShowActiveOptions {
        [Tracked(false)]
        public class GooberOptionsList : Entity {
            public readonly float Padding = 8f;
            public readonly float Scale = 0.4f;
            public readonly Color Color = new(1f, 1f, 1f, 0.8f);
            public readonly float Stroke = 2f;

            private static Dictionary<string, FastReflectionHelper.FastInvoker> optionGetters;

            public GooberOptionsList() {
                Tag = Tags.HUD | Tags.Global;
                Depth = 10000;
            }

            public override void Render() {
                if(!GetOptionBool(Option.ShowActiveOptions))
                    return;

                ActiveFont.DrawOutline(
                    text: GetEnabledOptionsString(),
                    position: new Vector2(1920 - Padding, Padding),
                    justify: new Vector2(1f, 0),
                    scale: new Vector2(Scale),
                    color: Color,
                    stroke: Stroke,
                    strokeColor: Color.Black
                );
            }
        }

        [OnLoad]
        public static void Load()
            => Everest.Events.Level.OnLoadLevel += addToScene;

        [OnUnload]
        public static void Unload()
            => Everest.Events.Level.OnLoadLevel -= addToScene;

        private static void addToScene(Level level, Player.IntroTypes playerIntro, bool isFromLoader) {
            if(level.Tracker.GetEntity<GooberOptionsList>() is null)
                level.Add(new GooberOptionsList());
        }
    }
}