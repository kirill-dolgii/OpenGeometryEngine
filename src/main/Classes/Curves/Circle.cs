using System;

namespace OpenGeometryEngine;

public sealed class Circle : CurveBase, ICircle
{
    private Circle() { }

    public Circle(Frame frame, double radius)
    {
        Frame = frame;
        Radius = radius;
        Direction = frame.DirZ;
        Plane = new Plane(frame);
        Origin = frame.Origin;
    }

    public Circle(ICircle otherCircle) : this(otherCircle.Frame, otherCircle.Radius) { }

    public Circle(Point origin, UnitVec unitX, UnitVec unitY, double radius)
        : this(new Frame(origin, unitX, unitY, Vector.Cross(unitX, unitY).Unit), radius) { }

    public new ICircle Clone() => new Circle(this);

    ICurve ICurve.Clone() => Clone();

    public ICircle CreateTransformedCopy(Matrix transformMatrix)
        => new Circle(transformMatrix * Frame, Radius);

    ICurve ICurve.CreateTransformedCopy(Matrix transformMatrix)
        => CreateTransformedCopy(transformMatrix);

    public override double GetLength(Interval interval)
        => Math.Abs((interval.End - interval.Start) * Radius);

    public ICurve Curve => this;

    public new ICurveEvaluation ProjectPoint(Point point)
    {
        var proj = Plane.ProjectPoint(point);
        var centerToPoint = proj.Point - Frame.Origin;
        return new CircleEvaluation(this, Vector.SignedAngle(centerToPoint.Unit, Frame.DirX, Frame.DirZ));
    }

    public ICurveEvaluation Evaluate(double param) => new CircleEvaluation(this, param);

    public Parametrization Parametrization => defaultCircleParametrization;

    public UnitVec Direction { get; }

    public Point Origin { get; }

    public Plane Plane { get; }

    public Frame Frame { get; }

    public double Radius { get; }

    private static readonly Parametrization defaultCircleParametrization =
        new Parametrization(new Bounds(.0, 2 * Math.PI), Form.Periodic);
}