using OpenGeometryEngine.Collections;
using OpenGeometryEngine.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenGeometryEngine.Fillet;

public static class LinesegmentFillet
{
    public static ICollection<IBoundedCurve> Fillet(LineSegment first,
                                                    LineSegment second,
                                                    double radius)
    {
        Argument.IsNotNull(nameof(first), first);
        Argument.IsNotNull(nameof(second), second);

        var pntComparer = new PointEqualityComparer();

        var commonPoints = first.StartEndPoints.Intersect(second.StartEndPoints, pntComparer).ToList();

        if (!commonPoints.Any()) throw new NoCommonPointException(nameof(first), nameof(second));
        if (commonPoints.Count() > 1) throw new Exception("Lines are equal");

        var commonPoint = commonPoints.Single();

        var firstPoint = first.StartEndPoints.Except(commonPoints, pntComparer).ToList().Single();
        var secondPoint = second.StartEndPoints.Except(commonPoints, pntComparer).ToList().Single();

        var firstTangent = (firstPoint - commonPoint).Unit;
        var secondTangent = (secondPoint - commonPoint).Unit;

        var circleCenterVec = (firstTangent + secondTangent) / 2;

        var cross = Vector.Cross(firstTangent, secondTangent).Unit;

        var angle = Math.PI - Vector.Angle(firstTangent, secondTangent);
        var cos = Math.Cos(angle / 2);
        var shift = radius / cos;
        var circleCenter = commonPoint + circleCenterVec * shift;

        var arc = new Arc(circleCenter,
            first.ProjectPoint(circleCenter).Point,
            second.ProjectPoint(circleCenter).Point, cross.Reverse());

        var ret = Iterate.Over<IBoundedCurve>(new LineSegment(firstPoint, first.Line.ProjectPoint(circleCenter).Point),
                            arc,
                            new LineSegment(second.Line.ProjectPoint(circleCenter).Point, secondPoint));
        return ret.ToList();
    }
}