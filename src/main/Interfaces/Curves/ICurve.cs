using System.Collections.Generic;

namespace OpenGeometryEngine;

public interface ICurve : IGeometry, IHasCurve
{
    new ICurve CreateTransformedCopy(Matrix transformMatrix);
    new ICurve Clone();
    double GetLength(Interval interval);

    public ICollection<IntersectionPoint<ICurveEvaluation, ICurveEvaluation>> IntersectCurve(ICurve other);
}