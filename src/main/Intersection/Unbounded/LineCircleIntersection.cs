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
                
                Vector.TryGetAngleClockWiseInDir(circle.Frame.DirX, (p1 - circle.Frame.Origin).Unit, circle.Frame.DirZ, out double param1);
                Vector.TryGetAngleClockWiseInDir(circle.Frame.DirX, (p2 - circle.Frame.Origin).Unit, circle.Frame.DirZ, out double param2);

                if (param1 < 0.0) param1 = 2 * Math.PI + param1;
                if (param2 < 0.0) param2 = 2 * Math.PI + param2;

                ret.Add(new(line.ProjectPoint(p1), circle.Evaluate(param1)));
                ret.Add(new(line.ProjectPoint(p2), circle.Evaluate(param2)));
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