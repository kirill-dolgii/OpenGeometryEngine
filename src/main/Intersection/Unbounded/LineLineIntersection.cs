using System;
using System.Collections.Generic;

namespace OpenGeometryEngine.Intersection.Unbounded;

internal static class LineLineIntersection
{
    public static ICollection<IntersectionPoint<ICurveEvaluation, ICurveEvaluation>>
        LineIntersectLine(Line first, Line second)
    {
        if (first == null) throw new ArgumentNullException("first was null");
        if (second == null) throw new ArgumentNullException("second was null");
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
                var param = Vector.Dot(Vector.Cross(p1p2, second.Direction), cross) / Math.Pow(cross.Magnitude, 2); //Vector3.Dot(Vector3.Cross(p1p2, direction2), cross) / cross.LengthSquared();
                var eval1 = first.Evaluate(param);
                var eval2 = second.ProjectPoint(eval1.Point);
                ret.Add(new(eval1, eval2));
            }
        }
        return ret;
    }

}