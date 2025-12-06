using System;

namespace Celeste.Mod.GooberHelper {
    public partial class Utils {
        //stolen from https://gist.github.com/doomlaser/c7b894fa4936297195a053eda21fc0a0
        public static Vector4 RgbToHsv(this Vector4 color) {
            var a = color.W;
            var r = color.X;
            var g = color.Y;
            var b = color.Z;

            var min = MathF.Min(MathF.Min(r, g), b);
            var max = MathF.Max(MathF.Max(r, g), b);
            var delta = max - min;
            
            var value = max;
            var saturation = delta / max;

            if(max < float.Epsilon)
                return new Vector4(0, 0, value, a);
            
            if(min == max)
                return new Vector4(0, 0, max, a);
            
            var hue = 
                r == max ? 0 + (g - b) / delta :
                g == max ? 2 + (b - r) / delta :
                b == max ? 4 + (r - g) / delta :
                0;

            hue /= 6f;
            
            if(hue < 0)
                hue += 1;

            return new Vector4(hue, saturation, value, a);
        }

        //copied from Calc.HsvToColor
        //i dont understand what the hell its doing but i formatted it nicely
        public static Vector4 HsvToRgb(this Vector4 color) {
            var num = (int)(color.X * 360f) % 360f;
            var num2 = color.Y * color.Z;
            var num3 = num2 * (1f - Math.Abs(num / 60f % 2f - 1f));
            var num4 = color.Z - num2;

            return
                num < 060 ? new Vector4(num4 + num2, num4 + num3, num4, color.W) :
                num < 120 ? new Vector4(num4 + num3, num4 + num2, num4, color.W) :
                num < 180 ? new Vector4(num4, num4 + num2, num4 + num3, color.W) :
                num < 240 ? new Vector4(num4, num4 + num3, num4 + num2, color.W) :
                num < 300 ? new Vector4(num4 + num3, num4, num4 + num2, color.W) :
                num < 360 ? new Vector4(num4 + num2, num4, num4 + num3, color.W) :
                throw new InvalidOperationException();
        }

        public static Color MultiplyByAlpha(this Color color)
            => new(
                (byte)(color.R * color.A / 255f),
                (byte)(color.G * color.A / 255f),
                (byte)(color.B * color.A / 255f),
                color.A
            );

        public static Color ToColor(this Vector4 color)
            => new(color);

        public static string ToStringRgba(this Vector4 color)
            => $"rgba({color.X * 255:F0}, {color.Y * 255:F0}, {color.Z * 255:F0}, {color.W * 255:F0})";

        public static string ToStringHex(this Vector4 color)
            => $"#{(int)(color.X * 255):X0}{(int)(color.Y * 255):X0}{(int)(color.Z * 255):X0}{(int)(color.W * 255):X0}";

        public static string ToStringHsva(this Vector4 color)
            => $"hsva({color.X * 360:F0}, {color.Y * 100:F0}, {color.Z * 100:F0}, {color.W * 255:F0})";
    }
}