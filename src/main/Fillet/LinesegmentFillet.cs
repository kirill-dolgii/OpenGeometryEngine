using OpenGeometryEngine.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenGeometryEngine.Fillet;

public static class LineSegmentFillet
{
    public static FilletResult Fillet(LineSegment first,
                                      LineSegment second,
                                      double radius)
    {
        Argument.IsNotNull(nameof(first), first);
        Argument.IsNotNull(nameof(second), second);

        var check = Check(first, second);

        if (Accuracy.LengthIsGreaterThan(radius, check.MaxFilletRadius))
            return new FilletResult(first, second, FilletType.Failed, null, null);

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

        var ret = new List<IBoundedCurve>();

        var newFirst = pntComparer.Equals(firstPoint, first.Line.ProjectPoint(circleCenter).Point)
            ? null
            : new LineSegment(firstPoint, first.Line.ProjectPoint(circleCenter).Point);

        var newSecond = pntComparer.Equals(second.Line.ProjectPoint(circleCenter).Point, secondPoint)
            ? null
            : new LineSegment(second.Line.ProjectPoint(circleCenter).Point, secondPoint);

        if (newFirst != null) ret.Add(newFirst);
        ret.Add(arc);
        if (newSecond != null) ret.Add(newSecond);

        var type = ret.Count != 3 ? FilletType.Degenerated : FilletType.Regular;
        return new FilletResult(newFirst, newSecond, type, arc, ret);
    }

    public static FilletCheck Check(LineSegment first, LineSegment second)
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

        var angle = Vector.Angle(firstTangent, secondTangent);

        var maxRadius = Iterate.Over(first.Length, second.Length).Min(l => l * Math.Tan(angle / 2));
        return new FilletCheck(first, second, maxRadius);
    }
}