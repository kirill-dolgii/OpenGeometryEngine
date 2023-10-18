using static OpenGeometryEngine.Intersection.Unbounded.LinePlaneIntersection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenGeometryEngine.Intersection.Unbounded;

internal static class LineCircleIntersection
{
    public static ICollection<IntersectionPoint<CurveEvaluation, CurveEvaluation>> LineIntersectCircle(Line line, Circle circle)
    {
        if (line == null) throw new ArgumentNullException("line is null");
        if (circle == null) throw new ArgumentException("circle is null");
        var ret = new List<IntersectionPoint<CurveEvaluation, CurveEvaluation>>();
        if (PlaneContainsLine(circle.Plane, line))
        {
            var proj = line.ProjectPoint(circle.Center);
            var distance = (proj.Point - circle.Center).Magnitude;
            if (Accuracy.CompareLength(circle.Radius, distance) == 1)
            {
                var offset = (float)Math.Sqrt(circle.Radius * circle.Radius - distance * distance); // Pythagoras triangle
                var p1 = proj.Point + line.Direction * offset;
                var p2 = proj.Point - line.Direction * offset;
                ret.Add(new(line.ProjectPoint(p1), circle.ProjectPoint(p1)));
                ret.Add(new(line.ProjectPoint(p2), circle.ProjectPoint(p2)));
            }
            else if (Accuracy.EqualLengths(circle.Radius, distance))
                ret.Add(new(proj, circle.ProjectPoint(proj.Point)));
        }
        else if (LineIntersectsPlane(line, circle.Plane))
        {
            var intersection = LineIntersectPlane(line, circle.Plane).Single();
            var intersPoint = intersection.FirstEvaluation.Point;
            var circleEval = circle.ProjectPoint(intersPoint);
            if (circleEval.Point == intersPoint)
                ret.Add(new(intersection.FirstEvaluation, circleEval));
        }
        return ret;
    }
}