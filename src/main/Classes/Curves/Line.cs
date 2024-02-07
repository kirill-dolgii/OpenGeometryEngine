using System;
using System.Collections.Generic;
using OpenGeometryEngine.Intersection.Unbounded;

namespace OpenGeometryEngine;

public sealed class Line : CurveBase, ILine
{
    private readonly UnitVec dir;
    private readonly Point origin;

    private static readonly Parametrization defaultLineParametrization =
        new Parametrization(new Bounds(null, null), Form.Open);

    private Line() {}

    public Line(Point origin, UnitVec dir)
    {
        this.dir = dir;
        this.origin = origin;
    }

    public Line(Point firstPoint, Point secondPoint) 
        : this(firstPoint, new UnitVec(secondPoint - firstPoint)) {}

    public Line(ILine otherLine) : this(otherLine.Origin, otherLine.Direction) { }

    public ILine CreateTransformedCopy(Matrix transformMatrix)
        => (object) transformMatrix == (object) Matrix.Identity ? 
            new Line(this) : 
            new Line(transformMatrix * origin, transformMatrix * dir);

    ICurve ICurve.CreateTransformedCopy(Matrix transformMatrix)
        => CreateTransformedCopy(transformMatrix);

    public new ILine Clone() => (ILine) new Line((ILine) this);

    ICurve ICurve.Clone() => Clone();

    public override double GetLength(Interval interval) => interval.Span;

    public ICollection<IntersectionPoint<ICurveEvaluation, ICurveEvaluation>> IntersectCurve(ICurve other)
    {
        if (other == null) throw new ArgumentNullException();
        switch (other.Geometry)
        {
            case Line line:
                return LineLineIntersection.LineIntersectLine(this, line);
            case Circle circle:
                return LineCircleIntersection.LineIntersectCircle(this, circle);
            default:
                throw new NotImplementedException();
        }
    }

    public UnitVec Direction => dir;
    public Point Origin => origin;
    public ICurve Curve => base.ICurve;

    public new ICurveEvaluation ProjectPoint(Point point)
    {
        var vec = point - origin;
        var param = Vector.Dot(vec, dir);
        return new LineEvaluation(this, param);
    }

    public ICurveEvaluation Evaluate(double param)
        => new LineEvaluation(this, param);

    public Box GetBoundingBox(Interval interval)
    {
        var eval1 = this.Evaluate(interval.Start);
        var eval2 = this.Evaluate(interval.End);
        return Box.Create(eval1.Point, eval2.Point);
    }

    public Parametrization Parametrization => defaultLineParametrization;

    public static bool AreCoincident(Line line1, Line line2, 
                                     double tolerance = Accuracy.DefaultLinearTolerance)
    {
        if (line1 == line2) return true;
        if (line1 == null || line2 == null) return false;
        if (!UnitVec.AreParallel(line1.Direction, line2.Direction)) return false;
        return Accuracy.IsZero((line1.origin - line2.origin).Magnitude, tolerance);
    }

    public ICollection<LineEvaluation> GetPolyline(PolylineOptions options, Interval interval)
    {
        int steps = (int)(Math.Ceiling(interval.Span / options.MaxChordLength));
        var evals = new LineEvaluation[steps];
        var theta = interval.Span / steps;
        for (int i = 0; i < steps; i++)
        {
            evals[i] = (LineEvaluation)Evaluate(interval.Start + theta * i);
        }
        return evals;
    }
}