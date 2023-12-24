//using static System.Math;

//namespace OpenGeometryEngine;

///// <summary>
///// Represents a plane in 3D space.
///// </summary>
//public class Plane : IGeometry
//{
//    /// <summary>
//    /// Initializes a new instance of the <see cref="Plane"/> class with the specified point on the plane
//    /// and the normal vector.
//    /// </summary>
//    /// <param name="frame">A <see cref="Frame"/> coordinate system to build a plane on.</param>

//    public Plane(Frame frame) => Frame = frame;

//    public readonly Frame Frame;

//    /// <summary>
//    /// Checks if a given point lies on the plane.
//    /// </summary>
//    /// <param name="point">The <see cref="Point"/> to check.</param>
//    /// <returns><c>true</c> if the point lies on the plane; otherwise, <c>false</c>.</returns>
//    public bool ContainsPoint(Point point)
//    {
//        Vector pointToPlane = point - Frame.Origin;
//        double dotProduct = Vector.Dot(pointToPlane, Frame.DirZ);
//        return Abs(dotProduct) == 0;
//    }

//    /// <summary>
//    /// Projects a point onto the plane.
//    /// </summary>
//    /// <param name="point">The <see cref="Point"/> to project onto the plane.</param>
//    /// <returns>The projected <see cref="Point"/> on the plane.</returns>
//    public Point ProjectPoint(Point point)
//    {
//        Vector pointToPlane = point - Frame.Origin;
//        double distance = Vector.Dot(pointToPlane, Frame.DirZ);
//        return point - Frame.DirZ * distance;
//    }

//    /// <summary>
//    /// Converts a point from the local coordinate system to global coordinates.
//    /// </summary>
//    /// <returns>The point in global coordinates.</returns>
//    public Point Evaluate(double u, double v) => Frame.Origin + Frame.DirX * u + Frame.DirY * v;

//    IGeometry IGeometry.CreateTransformedCopy(Matrix transformationMatrix)
//    {
//        throw new System.NotImplementedException();
//    }
    
//    public bool IsCoincident(IGeometry otherGeometry)
//    {
//        var otherPlane = (Plane)otherGeometry;
//        if (otherPlane == null) return false;
//        return Frame.Origin == otherPlane.Frame.Origin && 
//               Frame.DirZ == otherPlane.Frame.DirZ;
//    }
//}

using OpenGeometryEngine;
using OpenGeometryEngine.Interfaces.Surfaces;

public sealed class Plane : FrameSurfaceBase, ISurface
{
    public Plane(Frame frame) : base(frame)
    {
    }

    public Plane(IHasFrame hasFrame) : base(hasFrame)
    {
    }

    public Plane(Point origin, UnitVec dirX, UnitVec dirY) : base(origin, dirX, dirY)
    {
    }

    public ISurface Surface { get; }

    public ISurfaceEvaluation Evaluate(PointUV param) => new PlaneEvaluation(this, param);

    public new ISurfaceEvaluation ProjectPoint(Point point)
    {
        var projX = Vector.Dot(point.Vector, Frame.DirX);
        var projY = Vector.Dot(point.Vector, Frame.DirY);
        return new PlaneEvaluation(this, new PointUV(projY, projY));
    }

    private static readonly UV<Parametrization> defaultPlaneParametrization
        = new (new Parametrization(new Bounds(null, null), Form.Open),
               new Parametrization(new Bounds(null, null), Form.Open));

    public UV<Parametrization> Parametrization => defaultPlaneParametrization;

    public new ISurface CreateTransformedCopy(Matrix transfMatrix) => new Plane(transfMatrix * Frame);

    public new ISurface Clone() => new Plane(this);
}