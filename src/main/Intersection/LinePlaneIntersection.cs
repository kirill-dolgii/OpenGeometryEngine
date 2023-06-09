using System;
using System.Collections.Generic;

namespace OpenGeometryEngine.Intersection;

internal static class LinePlaneIntersection
{
    public static bool PlaneContainsLine(Plane plane, Line line)
    {
        if (plane == null) throw new ArgumentNullException(nameof(plane));
        if (line == null) throw new ArgumentNullException(nameof(line));
        if (!plane.ContainsPoint(line.Origin)) return false;
        var dot = Vector.Dot(line.Direction, plane.Frame.DirZ);
        return Math.Abs(dot) <= Constants.Tolerance;
    }

    public static bool LineIntersectsPlane(Line line, Plane plane)
    {
        if (plane == null) throw new ArgumentNullException(nameof(plane));
        if (line == null) throw new ArgumentNullException(nameof(line));
        return Math.Abs(Vector.Dot(line.Direction, plane.Frame.DirZ)) >= Constants.Tolerance;
    }

    public static ICollection<IntersectionPoint<CurveEvaluation, SurfaceEvaluation>> LineIntersectPlane(
        Line line, Plane plane)
    {
        var ret = new List<IntersectionPoint<CurveEvaluation, SurfaceEvaluation>>();
        if (!PlaneContainsLine(plane, line) && LineIntersectsPlane(line, plane))
        {
            var lAb = -line.Direction;
            var planeToLineOrigin = line.Origin - plane.Frame.Origin;
            var lineDirPlaneNormalDot = Vector.Dot(lAb, plane.Frame.DirZ);

            var t = Vector.Dot(plane.Frame.DirZ, planeToLineOrigin) / lineDirPlaneNormalDot;
            var u = Vector.Dot(Vector.Cross(plane.Frame.DirY, lAb), planeToLineOrigin) / lineDirPlaneNormalDot;
            var v = Vector.Dot(Vector.Cross(lAb, plane.Frame.DirX), planeToLineOrigin) / lineDirPlaneNormalDot;
            var intersPoint = line.Evaluate(t).Point;
            ret.Add(new(new CurveEvaluation(t, intersPoint), new SurfaceEvaluation(new PointUV(u, v), intersPoint)));
        }
        return ret;
    }
}
