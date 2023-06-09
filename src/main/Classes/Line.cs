using System;
using System.Collections.Generic;

using OpenGeometryEngine.Extensions;
using OpenGeometryEngine.Intersection;
using OpenGeometryEngine.Structures;

namespace OpenGeometryEngine;

/// <summary>
/// Represents an infinite 3D line defined by an origin point and a direction vector.
/// </summary>
public class Line : Curve
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
    {
        Origin = origin;
        Direction = direction.Normalize();
    }

    public override CurveEvaluation ProjectPoint(Point point)
    {
        var originToPoint = point - Origin;
        var t = Vector.Dot(originToPoint, Direction);
        return new CurveEvaluation(t, Evaluate(t).Point);
    }

    public override Curve CreateTransformedCopy(Matrix transformationMatrix) =>
        new Line(transformationMatrix * Origin, transformationMatrix * Direction);
        
    public override bool IsCoincident(IGeometry otherGeometry)
    {
        var otherLine = (Line) otherGeometry;
        if (otherLine == null) return false;
        return (Origin - otherLine.Origin).Magnitude <= Constants.Tolerance &&
               (Direction - otherLine.Direction).Magnitude <= Constants.Tolerance;
    }

    public override ICollection<IntersectionPoint<CurveEvaluation, CurveEvaluation>> IntersectCurve(Curve otherCurve)
    {
        var otherLine = (Line) otherCurve;
        if (otherLine != null) return CurveIntersection.LineIntersectLine(this, otherLine);
        return null;
    }

    /// <summary>
    /// Evaluates the line at the given parameter.
    /// </summary>
    /// <param name="param">Local coordinate.</param>
    /// <returns>Evaluated point at the specified parameter.</returns>
    public override CurveEvaluation Evaluate(double param)
    {
        if (!Parametrization.Bounds.ContainsParam(param)) throw new ArgumentException("param is not within bounds", nameof(param));
        return new CurveEvaluation(param, Origin + Direction * param);
    }
}