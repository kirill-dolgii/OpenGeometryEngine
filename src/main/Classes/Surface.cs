using System.Collections.Generic;

namespace OpenGeometryEngine;

public abstract class Surface : IGeometry
{
    public UV<Parametrization> Parametrization { get; }
    protected Surface(UV<Parametrization> parametrization) => Parametrization = parametrization;
    public abstract bool ContainsParam(PointUV pointUv);
    public abstract ICollection<SurfaceEvaluation> Evaluate(PointUV pointUv);
    public abstract UV<Bounds<bool>> IsSingular { get; }
    public abstract ICollection<SurfaceSurfaceIntersection> IntersectSurface(Surface otherSurface);
    public abstract ICollection<IntersectionPoint<SurfaceEvaluation, CurveEvaluation>> 
        IntersectCurve(Curve curve);
    public bool ContainsPoint(Point point) => ProjectPoint(point) == point;
    public abstract Point ProjectPoint(Point point);
    public abstract bool IsCoincident(IGeometry otherGeometry);
}
