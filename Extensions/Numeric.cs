using System;

namespace OpenGeometryEngine.Extensions;

public static class Numeric
{
    public static T Clamp<T>(this T value, T min, T max) where T : IComparable
    {
        var cmp1 = value.CompareTo(min);
        var cmp2 = value.CompareTo(max);
        if (cmp1 <= 0) return min;
        if (cmp2 >= 0) return max;
        return value;
    }
}