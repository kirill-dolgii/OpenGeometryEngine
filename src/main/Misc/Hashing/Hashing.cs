using System;
using System.Linq;

namespace OpenGeometryEngine.Misc;

internal static class Hashing
{
    public static int GetHashCode(this Point point) => GetHashCode(point.X, point.Y, point.Z);

    public static int GetHashCode(this Vector vector) => GetHashCode(vector.X, vector.Y, vector.Z);

    public static int GetHashCode(double x, double y, double z)
    {
        var xBytes = BitConverter.GetBytes(x);
        var yBytes = BitConverter.GetBytes(y);
        var zBytes = BitConverter.GetBytes(z);
        var hashBytes = Enumerable.Range(0, 8)
            .Select(i => (byte)(xBytes[i] ^ yBytes[7 - i] ^ zBytes[i])).ToArray();
        return (int)(BitConverter.ToInt64(hashBytes, 0) % Int32.MaxValue);
    }
}