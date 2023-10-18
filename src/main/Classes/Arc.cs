﻿using System;
using System.Collections.Generic;
using OpenGeometryEngine.Exceptions;

namespace OpenGeometryEngine.Classes;

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

    public override ICollection<IntersectionPoint<CurveEvaluation, CurveEvaluation>> IntersectCurve(CurveSegment otherSegment)
    {
        throw new NotImplementedException();
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