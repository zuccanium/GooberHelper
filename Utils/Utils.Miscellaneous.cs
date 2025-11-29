using System.Reflection;

namespace Celeste.Mod.GooberHelper {
    public static partial class Utils {
        public static readonly BindingFlags BindingFlagsAll =
            BindingFlags.NonPublic |
            BindingFlags.Public |
            BindingFlags.Instance |
            BindingFlags.Static;
    }
}