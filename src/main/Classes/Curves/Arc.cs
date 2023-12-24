using System;
using System.Collections.Generic;
using System.Linq;
//using System.Threading;
//using System.Xml.Serialization;
//using OpenGeometryEngine.Exceptions;
//using OpenGeometryEngine.Intersection.Unbounded;
//using OpenGeometryEngine.Structures;

//namespace OpenGeometryEngine;

//public sealed class Arc : CurveSegment
//{
//    private Arc(Interval interval, Curve geometry, Point startPoint, Point endPoint, double length)
//        : base(interval, geometry, startPoint, endPoint, length)
//    {
//    }

//    public static Arc Create(Circle circle, Interval interval)
//    {
//        if (circle == null) throw new ArgumentNullException("circle was null");
//        if (Accuracy.EqualAngles(interval.Start, interval.End)) throw new SingularIntervalException();
//        var startPnt = circle.Evaluate(interval.Start).Point;
//        var endPnt = circle.Evaluate(interval.End).Point;
//        var length = circle.Radius * interval.Span / (2 * Math.PI);
//        var arc = new Arc(interval, circle, startPnt, endPnt, length);
//        return arc;
//    }

//    public override TCurve GetGeometry<TCurve>()
//    {
//        var geomType = Geometry.GetType();
//        if (typeof(TCurve) != geomType) return null;
//        return (TCurve) Geometry;
//    }

//    public override Box GetBoundingBox()
//    {
//        var circle = GetGeometry<Circle>();
//        var transform = Matrix.CreateMapping(circle.Frame);
//        var inverseTransform = transform.Inverse();

//        var angle = circle.Frame.DirX.SignedAngle(Frame.World.DirX, 
//                                                  Vector.Cross(circle.Frame.DirX, Frame.World.DirX));

//        var centeredCircle = circle.CreateTransformedCopy(inverseTransform);
//        var cornerEvals = new[] { 0, Math.PI / 2, Math.PI, 3 * Math.PI / 2 };
//        var cornerPoints = cornerEvals.Where(param => Accuracy.WithinAngleInterval(Interval, param))
//                                      .Select(param => centeredCircle.Evaluate(param + angle).Point);
//        var boxPoints = new List<Point> { centeredCircle.Evaluate(Interval.Start + angle).Point, 
//                                          centeredCircle.Evaluate(Interval.End + angle).Point };
//        boxPoints.AddRange(cornerPoints);
//        var mappedBoxPoints = boxPoints.Select(p => transform * p).ToList();
//        return Box.Create(mappedBoxPoints);
//    }

//    public override ICollection<IntersectionPoint<CurveEvaluation, CurveEvaluation>> IntersectCurve(CurveSegment otherSegment)
//    {
//        if (otherSegment == null) throw new ArgumentNullException("otherSegment was null");
//        switch (otherSegment)
//        {
//            case (LineSegment lineSegment):
//            {
//                var inters = LineCircleIntersection.LineIntersectCircle((Line)lineSegment.Geometry, (Circle)Geometry);
//                    return inters.Where(ip => Accuracy.WithinLengthInterval(lineSegment.Interval, ip.FirstEvaluation.Param) &&
//                                              Accuracy.WithinAngleInterval(Interval, ip.SecondEvaluation.Param))
//                                 .ToList();
//            };
//        }
//        throw new NotImplementedException();
//    }

//    public override CurveSegment CreateTransformedCopy(Matrix matrix)
//    {
//        var newCircle = ((Circle)Geometry).CreateTransformedCopy(matrix);
//        var newArc = Arc.Create(newCircle, Interval);
//        return newArc;
//    }

//    public override bool IsCoincident(CurveSegment otherSegment)
//    {
//        if (otherSegment == null) throw new ArgumentNullException("other segment was null");
//        var otherArc = otherSegment as Arc;
//        if (otherArc == null) return false;
//        var circle = (Circle)Geometry;
//        var otherCircle = (Circle)otherSegment.Geometry;
//        return circle.Center == otherCircle.Center &&
//               Accuracy.EqualLengths(circle.Radius, otherCircle.Radius) &&
//               StartPoint == otherArc.StartPoint &&
//               EndPoint == otherArc.EndPoint;
//    }
//}

using OpenGeometryEngine;
using OpenGeometryEngine.Classes.Curves;
using OpenGeometryEngine.Interfaces.Curves;
using OpenGeometryEngine.Structures;

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
        var circle = GetGeometry<Circle>();
        var transform = Matrix.CreateMapping(circle.Frame);
        var inverseTransform = transform.Inverse();

        var angle = Vector.SignedAngle(circle.Frame.DirX, Frame.World.DirX,
                                                  Vector.Cross(circle.Frame.DirX, Frame.World.DirX));

        var centeredCircle = circle.CreateTransformedCopy(inverseTransform);
        var cornerEvals = new[] { 0, System.Math.PI / 2, System.Math.PI, 3 * System.Math.PI / 2 };
        var cornerPoints = cornerEvals.Where(param => Accuracy.WithinAngleInterval(Interval, param))
                                      .Select(param => centeredCircle.Evaluate(param + angle).Point);
        var boxPoints = new List<Point> { centeredCircle.Evaluate(Interval.Start + angle).Point,
                                                  centeredCircle.Evaluate(Interval.End + angle).Point };
        boxPoints.AddRange(cornerPoints);
        var mappedBoxPoints = boxPoints.Select(p => transform * p).ToList();
        return Box.Create(mappedBoxPoints);
    }

    public IGeometry Geometry { get; }

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
        return Evaluate(param);
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