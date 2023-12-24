//using System;
//using System.Collections.Generic;
//using OpenGeometryEngine.Intersection.Unbounded;

//namespace OpenGeometryEngine;

///// <summary>
///// Represents an infinite 3D line defined by an origin point and a direction vector.
///// </summary>
//public sealed class Line : ICurve
//{
//    /// <summary>
//    /// The origin point of the line.
//    /// </summary>
//    public Point Origin { get; }
//    /// <summary>
//    /// The direction unit vector of the line.
//    /// </summary>
//    public Vector Direction { get; }

//    private Line(Point origin, Vector direction) : base(new Parametrization(Bounds.Unbounded, Form.Open))
//        => (Origin, Direction) = (origin, direction);

//    /// <summary>
//    /// Initializes a new Line instance with the specified origin and direction.
//    /// </summary>
//    /// <param name="origin">The origin point of the line.</param>
//    /// <param name="direction">The direction vector of the line.</param>
//    public static Line Create(Point origin, Vector direction) => new (origin, direction.Normalize());

//    public override ICurve CreateTransformedCopy(Matrix transformMatrix)
//    {
//        return this;
//    }

//    public override CurveEvaluation ProjectPoint(Point point)
//    {
//        var originToPoint = point - Origin;
//        var t = Vector.Dot(originToPoint, Direction);
//        return Evaluate(t);
//    }

//    public override bool IsCoincident(ICurve otherCurve)
//    {
//        var otherLine = otherCurve as Line;
//        if (otherLine == null) return false;
//        return Origin == otherLine.Origin &&
//               Direction.IsParallel(otherLine.Direction);
//    }

//    public override ICollection<IntersectionPoint<CurveEvaluation, CurveEvaluation>> IntersectCurve(ICurve otherCurve)
//    {
//        if (otherCurve == null) throw new ArgumentNullException(nameof(otherCurve));
//        switch (otherCurve)
//        {
//            case Line line:
//                return LineLineIntersection.LineIntersectLine(this, line);
//            case Circle circle:
//                return LineCircleIntersection.LineIntersectCircle(this, circle);
//        }
//        throw new NotImplementedException();
//    }

//    public override bool ContainsParam(double param) 
//        => Accuracy.WithinAngleBounds(Parametrization.Bounds, param);

//    /// <summary>
//    /// Evaluates the line at the given parameter.
//    /// </summary>
//    /// <param name="param">Local coordinate.</param>
//    /// <returns>Evaluated point at the specified parameter.</returns>
//    public override CurveEvaluation Evaluate(double param)
//    {
//        if (!ContainsParam(param)) 
//            throw new ArgumentException("param is not within bounds", nameof(param));
//        return new CurveEvaluation(param, Origin + Direction * param);
//    }
//}

using System.Reflection;
using OpenGeometryEngine;
using OpenGeometryEngine.Interfaces.Curves;
using OpenGeometryEngine.Structures;

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

    public Line(ILine otherLine) 
        : this(otherLine.Origin, otherLine.Direction) { }

    public ILine CreateTransformedCopy(Matrix transformMatrix)
        => (object) transformMatrix == (object) Matrix.Identity ? 
            this : 
            new Line(transformMatrix * origin, transformMatrix * dir);

    ICurve ICurve.CreateTransformedCopy(Matrix transformMatrix)
        => CreateTransformedCopy(transformMatrix);

    public new ILine Clone() => (ILine) new Line((ILine) this);

    ICurve ICurve.Clone() => Clone();

    public override double GetLength(Interval interval)
        => interval.Span;

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

    public static bool AreCoincident(Line line1, Line line2, double? tolerance)
    {
        if (line1 == line2) return true;
        if (line1 == null || line2 == null) return false;
        if (!UnitVec.AreParallel(line1.Direction, line2.Direction)) return false;
        if (!tolerance.HasValue) tolerance = Accuracy.LinearTolerance;
        return Accuracy.CompareWithTolerance((line1.origin - line2.origin).Magnitude, 0.0, tolerance.Value) == 0;
    }
}