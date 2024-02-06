using System;
using OpenGeometryEngine.Collections;
using OpenGeometryEngine.Exceptions;
using System.Collections.Generic;
using System.Linq;

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

    public bool ContainsPoint(Point point) => ProjectPoint(point).Point == point;

    public ICurveEvaluation ProjectPoint(Point point)
    {
        var param = Vector.Dot(point.Vector, Line.Direction);
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
        switch (other.Geometry)
        {
            case Line line:
            {
                var inters = Line.IntersectCurve(line);
                return inters.Where(ip => Accuracy.WithinLengthInterval(Interval, ip.FirstEvaluation.Param) &&
                                          Accuracy.WithinLengthInterval(other.Interval, ip.SecondEvaluation.Param))
                                          .ToList();
            }
            default: throw new NotImplementedException();
        }
    }

    public Point StartPoint { get; }
    public Point EndPoint { get; }
    public Point MidPoint { get; }

    public IEnumerable<Point> StartEndPoints => Iterate.Over(StartPoint, EndPoint);

    ICurve IBoundedCurve.CreateTransformedCopy(Matrix transfMatrix) 
        =>  Line.CreateTransformedCopy(transfMatrix);

    public ICollection<ICurveEvaluation> GetPolyline(PolylineOptions options)
    {
        throw new NotImplementedException();
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