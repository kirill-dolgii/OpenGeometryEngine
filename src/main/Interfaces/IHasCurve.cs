namespace OpenGeometryEngine;

public interface IHasCurve : IHasGeometry, ISpatial
{
    ICurve Curve { get; }
    ICurveEvaluation ProjectPoint(Point point);
    ICurveEvaluation Evaluate(double param);
    Parametrization Parametrization { get; }
}