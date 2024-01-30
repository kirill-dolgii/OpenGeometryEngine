using OpenGeometryEngine.Extensions;

namespace OpenGeometryEngine;

public class PlaneEvaluation : ISurfaceEvaluation
{
    public Plane Plane { get; }
    public PlaneEvaluation(Plane plane, PointUV param)
    {
        Plane = plane;
        Param = param;
        Point = Plane.Origin + (Plane.Frame.DirX * param.U + Plane.Frame.DirY * param.V);
    }
    
    public PointUV Param { get; }
    public Point Point { get; }
    public UnitVec Normal => Plane.Frame.DirZ;
}