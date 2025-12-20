using System.Collections.Generic;
using Celeste.Mod.GooberHelper.Attributes;
using MonoMod.Utils;

namespace Celeste.Mod.GooberHelper.Options.General {
    [GooberHelperOption(Option.ShowActiveOptions)]
    public static class ShowActiveOptions {
        [Tracked(false)]
        public class GooberOptionsList : Entity {
            public static readonly float Padding = 8f;
            public static readonly float Scale = 0.4f;
            public static readonly Color Color = new(1f, 1f, 1f, 0.8f);
            public static readonly float Stroke = 2f;

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

        [OnLoadLevel]
        public static void LoadLevel(Level level, Player.IntroTypes playerIntro, bool isFromLoader) {
            if(level.Tracker.GetEntity<GooberOptionsList>() is null)
                level.Add(new GooberOptionsList());
        }
    }
}