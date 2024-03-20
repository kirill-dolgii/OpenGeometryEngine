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
        var param = Circle.ProjectPoint(point).Param;
        return Accuracy.WithinAngleInterval(Interval, param); 
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
        var arcParams = new List<(double, double)>();
        switch (curve)
        {
            case Line:
            {
                var intersections = IntersectCurve(curve);
                switch (intersections.Count)
                {
                    case 0: return Array.Empty<IBoundedCurve>();
                    case 1:
                    {
                        arcParams.Add(new (Interval.Start, intersections.Single().FirstEvaluation.Param));
                        arcParams.Add(new (intersections.Single().FirstEvaluation.Param, Interval.End));
                        break;
                    }
                    case 2:
                    {
                        if (!Accuracy.AreEqual(2 * Math.PI, Interval.Span))
                        {
                            arcParams = Iterate.Over(Interval.Start, Interval.End,
                                                     intersections.ElementAt(0).FirstEvaluation.Param,
                                                     intersections.ElementAt(1).FirstEvaluation.Param)
                                               .OrderBy(param => param).Pairs(closed:false).ToList();
                        }
                        else
                        {
                            var span = intersections.ElementAt(1).FirstEvaluation.Param -
                                       intersections.ElementAt(0).FirstEvaluation.Param;
                            arcParams = Enumerable.Range(0, 3).Select(i => i * span)
                                .Pairs(closed: false).ToList();
                        }
                        break;
                    }
                }
                break;
            }
            default: throw new NotImplementedException();
        }

        // if are equal => intersection is on Interval's bound
        var newArcParams = arcParams
            .Where(tpl => !Accuracy.AreEqual(tpl.Item1, tpl.Item2) &&
                            !(Accuracy.AreEqual(tpl.Item1, Interval.Start) && 
                              Accuracy.AreEqual(tpl.Item2, Interval.End))).ToArray();
        return newArcParams
            .Select(tpl => new Arc(Circle.Frame, Circle.Radius, new Interval(tpl.Item1, tpl.Item2)))
            .ToArray();
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