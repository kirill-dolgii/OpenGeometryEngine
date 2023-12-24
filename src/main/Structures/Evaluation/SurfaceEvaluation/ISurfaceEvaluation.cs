namespace OpenGeometryEngine;

public interface ISurfaceEvaluation
{
    PointUV Param { get; }
    Point Point { get; }
    UnitVec Normal { get; }
}