using OpenGeometryEngine.Classes;
using OpenGeometryEngine.Collections;
using OpenGeometryEngine.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenGeometryEngine.Fillet;

public static class LinesegmentFillet
{
    public static ICollection<ITrimmedCurve> Fillet(LineSegment first,
                                                    LineSegment second,
                                                    double radius)
    {
        Argument.IsNotNull(nameof(first), first);
        Argument.IsNotNull(nameof(second), second);

        var commonPoints = first.StartEndPoints.Intersect(second.StartEndPoints).ToList();

        if (!commonPoints.Any()) throw new NoCommonPointException(nameof(first), nameof(second));
        if (commonPoints.Count() > 1) throw new Exception("Lines are equal");

        var commonPoint = commonPoints.Single();

        var firstPoint = first.StartEndPoints.Except(commonPoints).ToList().Single();
        var secondPoint = second.StartEndPoints.Except(commonPoints).ToList().Single();

        var firstTangent = (firstPoint - commonPoint).Unit;
        var secondTangent = (secondPoint - commonPoint).Unit;

        var circleCenterVec = (firstTangent + secondTangent) / 2;

        var cross = Vector.Cross(firstTangent, secondTangent).Unit;

        var angle = Vector.Angle(firstTangent, secondTangent);
        var cos = Math.Cos(angle / 2);
        var shift = radius / cos;
        var circleCenter = (circleCenterVec * shift).ToPoint();

        var arc = new Arc(circleCenter,
            first.ProjectPoint(circleCenter).Point,
            second.ProjectPoint(circleCenter).Point, cross);

        var ret = Iterate.Over<ITrimmedCurve>(new LineSegment(firstPoint, first.Line.ProjectPoint(circleCenter).Point),
                            arc,
                            new LineSegment(second.Line.ProjectPoint(circleCenter).Point, secondPoint));
        return ret.ToList();
    }
}