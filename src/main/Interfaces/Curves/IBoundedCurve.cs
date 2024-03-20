using System.Collections.Generic;

namespace OpenGeometryEngine;

public interface IBoundedCurve : IBounded, IHasCurve
{
    new ICurveEvaluation ProjectPoint(Point point);

    Interval Interval { get; }

    double Length { get; }

    new ICurveEvaluation Evaluate(double param);

    ICurveEvaluation EvaluateAtProportion(double param);

    ICollection<IntersectionPoint<ICurveEvaluation, ICurveEvaluation>> IntersectCurve(IBoundedCurve other);

    ICollection<IntersectionPoint<ICurveEvaluation, ICurveEvaluation>> IntersectCurve(ICurve other);

    Point StartPoint { get; }

    Point EndPoint { get; }

    Point MidPoint { get; }

    ICurve CreateTransformedCopy(Matrix transfMatrix);

    ICollection<IBoundedCurve> Split(ICurve curve);

    ICollection<IBoundedCurve> Split(ICollection<double> parameters);

    ICollection<ICurveEvaluation> GetPolyline(PolylineOptions options);
}