namespace OpenGeometryEngine;

public interface ICurve : IGeometry, IHasCurve
{
    new ICurve CreateTransformedCopy(Matrix transformMatrix);
    new ICurve Clone();
    double GetLength(Interval interval);
}