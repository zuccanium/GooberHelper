using System;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace Celeste.Mod.GooberHelper {
    public static partial class Utils {
        public static readonly BindingFlags BindingFlagsAll =
            BindingFlags.NonPublic |
            BindingFlags.Public |
            BindingFlags.Instance |
            BindingFlags.Static;
        
        // public static T RotateEnum<U, T>(T enumValue) where T : struct, Enum, U where U : INumber<U> {
        //     if(Enum.GetValuesAsUnderlyingType<T>() is not U[] intValues)
        //         return enumValue;
            
        //     return (T)((enumValue + U.AdditiveIdentity) % intValues.Last());
        // }

        public static T RotateEnum<T>(T enumValue, int dir) where T : struct, Enum {
            if(Enum.GetValues<T>() is not T[] values)
                return enumValue;
            
            var raw = Convert.ToInt32(enumValue) + dir;

            if(raw > Convert.ToInt32(values.Last()))
                return values.First();
            
            if(raw < Convert.ToInt32(values.First()))
                return values.Last();

            return (T)Enum.ToObject(typeof(T), raw);
        }
    }
}