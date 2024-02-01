using static OpenGeometryEngine.Intersection.Unbounded.LinePlaneIntersection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenGeometryEngine.Intersection.Unbounded;

internal static class LineCircleIntersection
{
    public static ICollection<IntersectionPoint<ICurveEvaluation, ICurveEvaluation>> LineIntersectCircle(Line line, Circle circle)
    {
        Argument.IsNotNull(nameof(line), line);
        Argument.IsNotNull(nameof(circle), circle);

        var ret = new List<IntersectionPoint<ICurveEvaluation, ICurveEvaluation>>();

        if (PlaneContainsLine(circle.Plane, line))
        {
            var proj = line.ProjectPoint(circle.Frame.Origin);
            var distance = (proj.Point - circle.Frame.Origin).Magnitude;
            if (Accuracy.LengthIsGreaterThan(circle.Radius, distance))
            {
                // Pythagoras triangle
                var offset = Math.Sqrt(circle.Radius * circle.Radius - distance * distance); 
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