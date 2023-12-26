using System.Collections.Generic;
using System.Linq;
using OpenGeometryEngine.Exceptions;

namespace OpenGeometryEngine;

public class Arc : ITrimmedCurve
{
    private Arc() {}
        
    public Arc(Frame frame, double radius, Interval bounds)
    {
        Circle = new Circle(frame, radius);
        Interval = bounds;
        Length = radius * bounds.Span;
        StartPoint = EvaluateAtProportion(bounds.Start).Point;
        MidPoint = Evaluate(bounds.Start + bounds.Span / 2).Point;
        EndPoint = Evaluate(bounds.End).Point;
    }

    public Arc(Arc other) : this(other.Circle.Frame, other.Circle.Radius, other.Interval) { }

    public Box GetBoundingBox()
    {
        var boxPoints = new List<Point>() { StartPoint, EndPoint };
        foreach (var worldDirection in UnitVec.WorldDirections)
        {
            var evals = Circle.GetExtremePointInDir(Circle, worldDirection);
            if (!evals.Any()) continue;
            if (Interval.Contains(evals.First().Param, Accuracy.AngularTolerance)) 
                boxPoints.Add(evals.First().Point);
            if (Interval.Contains(evals.ElementAt(1).Param, Accuracy.AngularTolerance))
                boxPoints.Add(evals.ElementAt(1).Point);
        }
        return Box.Create(boxPoints);
    }

    public IGeometry Geometry => Circle;

    public TGeometry GetGeometry<TGeometry>() where TGeometry : class, IGeometry => Circle as TGeometry;

    public bool IsGeometry<TGeometry>() where TGeometry : class, IGeometry => Circle is TGeometry;

    public bool ContainsPoint(Point point)
    {
        var param = Circle.ProjectPoint(point).Param;
        return Interval.Contains(param, Accuracy.AngularTolerance);
    }

    public ICurveEvaluation ProjectPoint(Point point)
    {
        var param = Circle.ProjectPoint(point).Param;
        return new CircleEvaluation(Circle, Interval.Project(param, Accuracy.AngularTolerance));
    }

    public ICurveEvaluation Evaluate(double param) => new CircleEvaluation(Circle, param);

    public ICurveEvaluation EvaluateAtProportion(double param)
    {
        if (Accuracy.CompareWithTolerance(param, 0.0, Accuracy.DefaultDoubleTolerance) == -1 || 
            Accuracy.CompareWithTolerance(param, 1.0, Accuracy.DefaultDoubleTolerance) == 1)
            throw new ProportionOutsideBoundsException(nameof(param));
        return Evaluate(Interval.Start + param * Interval.Span);
    }

    public Point StartPoint { get; }
    public Point EndPoint { get; }
    public Point MidPoint { get; }

    public Arc CreateTransformedCopy(Matrix transfMatrix)
    {
        if ((object)transfMatrix == (object)Matrix.Identity) return new Arc(this);
        return new Arc(transfMatrix * Circle.Frame, Circle.Radius, Interval);
    }

    ICurve ITrimmedCurve.CreateTransformedCopy(Matrix transfMatrix) 
        => CreateTransformedCopy(transfMatrix).Curve;

    public Interval Interval { get; }
    public double Length { get; }

    public Parametrization Parametrization => Curve.Parametrization;

    public Circle Circle { get; }

    public ICurve Curve => Circle;

    Point ISpatial.ProjectPoint(Point point) => ProjectPoint(point).Point;
}