using System;
using OpenGeometryEngine;
using OpenGeometryEngine.Interfaces.Curves;

public abstract class CurveBase : IGeometry
{
    public ICurve ICurve => (ICurve)this;

    public bool ContainsPoint(Point point) => this.ProjectPoint(point) == point;

    public Point ProjectPoint(Point point) => this.ICurve.ProjectPoint(point).Point;

    IGeometry IGeometry.CreateTransformedCopy(Matrix transformMatrix) 
        => (IGeometry)this.ICurve.CreateTransformedCopy(transformMatrix);

    public IGeometry Clone() => this.ICurve.Clone();

    object ICloneable.Clone() => this.ICurve.Clone();

    public abstract double GetLength(Interval interval);

    public IGeometry Geometry => (IGeometry)this;

    public TGeometry GetGeometry<TGeometry>() where TGeometry : class, IGeometry => this as TGeometry;

    public bool IsGeometry<TGeometry>() where TGeometry : class, IGeometry => this is TGeometry;
}