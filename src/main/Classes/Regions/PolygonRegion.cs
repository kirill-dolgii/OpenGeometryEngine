using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OpenGeometryEngine.Exceptions;
using OpenGeometryEngine.Collections;

namespace OpenGeometryEngine;

public class PolygonRegion : IFlatRegion
{
    private readonly LineSegment[] _lines;
    private readonly Point[] _points;

    internal PolygonRegion(Loop loop, Plane plane, PolylineOptions? options)
    {
        if (options == null) options = PolylineOptions.Default;
        _points = loop.SelectMany(fin =>
        {
            var points = (fin.Curve switch
            {
                LineSegment ls => Iterate.Over(fin.Y._point),
                Arc arc => arc.GetPolyline(options.Value).Select(eval => eval.Point),
                _ => throw new NotImplementedException()
            }).ToList();
            if (!PointEqualityComparer.Default.Equals(fin.X._point, fin.Curve.StartPoint)) points.Reverse();
            return points;
        }).Distinct(PointEqualityComparer.Default).ToArray();

        _lines = new LineSegment[_points.Length];
        for (int i = 0; i < _points.Length; i++)
        {
            if (!plane.ContainsPoint(_points[i])) throw new PointOffRegionPlaneException();
            int j = (i + 1) % _points.Length;
            _lines[i] = new LineSegment(_points[i], _points[j]);
            Perimeter += _lines[i].Length;
        }

        if (SelfIntersects(_lines)) throw new SelfIntersectingRegionException();

        Plane = plane;
        Area = ComputeArea(_points, Plane);
        IsConvex = PolygonIsConvex(_points, plane);
    }

    public PolygonRegion(Point[] points, Plane plane)
    {
        Argument.IsNotNull(nameof(points), points);
        if (points.Length < 2) throw new ArgumentException("Can't create a region from less than 3 points");
        _lines = new LineSegment[points.Length];

        for (int i = 0; i < points.Length; i++)
        {
            if (!plane.ContainsPoint(points[i])) throw new PointOffRegionPlaneException();
            int j = (i + 1) % points.Length;
            _lines[i] = new LineSegment(points[i], points[j]);
			Perimeter += _lines[i].Length;
        }
        
        if (SelfIntersects(_lines)) throw new SelfIntersectingRegionException();

        Plane = plane;
        _points = points;
        Area = ComputeArea(points, plane);
        IsConvex = PolygonIsConvex(points, plane);
    }

    internal static double ComputeArea(Point[] points, Plane plane)
    {
        var area = 0.0;
        for (int i = 0; i < points.Length; i++)
        {
            int j = (i + 1) % points.Length;

            var cross = Vector.Cross(points[i] - plane.Frame.Origin, 
                                     points[j] - plane.Frame.Origin);
            area += cross.Magnitude * Math.Sign(Vector.Dot(cross, plane.Frame.DirZ));
        }
        if (points.Length > 3) area /= 2;
        return Math.Abs(area);
    }
    
    // Naive self-intersection test
    internal static bool SelfIntersects(LineSegment[] lines)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            for (int j = 0; j < lines.Length; j++)
            {
                if (i == j) continue;
                if (lines[i].IntersectCurve(lines[j])
                    .Any(ip => !lines[i].Interval.OnBounds(ip.FirstEvaluation.Param, Accuracy.DefaultLinearTolerance) &&
                               !lines[j].Interval.OnBounds(ip.SecondEvaluation.Param, Accuracy.DefaultLinearTolerance))) return true;
            }
        }
        return false;
    }

    private static bool PolygonIsConvex(IList<Point> points, Plane plane)
    {
        if (points.Count == 3) return true;
        var first = Vector.Dot(Vector.Cross(points[0] - points[1], points[2] - points[1]), plane.Frame.DirZ) > 0;
        for (int i = 1; i < points.Count - 1; i++)
        {
            var j = (i + 2) % points.Count;
            var current = Vector.Dot(Vector.Cross(points[i] - points[i + 1],
                points[j] - points[i + 1]), plane.Frame.DirZ) > 0;
            if (current != first) return false;
            first = current;
        }
        return true;
    }

    public bool ContainsPoint(Point point)
    {
        if (!Plane.ContainsPoint(point)) return false;
        if (IsOnBound(point)) return true;
        var sum = 0.0d;
        for (int i = 0; i < _points.Length - 1; i++)
        {
			//var vec1 = _points[i] - point;
			//var vec2 = _points[i + 1] - point;
			//var cross = Vector.Cross(vec1, vec2);
			//var dir = Vector.Dot(Plane.Frame.DirZ, cross) > 0 ? Plane.Frame.DirZ : Plane.Frame.DirZ.Reverse();
			//if (!Vector.TryGetAngleClockWiseInDir(vec1, vec2, dir, out double angle))
			//    throw new Exception();
			//sum += angle;
			if (!Vector.TryGetSignedAngleInDir(_points[i] - point, _points[i + 1] - point, 
                                               Plane.Frame.DirZ, out double angle))
                throw new Exception("failed"); //TODO: typed exception
            sum += angle;
		}
        return Math.Abs(sum) > Math.PI;
    }

    public bool IsOnBound(Point point)
    {
        if (!Plane.ContainsPoint(point)) return false;
        foreach (var lineSegment in _lines)
        {
            if (lineSegment.ContainsPoint(point)) return true;
        }
        return false;
    }

    private bool Contains(PolygonRegion other)
    {
        //foreach (var l1 in _lines)
        //{
        //    foreach (var l2 in other._lines)
        //    {
        //        if (l1.IntersectCurve(l2).Any()) return false;
        //    }
        //}
        foreach (var point in other._points)
        {
            if (!ContainsPoint(point)) return false;
        }
        return true;
    }

    public IEnumerator<IBoundedCurve> GetEnumerator()
    {
        foreach (var lineSegment in _lines)
        {
            yield return lineSegment;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public double Perimeter { get; }

    public double Area { get; }

    public Plane Plane { get; }

    public ICollection<IFlatRegion> InnerRegions => throw new NotImplementedException();

    public bool IsConvex { get; }

    public Point[] Points => _points;

    public ICollection<IBoundedCurve> Boundary => _lines;

    public int WindingNumber(Point point)
    {
        throw new System.NotImplementedException();
    }

    public IFlatRegion Offset(double offset)
    {
        throw new NotImplementedException();
    }

    public bool Contains(IFlatRegion other)
    {
        Argument.IsNotNull(nameof(other), other);
        switch (other)
        {
            case PolygonRegion polygon:
            {
                return Contains(polygon);
            }
            default: throw new NotImplementedException();
        }
    }

    public bool Intersects(IFlatRegion other)
    {
        throw new System.NotImplementedException();
    }
}

