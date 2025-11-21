using System;

namespace Celeste.Mod.GooberHelper;

public static partial class Utils {
    public static float FirstSign(params float[] values) {
        for(var i = 0; i < values.Length; i++) {
            var sign = Math.Sign(values[i]);
            
            if(sign != 0)
                return sign;
        }

        return 0f;
    }

    public static float UnsignedAbsMax(params float[] values) {
        var max = Math.Abs(values[0]);

        for(var i = 1; i < values.Length; i++) {
            max = Math.Max(max, Math.Abs(values[i]));
        }

        return max;
    }

    public static float SignedAbsMax(params float[] values) {
        var max = Math.Abs(values[0]);
        var sign = Math.Sign(values[0]);

        for(var i = 1; i < values.Length; i++) {
            max = Math.Max(max, Math.Abs(values[i]));

            if(sign == 0f)
                sign = Math.Sign(values[i]);
        }

        return sign * max;
    }
}