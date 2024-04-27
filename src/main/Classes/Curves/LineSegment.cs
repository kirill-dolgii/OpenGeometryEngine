using System;
using OpenGeometryEngine.Collections;
using OpenGeometryEngine.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime;
using OpenGeometryEngine.Extensions;

namespace OpenGeometryEngine;

public class LineSegment : IBoundedCurve
{
    private LineSegment()
    {
    }

    public LineSegment(LineSegment other)
    {
        Line = other.Line;
        Interval = other.Interval;
        StartPoint = other.StartPoint;
        MidPoint = other.MidPoint;
        EndPoint = other.EndPoint;
    }

    public LineSegment(ILine line, Interval interval)
    {
        Line = new(line);
        Interval = interval;
        StartPoint = EvaluateAtProportion(.0).Point;
        MidPoint = EvaluateAtProportion(.5).Point;
        EndPoint = EvaluateAtProportion(1.0).Point;
        Length = (EndPoint - StartPoint).Magnitude; 
    }

    public LineSegment(Point origin, UnitVec dir, Interval interval) : this(new Line(origin, dir), interval) {}

    public LineSegment(Point startPoint, Point endPoint) 
        : this(new Line(startPoint, endPoint), new Interval(.0, (endPoint - startPoint).Magnitude)) {}

    public Box GetBoundingBox() => Box.Create(StartPoint, EndPoint);

    public IGeometry Geometry => Line;

    public TGeometry? GetGeometry<TGeometry>() where TGeometry : class, IGeometry => Line as TGeometry;

    public bool IsGeometry<TGeometry>() where TGeometry : class, IGeometry => Line is TGeometry;

    public bool ContainsPoint(Point point) 
    {
        var eval = ProjectPoint(point);
        if (!Accuracy.LengthIsZero((eval.Point - point).Magnitude)) return false;
        return Accuracy.WithinLengthInterval(Interval, eval.Param);
    }

    public ICurveEvaluation ProjectPoint(Point point)
    {
        var param = Vector.Dot(point.Vector - Line.Origin.Vector, Line.Direction);
        return new LineEvaluation(Line, param);
    }

    public ICurveEvaluation Evaluate(double param) => new LineEvaluation(Line, param);

    public ICurveEvaluation EvaluateAtProportion(double param)
    {
        if (!Accuracy.WithinRangeWithTolerance(0.0, 1.0, param))
            throw new ProportionOutsideBoundsException(nameof(param));
        return Evaluate(Interval.Start + param * Interval.Span);
    }

    public ICollection<IntersectionPoint<ICurveEvaluation, ICurveEvaluation>> IntersectCurve(IBoundedCurve other)
    {
        Argument.IsNotNull(nameof(other), other);
        return IntersectCurve(other.Curve)
            .Where(ip => Accuracy.WithinLengthInterval(Interval, ip.FirstEvaluation.Param) &&
                         Accuracy.WithinLengthInterval(other.Interval, ip.SecondEvaluation.Param)).ToArray();
    }

    public ICollection<IntersectionPoint<ICurveEvaluation, ICurveEvaluation>> IntersectCurve(ICurve other)
    {
        Argument.IsNotNull(nameof(other), other);
        var inters = other.Geometry switch
        {
            Line line => Line.IntersectCurve(line),
            Circle circle => Line.IntersectCurve(circle),
            _ => throw new NotImplementedException()
        };
        return inters.Where(ip => Accuracy.WithinLengthInterval(Interval, ip.FirstEvaluation.Param)).ToArray();
    }

    public Point StartPoint { get; }
    public Point EndPoint { get; }
    public Point MidPoint { get; }

    public IEnumerable<Point> StartEndPoints => Iterate.Over(StartPoint, EndPoint);

    ICurve IBoundedCurve.CreateTransformedCopy(Matrix transfMatrix) 
        =>  Line.CreateTransformedCopy(transfMatrix);

    /// <summary>
    /// Splits the arc by a Curve. Intersection points that are on the bounds
    /// of the arc are not considered as split points.
    /// </summary>
    /// <param name="curve"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public ICollection<IBoundedCurve> Split(ICurve curve)
    {
        Argument.IsNotNull(nameof(curve), curve);
		double[] arcParams = IntersectCurve(curve).Select(ip => ip.FirstEvaluation.Param).ToArray();
        return Split(arcParams);
    }

    public ICollection<IBoundedCurve> Split(ICollection<double> parameters)
    {
        Argument.IsNotNull(nameof(parameters), parameters);
        var bounds = Iterate.Over(Interval.Start, Interval.End).ToArray();
        var nonBoundedParams = parameters.Where(param => !Accuracy.LengthIsZero(param)).Except(bounds).ToArray();
		if (!nonBoundedParams.Any()) return Array.Empty<LineSegment>();
        var suitableParams = nonBoundedParams
			.Where(param => Accuracy.WithinLengthInterval(Interval, param))
            .Concat(bounds)
            .OrderBy(param => param).ToArray();
        return suitableParams.Pairs(closed: false)
            .Select(paramPair => new LineSegment(Line, 
                    new Interval(paramPair.First, paramPair.Second))).ToArray();
    }

    public ICollection<ICurveEvaluation> GetPolyline(PolylineOptions options)
    {
        return Line.GetPolyline(options, Interval).Cast<ICurveEvaluation>().ToArray();
    }

    public LineSegment CreateTransformedCopy(Matrix transfMatrix) 
        => (object)transfMatrix == (object)Matrix.Identity ? 
            new LineSegment(this) : 
            new(transfMatrix * StartPoint, transfMatrix * EndPoint);

    public Interval Interval { get; }
    public double Length { get; }

    public Parametrization Parametrization => Line.Parametrization;

    public Line Line { get; }

    public ICurve Curve => Line;

    Point ISpatial.ProjectPoint(Point point) => ProjectPoint(point).Point;
}