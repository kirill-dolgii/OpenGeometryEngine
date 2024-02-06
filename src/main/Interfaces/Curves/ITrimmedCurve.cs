using System.Collections.Generic;

namespace OpenGeometryEngine;

public interface ITrimmedCurve : IBounded, IHasCurve
{
    new ICurveEvaluation ProjectPoint(Point point);

    Interval Interval { get; }

    double Length { get; }

    new ICurveEvaluation Evaluate(double param);

    ICurveEvaluation EvaluateAtProportion(double param);

    ICollection<IntersectionPoint<ICurveEvaluation, ICurveEvaluation>> IntersectCurve(ITrimmedCurve other);

    Point StartPoint { get; }

    Point EndPoint { get; }

    Point MidPoint { get; }

    ICurve CreateTransformedCopy(Matrix transfMatrix);

    ICollection<ICurveEvaluation> GetPolyline(PolylineOptions options);
}