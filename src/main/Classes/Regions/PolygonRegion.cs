using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OpenGeometryEngine.Exceptions;

namespace OpenGeometryEngine;

public class PolygonRegion : IFlatRegion
{
    private readonly LineSegment[] _lines;
    private readonly Point[] _points;

    public PolygonRegion(Point[] points, Plane plane)
    {
        Argument.IsNotNull(nameof(points), points);
        if (points.Length < 2) throw new ArgumentException("Can't create a region from less than 3 points");
        _lines = new LineSegment[points.Length];

        double length = 0;

        for (int i = 0; i < points.Length; i++)
        {
            if (!plane.ContainsPoint(points[i])) throw new PointOffRegionPlaneException();
            int j = (i + 1) % points.Length;
            _lines[i] = new LineSegment(points[i], points[j]);
            length += _lines[i].Length;
        }
        
        if (SelfIntersects(_lines)) throw new SelfIntersectingRegionException();

        Plane = plane;
        Length = length;
        _points = points;
        Area = ComputeArea(points, plane);
        IsConvex = CheckIfConvex();
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
        return Math.Abs(area) / 2;
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

    private bool CheckIfConvex()
    {
        if (_points.Length == 3) return true;
        var first = Vector.Dot(Vector.Cross(_points[0] - _points[1], _points[2] - _points[1]), Plane.Frame.DirZ) > 0;
        for (int i = 1; i < _points.Length - 1; i++)
        {
            var j = (i + 2) % _points.Length;
            var current = Vector.Dot(Vector.Cross(_points[i] - _points[i + 1],
                _points[j] - _points[i + 1]), Plane.Frame.DirZ) > 0;
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
            var vec1 = _points[i] - point;
            var vec2 = _points[i + 1] - point;
            var cross = Vector.Cross(vec1, vec2);
            var dir = Vector.Dot(Plane.Frame.DirZ, cross) > 0 ? Plane.Frame.DirZ : Plane.Frame.DirZ.Reverse();
            if (!Vector.TryGetAngleClockWiseInDir(vec1, vec2, dir, out double angle))
                throw new Exception();
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
        foreach (var l1 in _lines)
        {
            foreach (var l2 in other._lines)
            {
                if (l1.IntersectCurve(l2).Any()) return false;
            }
        }
        return ContainsPoint(other._points[0]);
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

    public double Length { get; }

    public double Area { get; }

    public Plane Plane { get; }

    public ICollection<IFlatRegion> InnerRegions => throw new NotImplementedException();

    public bool IsConvex { get; }

    public Point[] Points => _points;

    public int WindingNumber(Point point)
    {
        throw new System.NotImplementedException();
    }

    public IFlatRegion Offset(double offset)
    {
        throw new System.NotImplementedException();
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

