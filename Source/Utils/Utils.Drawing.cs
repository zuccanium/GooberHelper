using System;
using Celeste.Mod.GooberHelper.Attributes;

namespace Celeste.Mod.GooberHelper {
    public partial class Utils {
        public static readonly float RadiusToResolution = 0.5f;
        public static MTexture LargeCircle;
        public static MTexture LargeCircleMask;

        [OnLoadContent]
        public static void LoadContent() {
            LargeCircle = GFX.Gui["GooberHelper/largeCircle"];
            LargeCircleMask = GFX.Gui["GooberHelper/largeCircleMask"];
        }

        public static void DrawFilledCircle(Vector2 position, float radius, Color color, float resolution = 0) {
            if(resolution == 0)
                resolution = radius * RadiusToResolution;

            var thickness = 2 * MathF.Tan(MathF.PI * 2f / resolution) * radius;

            for(var i = 0f; i < resolution; i++)
                Draw.Line(position, position + Calc.AngleToVector(i / resolution * MathF.PI * 2f, radius), color, thickness);
        }

        public static void DrawHollowCircle(Vector2 position, float radius, Color color, float thickness, float resolution = 0) {
            if(resolution == 0)
                resolution = radius * RadiusToResolution;

            Draw.Circle(position, radius, color, (int)thickness, (int)resolution);
        }

        public static void DrawTexturedCircleInner(Vector2 position, float radius, Color color) {
            if(radius == 0 || color.A == 0)
                return;
            
            LargeCircle.DrawCentered(position, color, radius / 256f);
        }

        public static void DrawTexturedCircleOuter(Vector2 position, float radius, Color color) {
            if(radius == 0 || color.A == 0)
                return;

            LargeCircleMask.DrawCentered(position, color, radius / 256f);
            
            //left panel
            Draw.Rect(
                new Rectangle(
                    0,
                    0,
                    (int)(position.X - radius),
                    Engine.Viewport.Height
                ), 
                color
            );

            //top panel
            Draw.Rect(
                new Rectangle(
                    (int)(position.X - radius),
                    0,
                    (int)(radius * 2f),
                    (int)(position.Y - radius)
                ), 
                color
            );

            //bottom panel
            Draw.Rect(
                new Rectangle(
                    (int)(position.X - radius),
                    (int)(position.Y + radius),
                    (int)(radius * 2f),
                    (int)(Engine.Viewport.Height - (position.Y - radius))
                ), 
                color
            );

            //right panel
            Draw.Rect(
                new Rectangle(
                    (int)(position.X + radius),
                    0,
                    (int)(Engine.Viewport.Width - (position.X - radius)),
                    Engine.Viewport.Height
                ), 
                color
            );
        }
    }
}