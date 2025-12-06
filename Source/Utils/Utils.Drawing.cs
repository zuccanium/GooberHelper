using System;

namespace Celeste.Mod.GooberHelper {
    public partial class Utils {
        public static readonly float RadiusToResolution = 0.5f;

        public static void DrawFilledCircle(Vector2 position, float radius, Color color, int resolution) {
            var thickness = 2 * MathF.Tan(MathF.PI * 2f / resolution) * radius;

            for(var i = 0f; i < resolution; i++)
                Draw.Line(position, position + Calc.AngleToVector(i / resolution * MathF.PI * 2f, radius), color, thickness);
        }

        public static void DrawHollowCircle(Vector2 position, float radius, Color color, float thickness) {
            var resolution = radius * RadiusToResolution;

            Draw.Circle(position, radius, color, (int)thickness, (int)resolution);
        }
    }
}