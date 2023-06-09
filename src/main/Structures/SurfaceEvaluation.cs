namespace OpenGeometryEngine;

public readonly struct SurfaceEvaluation
{
    public PointUV Param { get; }
    public Point Point { get; }
    internal SurfaceEvaluation(PointUV param, Point point) =>
        (Param, Point) = (param, point);
}