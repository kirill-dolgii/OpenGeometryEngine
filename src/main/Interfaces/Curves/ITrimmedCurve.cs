using OpenGeometryEngine;
using OpenGeometryEngine.Interfaces.Curves;

public interface ITrimmedCurve : IBounded, IHasCurve
{
    new ICurveEvaluation ProjectPoint(Point point);

    Interval Interval { get; }

    double Length { get; }

    new ICurveEvaluation Evaluate(double param);

    ICurveEvaluation EvaluateAtProportion(double param);

    Point StartPoint { get; }

    Point EndPoint { get; }

    Point MidPoint { get; }

    ICurve CreateTransformedCopy(Matrix transfMatrix);
}