using System.Collections.Generic;

namespace OpenGeometryEngine;

public class PointEqualityComparer : IEqualityComparer<Point>
{
    public static PointEqualityComparer Default = new PointEqualityComparer();

    public bool Equals(Point p1, Point p2)
    {
        return Accuracy.EqualLengths(p1.X, p2.X) &&
               Accuracy.EqualLengths(p1.Y, p2.Y) &&
               Accuracy.EqualLengths(p1.Z, p2.Z);
    }

    public int GetHashCode(Point obj)
    {
        return 0;
    }
}