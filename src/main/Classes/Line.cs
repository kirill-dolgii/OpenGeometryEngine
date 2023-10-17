using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using OpenGeometryEngine.Intersection;

namespace OpenGeometryEngine;

/// <summary>
/// Represents an infinite 3D line defined by an origin point and a direction vector.
/// </summary>
public sealed class Line : Curve
{
    /// <summary>
    /// The origin point of the line.
    /// </summary>
    public Point Origin { get; }
    /// <summary>
    /// The direction unit vector of the line.
    /// </summary>
    public Vector Direction { get; }

    /// <summary>
    /// Initializes a new Line instance with the specified origin and direction.
    /// </summary>
    /// <param name="origin">The origin point of the line.</param>
    /// <param name="direction">The direction vector of the line.</param>
    public Line(Point origin, Vector direction) : base(new Parametrization(Bounds.Unbounded, Form.Open))
        => (Origin, Direction) = (origin, direction);

    public override CurveEvaluation ProjectPoint(Point point)
    {
        var originToPoint = point - Origin;
        var t = Vector.Dot(originToPoint, Direction);
        return new CurveEvaluation(t, Evaluate(t).Point);
    }
        
    public override bool IsCoincident(Curve otherCurve)
    {
        var otherLine = otherCurve as Line;
        if (otherLine == null) return false;
        return Origin == otherLine.Origin &&
               Direction.IsParallel(otherLine.Direction);
    }

    public override ICollection<IntersectionPoint<CurveEvaluation, CurveEvaluation>> IntersectCurve(Curve otherCurve)
    {
        if (otherCurve == null) throw new ArgumentNullException(nameof(otherCurve));
        switch (otherCurve)
        {
            case Line line:
                return LineLineIntersection.LineIntersectLine(this, line);
            case Circle circle:
                return LineCircleIntersection.LineIntersectCircle(this, circle);
        }
        throw new NotImplementedException();
    }

    public override bool ContainsParam(double param) 
        => Accuracy.WithinAngleBounds(Parametrization.Bounds, param);

    /// <summary>
    /// Evaluates the line at the given parameter.
    /// </summary>
    /// <param name="param">Local coordinate.</param>
    /// <returns>Evaluated point at the specified parameter.</returns>
    public override CurveEvaluation Evaluate(double param)
    {
        if (!ContainsParam(param)) 
            throw new ArgumentException("param is not within bounds", nameof(param));
        return new CurveEvaluation(param, Origin + Direction * param);
    }
}