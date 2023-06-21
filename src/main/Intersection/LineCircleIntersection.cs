using static OpenGeometryEngine.Extensions.VectorExtensions;
using static OpenGeometryEngine.Intersection.LinePlaneIntersection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenGeometryEngine.Intersection;

internal static class LineCircleIntersection
{
    public static ICollection<IntersectionPoint<CurveEvaluation, CurveEvaluation>> LineIntersectCircle(Line line, Circle circle)
    {
        var ret = new List<IntersectionPoint<CurveEvaluation, CurveEvaluation>>();
        if (PlaneContainsLine(circle.Plane, line))
        {
            var lineToCircleOrigin = circle.Center - line.Origin;
            var proj = line.ProjectPoint(lineToCircleOrigin.ToPoint());
            var distance = (proj.Point - circle.Center).Magnitude;
            if (circle.Radius - distance > Constants.Tolerance)
            {
                var offset = (float)Math.Sqrt(circle.Radius * circle.Radius - distance * distance); // Pythagoras triangle
                var p1 = proj.Point + line.Direction * offset;
                var p2 = proj.Point - line.Direction * offset;
                ret.Add(new(new CurveEvaluation(offset, p1), circle.ProjectPoint(p1))); 
                ret.Add(new(new CurveEvaluation(-offset, p2), circle.ProjectPoint(p2)));
            }
            else if (Math.Abs(circle.Radius - distance) <= Constants.Tolerance) 
                ret.Add(new(proj, circle.ProjectPoint(proj.Point)));
        }
        else if (LineIntersectsPlane(line, circle.Plane))
        {
            var intersection = LineIntersectPlane(line, circle.Plane).Single();
            var intersPoint = intersection.FirstEvaluation.Point;
            var circleEval = circle.ProjectPoint(intersPoint);
            if ((circleEval.Point - intersPoint).Magnitude <= Constants.Tolerance)
                ret.Add(new(intersection.FirstEvaluation, circleEval));
        }
        return ret;
    }
}