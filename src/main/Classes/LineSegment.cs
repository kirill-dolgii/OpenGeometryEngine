using System;
using System.Collections.Generic;
using System.Linq;
using OpenGeometryEngine.Intersection.Unbounded;

namespace OpenGeometryEngine;

public sealed class LineSegment : CurveSegment
{
    public LineSegment(Interval interval, Curve geometry, 
                       Point startPoint, Point endPoint, 
                       double length) : base(interval, geometry, startPoint, endPoint, length)
    { }

    public static LineSegment Create(Line line, Interval interval)
    {
        if (line == null) throw new ArgumentNullException($"Line specified {nameof(line)} was null.");
        var length = interval.Span;
        var start = line.Evaluate(interval.Start).Point;
        var end = line.Evaluate(interval.End).Point;
        var segment = new LineSegment(interval, line, start, end, length);
        return segment;
    }

    public override ICollection<IntersectionPoint<CurveEvaluation, CurveEvaluation>> IntersectCurve(CurveSegment otherSegment)
    {
        if (otherSegment == null) throw new ArgumentNullException(nameof(otherSegment));
        var myLine = (Line)Geometry;
        switch (otherSegment)
        {
            case LineSegment line:
                var otherLine = otherSegment.Geometry as Line;
                var intersections = LineLineIntersection.LineIntersectLine(myLine, otherLine);
                return intersections.Where(ip => Accuracy.WithinLengthInterval(Interval, ip.FirstEvaluation.Param) &&
                                                 Accuracy.WithinLengthInterval(otherSegment.Interval, ip.SecondEvaluation.Param))
                                    .ToList();
        }
        throw new NotImplementedException();
    }

    public override bool IsCoincident(CurveSegment otherSegment)
    {
        if (otherSegment == null) throw new ArgumentNullException($"{nameof(otherSegment)} was null");
        var otherLineSegment = otherSegment as LineSegment;
        if (otherLineSegment == null) return false;
        var otherLine = (Line)otherSegment.Geometry;
        var line = (Line)Geometry;
        return line.Direction.IsParallel(otherLine.Direction) &&
               StartPoint == otherLineSegment.StartPoint &&
               EndPoint == otherLineSegment.EndPoint;
    }
}
