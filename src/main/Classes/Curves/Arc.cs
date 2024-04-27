using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenGeometryEngine.Collections;
using OpenGeometryEngine.Exceptions;
using OpenGeometryEngine.Extensions;
using OpenGeometryEngine.Intersection.Unbounded;

namespace OpenGeometryEngine;

public class Arc : IBoundedCurve
{
    private Arc() {}
        
    public Arc(Frame frame, double radius, Interval bounds)
    {
        Circle = new Circle(frame, radius);
        Interval = bounds;
        Length = radius * bounds.Span;
        StartPoint = Evaluate(bounds.Start).Point;
        MidPoint = Evaluate(bounds.Start + bounds.Span / 2).Point;
        EndPoint = Evaluate(bounds.End).Point;
    }

    public Arc(Point center, Point startPoint, Point endPoint, UnitVec axis)
    {
        var vecX = startPoint - center;
        var radius = vecX.Magnitude;
        var dirX = vecX.Unit;
        var dirY = Vector.Cross(axis, dirX).Unit;
        var angle = Math.Atan2(Vector.Dot(dirY, endPoint - center), Vector.Dot(dirX, endPoint - center));
        if (angle < 0.0)
            angle = 2 * Math.PI + angle;

        Circle = new Circle(new Frame(center, dirX, dirY, axis), radius);
        Interval = new Interval(0.0, angle);
        Length = radius * angle;
        StartPoint = Evaluate(Interval.Start).Point;
        MidPoint = Evaluate(angle / 2).Point;
        EndPoint = Evaluate(angle).Point;
    }
    
    public Arc(Circle circle, Interval interval) : this(circle.Frame, circle.Radius, interval) {}
    
    public Arc(Arc other)
    {
        Circle = other.Circle;
        Interval = other.Interval;
        Length = other.Length;
        StartPoint = other.StartPoint;
        MidPoint = other.MidPoint;
        EndPoint = other.EndPoint;

    }

    public Box GetBoundingBox()
    {
        var boxPoints = new List<Point>() { StartPoint, EndPoint };
        foreach (var worldDirection in UnitVec.WorldDirections)
        {
            var evals = Circle.GetExtremePointInDir(Circle, worldDirection);
            if (!evals.Any()) continue;
            if (Accuracy.WithinAngleInterval(Interval, evals.First().Param))
                boxPoints.Add(evals.First().Point);
            if (Accuracy.WithinAngleInterval(Interval, evals.ElementAt(1).Param))
                boxPoints.Add(evals.ElementAt(1).Point);
        }
        return Box.Create(boxPoints);
    }

    public IGeometry Geometry => Circle;

    public TGeometry? GetGeometry<TGeometry>() where TGeometry : class, IGeometry => Circle as TGeometry;

    public bool IsGeometry<TGeometry>() where TGeometry : class, IGeometry => Circle is TGeometry;

    public bool ContainsPoint(Point point)
    {
        var proj = Circle.ProjectPoint(point);
        return Accuracy.EqualLengths(point.Vector.Magnitude, proj.Point.Vector.Magnitude) &&
               Accuracy.WithinAngleInterval(Interval, proj.Param); 
    }

    public ICurveEvaluation ProjectPoint(Point point)
    {
        var param = Circle.ProjectPoint(point).Param;
        return new CircleEvaluation(Circle, Interval.Project(param, Accuracy.AngularTolerance));
    }

    public ICurveEvaluation Evaluate(double param) => new CircleEvaluation(Circle, param);

    public ICurveEvaluation EvaluateAtProportion(double param)
    {
        if (Accuracy.WithinRangeWithTolerance(0.0, 1.0, param))
            throw new ProportionOutsideBoundsException(nameof(param));
        return Evaluate(Interval.Start + param * Interval.Span);
    }

    public ICollection<IntersectionPoint<ICurveEvaluation, ICurveEvaluation>> IntersectCurve(IBoundedCurve other)
    {
        Argument.IsNotNull(nameof(other), other);
        return IntersectCurve(other.Curve)
                   .Where(ci =>
                   {
                       return other.Geometry switch
                       {
                           Line line => Accuracy.WithinLengthInterval(other.Interval, ci.SecondEvaluation.Param),
                           Circle circle => Accuracy.WithinAngleInterval(other.Interval, ci.SecondEvaluation.Param),
                           _ => throw new NotImplementedException()
                       };
                   })
                   .ToArray();
    }

    public ICollection<IntersectionPoint<ICurveEvaluation, ICurveEvaluation>> IntersectCurve(ICurve other)
    {
        Argument.IsNotNull(nameof(other), other);
        switch (other.Geometry)
        {
            case Line line:
                var intersections = LineCircleIntersection.LineIntersectCircle(line, Circle)
                    .Select(ip => new IntersectionPoint<ICurveEvaluation, ICurveEvaluation>
                        (ip.SecondEvaluation, ip.FirstEvaluation)).ToArray();
                var suitableIntersections = intersections
                    .Where(ip => Accuracy.WithinAngleInterval(Interval, ip.FirstEvaluation.Param)).ToArray();
                return suitableIntersections;
            default: throw new NotImplementedException();
        }
    }

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
        var bounds = Iterate.Over(Interval.Start, Interval.End);
        var nonBoundedParams = parameters.Except(bounds);
        if (!nonBoundedParams.Any()) return Array.Empty<IBoundedCurve>();
        var suitableParams = nonBoundedParams
            .Where(param => Accuracy.WithinLengthInterval(Interval, param))
            .Concat(bounds)
            .OrderBy(param => param).ToArray();
        //bool isClosed = Interval.Span == 2 * Math.PI;
        return suitableParams.Pairs(closed: false)
            .Select(paramPair => new Arc(Circle,
            new Interval(paramPair.First, paramPair.Second))).ToArray();
    }

    public Point StartPoint { get; }
    public Point EndPoint { get; }
    public Point MidPoint { get; }

    public Arc CreateTransformedCopy(Matrix transfMatrix)
    {
        if ((object)transfMatrix == (object)Matrix.Identity) return new Arc(this);
        return new Arc(transfMatrix * Circle.Frame, Circle.Radius, Interval);
    }

    public ICollection<ICurveEvaluation> GetPolyline(PolylineOptions options)
    {
        return Circle.GetPolyline(options, Interval).Cast<ICurveEvaluation>().ToArray();
    }

    ICurve IBoundedCurve.CreateTransformedCopy(Matrix transfMatrix) 
        => CreateTransformedCopy(transfMatrix).Curve;

    public Interval Interval { get; }

    public double Length { get; }

    public Parametrization Parametrization => Curve.Parametrization;

    public Circle Circle { get; }

    public ICurve Curve => Circle;

    Point ISpatial.ProjectPoint(Point point) => ProjectPoint(point).Point;
}