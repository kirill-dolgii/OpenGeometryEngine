using System;
using OpenGeometryEngine;
using OpenGeometryEngine.Interfaces.Surfaces;

public abstract class FrameSurfaceBase : IGeometry, IHasFrame, IHasOrigin
{
    private FrameSurfaceBase() {}

    protected FrameSurfaceBase(Frame frame)
    {
        Frame = frame;
        Origin = frame.Origin;
    }

    protected FrameSurfaceBase(IHasFrame hasFrame) : this(hasFrame.Frame) {}

    protected FrameSurfaceBase(Point origin, UnitVec dirX, UnitVec dirY) 
        : this(new Frame(origin, dirX, dirY, Vector.Cross(dirX, dirY).Unit)) {}

    public IGeometry Geometry => this;

    public ISurface ISurface => (ISurface)this;

    public Frame Frame { get; }

    public Point Origin { get; }

    public TGeometry GetGeometry<TGeometry>() where TGeometry : class, IGeometry => this as TGeometry;

    public bool IsGeometry<TGeometry>() where TGeometry : class, IGeometry => this is TGeometry;

    public bool ContainsPoint(Point point) => ProjectPoint(point) == point;

    public Point ProjectPoint(Point point) => ISurface.ProjectPoint(point).Point;

    public IGeometry CreateTransformedCopy(Matrix transformMatrix) => CreateTransformedCopy(transformMatrix);

    public IGeometry Clone() => ISurface.Clone();

    object ICloneable.Clone()
    {
        return Clone();
    }
}