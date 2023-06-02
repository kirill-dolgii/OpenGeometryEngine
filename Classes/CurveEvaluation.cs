namespace OpenGeometryEngine;

public struct CurveEvaluation
{
    public readonly double Param;
    public readonly Point Point;

    public CurveEvaluation(double param, Point point)
    {
        Param = param;
        Point = point;
    }
}