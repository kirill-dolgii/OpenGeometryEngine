namespace OpenGeometryEngine.Extensions;

public static class VectorExtensions
{
    public static Point ToPoint(this Vector v) => new Point(v.X, v.Y, v.Z);
}