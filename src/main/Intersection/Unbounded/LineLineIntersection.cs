using System;
using System.Collections.Generic;

namespace OpenGeometryEngine.Intersection.Unbounded;

internal static class LineLineIntersection
{
    public static ICollection<IntersectionPoint<ICurveEvaluation, ICurveEvaluation>>
        LineIntersectLine(Line first, Line second)
    {
        Argument.IsNotNull(nameof(first), first);
        Argument.IsNotNull(nameof(second), second);

        var ret = new List<IntersectionPoint<ICurveEvaluation, ICurveEvaluation>>();

        if (Line.AreCoincident(first, second, Accuracy.LinearTolerance))
        {
            ret.Add(new(first.Evaluate(0), second.Evaluate(0)));
        }
        else
        {
            var cross = Vector.Cross(first.Direction, second.Direction);
            if (!Accuracy.LengthIsZero(cross.Magnitude))
            {
                var p1p2 = second.Origin - first.Origin;
                var param = Vector.Dot(Vector.Cross(p1p2, second.Direction), cross) / Math.Pow(cross.Magnitude, 2);
                var eval1 = first.Evaluate(param);
                var eval2 = second.ProjectPoint(eval1.Point);
                ret.Add(new(eval1, eval2));
            }
        }
        return ret;
    }
}