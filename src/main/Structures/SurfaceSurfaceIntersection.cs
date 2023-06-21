using System.Collections.Generic;

namespace OpenGeometryEngine;

public readonly struct SurfaceSurfaceIntersection
{
    public ICollection<Curve> Curves { get; }
    public ICollection<Point> Points { get; }
    internal SurfaceSurfaceIntersection(ICollection<Curve> curves, ICollection<Point> points) =>
        (Curves, Points) = (curves, points);
}