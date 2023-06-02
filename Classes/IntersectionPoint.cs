namespace OpenGeometryEngine;

public readonly struct IntersectionPoint
{
    public readonly Curve FirstCurve;
    public readonly Curve SecondCurve;

    public readonly CurveEvaluation FirstCurveEvaluation;
    public readonly CurveEvaluation SecondCurveEvaluation;

    public readonly Point;
}