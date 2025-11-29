using System;
using System.Runtime.CompilerServices;

namespace Celeste.Mod.GooberHelper {
    public static partial class Utils {
        public static readonly string AnsiReset = "\e[0m";
        public static string GetAnsiBackgroundColorCode(Color color) => $"\e[48;2;{color.R};{color.G};{color.B}m";
        public static string GetAnsiForegroundColorCode(Color color) => $"\e[38;2;{color.R};{color.G};{color.B}m";
        public static string GetAnsiColorCode(Color color) => GetAnsiForegroundColorCode(color);

        //i love boilerplate
        public static string Color(this string str, Color color) => GetAnsiColorCode(color) + str + AnsiReset;
        public static string Color(this string str, Color foreground, Color background) => GetAnsiColorCode(foreground) + GetAnsiColorCode(background) + str + AnsiReset;
        public static string Color(this string str, string color) => color + str + AnsiReset;
        public static string Color(this string str, string foreground, string background) => foreground + background + str + AnsiReset;

        public static readonly string LogBackgroundColor1 = GetAnsiBackgroundColorCode(new Color(20, 20, 20));
        public static readonly string LogBackgroundColor2 = GetAnsiBackgroundColorCode(new Color(25, 25, 25));
        public static readonly string LogForegroundColor = GetAnsiForegroundColorCode(new Color(0, 255, 0));

        private static int logColorCycle = 0;

#if DEBUG
        public static void Log(DefaultInterpolatedStringHandler str) {
            var stringifiedStr = str.ToString();

            Console.WriteLine(
                stringifiedStr
                    .PadRight((int)MathF.Ceiling((float)stringifiedStr.Length / Console.WindowWidth) * Console.WindowWidth, ' ')
                    .Color(
                        GetAnsiColorCode(Calc.HsvToColor(
                            new System.Diagnostics.StackTrace().FrameCount * 0.8236f % 1,
                            1,
                            1
                        )),
                        logColorCycle++ % 2 == 0
                            ? LogBackgroundColor1
                            : LogBackgroundColor2
                    )
            );
        }

        public static void Log(string str)
            => Console.WriteLine(
                str
                    .PadRight((int)MathF.Ceiling((float)str.Length / Console.WindowWidth) * Console.WindowWidth, ' ')
                    .Color(
                        GetAnsiColorCode(Calc.HsvToColor(
                            new System.Diagnostics.StackTrace().FrameCount * 0.8236f % 1,
                            1,
                            1
                        )),
                        logColorCycle++ % 2 == 0
                            ? LogBackgroundColor1
                            : LogBackgroundColor2
                    )
            );
#else
        public static void Log(DefaultInterpolatedStringHandler str)
            => Logger.Verbose("GooberHelper", str);

        public static void Log(string str)
            => Logger.Verbose("GooberHelper", str);
#endif
    }
}