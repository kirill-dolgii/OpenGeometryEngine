using System.Collections.Generic;

namespace OpenGeometryEngine;

public abstract class Curve : IGeometry
{
    public readonly Parametrization Parametrization;
    public bool ContainsParam(double param) =>
        Parametrization.Bounds.ContainsParam(param);
    protected Curve(Parametrization parametrization) => Parametrization = parametrization;
    public bool ContainsPoint(Point point) =>
        (ProjectPoint(point).Point - point) .Magnitude <= Constants.Tolerance;
    Point ISpatial.ProjectPoint(Point point) => ProjectPoint(point).Point;
    IGeometry IGeometry.CreateTransformedCopy(Matrix transformationMatrix) => 
        CreateTransformedCopy(transformationMatrix);
    public abstract CurveEvaluation ProjectPoint(Point point);
    public abstract Curve CreateTransformedCopy(Matrix transformationMatrix);
    public abstract bool IsCoincident(IGeometry otherGeometry);
    public abstract ICollection<IntersectionPoint<CurveEvaluation, CurveEvaluation>> IntersectCurve(Curve otherCurve);
    public abstract CurveEvaluation Evaluate(double param);
}
