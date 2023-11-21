using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;
using OpenGeometryEngine.Exceptions;
using OpenGeometryEngine.Intersection.Unbounded;
using OpenGeometryEngine.Structures;

namespace OpenGeometryEngine;

public sealed class Arc : CurveSegment
{
    private Arc(Interval interval, Curve geometry, Point startPoint, Point endPoint, double length)
        : base(interval, geometry, startPoint, endPoint, length)
    {
    }

    public static Arc Create(Circle circle, Interval interval)
    {
        if (circle == null) throw new ArgumentNullException("circle was null");
        if (Accuracy.EqualAngles(interval.Start, interval.End)) throw new SingularIntervalException();
        var startPnt = circle.Evaluate(interval.Start).Point;
        var endPnt = circle.Evaluate(interval.End).Point;
        var length = circle.Radius * interval.Span / (2 * Math.PI);
        var arc = new Arc(interval, circle, startPnt, endPnt, length);
        return arc;
    }

    public override TCurve GetGeometry<TCurve>()
    {
        var geomType = Geometry.GetType();
        if (typeof(TCurve) != geomType) return null;
        return (TCurve) Geometry;
    }

    public override Box GetBoundingBox()
    {
        var circle = GetGeometry<Circle>();
        var transform = Matrix.CreateMapping(circle.Frame);
        var inverseTransform = transform.Inverse();

        var angle = circle.Frame.DirX.SignedAngle(Frame.World.DirX, 
                                                  Vector.Cross(circle.Frame.DirX, Frame.World.DirX));

        var centeredCircle = circle.CreateTransformedCopy(inverseTransform);
        var cornerEvals = new[] { 0, Math.PI / 2, Math.PI, 3 * Math.PI / 2 };
        var cornerPoints = cornerEvals.Where(param => Accuracy.WithinAngleInterval(Interval, param))
                                      .Select(param => centeredCircle.Evaluate(param + angle).Point);
        var boxPoints = new List<Point> { centeredCircle.Evaluate(Interval.Start + angle).Point, 
                                          centeredCircle.Evaluate(Interval.End + angle).Point };
        boxPoints.AddRange(cornerPoints);
        var mappedBoxPoints = boxPoints.Select(p => transform * p).ToList();
        return Box.Create(mappedBoxPoints);
    }

    public override ICollection<IntersectionPoint<CurveEvaluation, CurveEvaluation>> IntersectCurve(CurveSegment otherSegment)
    {
        if (otherSegment == null) throw new ArgumentNullException("otherSegment was null");
        switch (otherSegment)
        {
            case (LineSegment lineSegment):
            {
                var inters = LineCircleIntersection.LineIntersectCircle((Line)lineSegment.Geometry, (Circle)Geometry);
                    return inters.Where(ip => Accuracy.WithinLengthInterval(lineSegment.Interval, ip.FirstEvaluation.Param) &&
                                              Accuracy.WithinAngleInterval(Interval, ip.SecondEvaluation.Param))
                                 .ToList();
            };
        }
        throw new NotImplementedException();
    }

    public override CurveSegment CreateTransformedCopy(Matrix matrix)
    {
        var newCircle = ((Circle)Geometry).CreateTransformedCopy(matrix);
        var newArc = Arc.Create(newCircle, Interval);
        return newArc;
    }

    public override bool IsCoincident(CurveSegment otherSegment)
    {
        if (otherSegment == null) throw new ArgumentNullException("other segment was null");
        var otherArc = otherSegment as Arc;
        if (otherArc == null) return false;
        var circle = (Circle)Geometry;
        var otherCircle = (Circle)otherSegment.Geometry;
        return circle.Center == otherCircle.Center &&
               Accuracy.EqualLengths(circle.Radius, otherCircle.Radius) &&
               StartPoint == otherArc.StartPoint &&
               EndPoint == otherArc.EndPoint;
    }
}
