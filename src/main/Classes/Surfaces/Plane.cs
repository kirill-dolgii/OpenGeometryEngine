using OpenGeometryEngine.Misc.Enums;

namespace OpenGeometryEngine;

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

    public static Plane PlaneXY = new(Frame.World);

    public ISurface Surface { get; }

    public ISurfaceEvaluation Evaluate(PointUV param) => new PlaneEvaluation(this, param);

    public new ISurfaceEvaluation ProjectPoint(Point point)
    {
        var pointToOriginVec = point - Origin;
        var projX = Vector.Dot(pointToOriginVec, Frame.DirX);
        var projY = Vector.Dot(pointToOriginVec, Frame.DirY);
        return new PlaneEvaluation(this, new PointUV(projX, projY));
    }

    private static readonly UV<Parametrization> defaultPlaneParametrization
        = new (new Parametrization(new Bounds(null, null), Form.Open),
               new Parametrization(new Bounds(null, null), Form.Open));

    public UV<Parametrization> Parametrization => defaultPlaneParametrization;

    public new ISurface CreateTransformedCopy(Matrix transfMatrix) => new Plane(transfMatrix * Frame);

    public new ISurface Clone() => new Plane(this);
}