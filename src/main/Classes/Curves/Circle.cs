using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OpenGeometryEngine.Intersection.Unbounded;

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
        => ((object)transformMatrix == (object)Matrix.Identity)
            ? new Circle(this)
            : new Circle(transformMatrix * Frame, Radius);

    ICurve ICurve.CreateTransformedCopy(Matrix transformMatrix)
        => CreateTransformedCopy(transformMatrix);

    public override double GetLength(Interval interval)
        => Math.Abs((interval.End - interval.Start) * Radius);

    public ICollection<IntersectionPoint<ICurveEvaluation, ICurveEvaluation>> IntersectCurve(ICurve other)
    {
        if (other == null) throw new ArgumentNullException();

        switch (other.Geometry)
        {
            case Line line:
                return LineCircleIntersection.LineIntersectCircle(line, this);
            case Circle circle:
                throw new NotImplementedException();
            default:
                throw new NotImplementedException();
        }
    }

    public ICurve Curve => this;

    public new ICurveEvaluation ProjectPoint(Point point)
    {
        var proj = Plane.ProjectPoint(point);
        var centerToPoint = proj.Point - Frame.Origin;
        var angle = Vector.SignedAngle(Frame.DirX, centerToPoint.Unit, Frame.DirZ);
        if (angle < 0.0)
            angle = 2 * Math.PI + angle;
        return new CircleEvaluation(this, angle);
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

    public static IEnumerable<CircleEvaluation> GetExtremePointInDir(Circle circle, UnitVec dir)
    {
        if (circle == null) throw new ArgumentNullException(nameof(circle));
        var cross = Vector.Cross(dir, circle.Frame.DirZ);
        if (cross == Vector.Zero) return Enumerable.Empty<CircleEvaluation>();
        var crossUnit = cross.Unit;
        var dotX = Vector.Dot(crossUnit, circle.Frame.DirX);
        var dotY = Vector.Dot(crossUnit, circle.Frame.DirY);
        var param = Math.Atan2(dotX, dotY);
        return new Pair<CircleEvaluation>(new CircleEvaluation(circle, param), 
            new CircleEvaluation(circle, param + Math.PI));
    }

    public ICollection<CircleEvaluation> GetPolyline(PolylineOptions options, Interval interval)
    {
        int steps = (int)(GetStepNum(this, options) * Math.PI / interval.Span);
        if (Accuracy.EqualAngles(interval.Span, 2 * Math.PI)) steps++;
        var evals = new CircleEvaluation[steps];
        var theta = interval.Span / steps;
        for (int i = 0; i < steps; i++)
        {
            evals[i] = (CircleEvaluation)Evaluate(interval.Start + theta * i);
        }
        return evals;
    }

    private static int GetStepNum(Circle circle, PolylineOptions options)
    {
        int angularNum = (int)Math.Ceiling(2 * Math.PI / options.AngularDeviation);
        //https://math.stackexchange.com/questions/4132060/compute-number-of-regular-polgy-sides-to-approximate-circle-to-defined-precision
        int chordTolNum = (int)Math.Ceiling(Math.PI / Math.Sqrt(2 * options.ChordTolerance / circle.Radius));
        return Math.Max(angularNum, chordTolNum);
    }
}