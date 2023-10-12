using System.Collections.Generic;

namespace OpenGeometryEngine;

public abstract class Curve : IGeometry
{
    protected Curve(Parametrization parametrization)                           
        => Parametrization = parametrization;

    public Parametrization Parametrization { get; }

    public bool IsCoincident(IGeometry otherGeometry)
    {
        var otherCurve = otherGeometry as Curve;
        if (otherCurve == null) return false;
        return IsCoincident(otherCurve);
    }

    Point IGeometry.ProjectPoint(Point point) => ProjectPoint(point).Point;

    public abstract CurveEvaluation ProjectPoint(Point point);
    
    public abstract bool IsCoincident(Curve otherCurve);
    
    public abstract ICollection<IntersectionPoint<CurveEvaluation, CurveEvaluation>> 
        IntersectCurve(Curve otherCurve);

    public abstract CurveEvaluation Evaluate(double param);

    public abstract bool ContainsParam(double param);

    public virtual bool ContainsPoint(Point point)
    {
        var proj = ProjectPoint(point);
        return ContainsParam(proj.Param);
    }
}