using DataStructures.Graph;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.ObjectModel;
using OpenGeometryEngine.Misc.Solvers;

namespace OpenGeometryEngine.Regions;

public sealed class PolyLineRegion : IFlatRegion
{
    private readonly HashSet<IBoundedCurve> _curves;
    private readonly Point[] _points;
    public readonly PolygonRegion _polygon;
    private bool _isOuter;
    private ICollection<PolyLineRegion> _innerRegions;
    private Graph<Point, IBoundedCurve> _graph; //contains curves from outer and all inner regions
    private Loop _loop;

    /// <summary>
    /// Creates PolyLineRegion with the only outer boundary i.e. there are no InnerRegions.
    /// </summary>
    /// <param name="loop"></param>
    /// <param name="plane"></param>
    internal PolyLineRegion(Loop loop, Plane plane)
    {
        _isOuter = false;
        _graph = new Graph<Point, IBoundedCurve>(false, PointEqualityComparer.Default, EqualityComparer<IBoundedCurve>.Default);
        foreach (var edge in loop)
        {
            _graph.AddEdge(edge.X, edge.Y, edge.Curve);
        }
        Plane = plane;
        _loop = loop;
        _curves = new HashSet<IBoundedCurve>(_graph.Edges);
        _polygon = new PolygonRegion(loop, plane, PolylineOptions.Default);
        _points = _polygon.Points;
        _innerRegions = Array.Empty<PolyLineRegion>();
        Area = _polygon.Area;
        Perimeter = _curves.Sum(curve => curve.Length);
    }

    internal PolyLineRegion(Loop outerLoop, ICollection<Loop> innerLoops, Plane plane) : this(outerLoop, plane)
    {
        _innerRegions = innerLoops.Select(loop => new PolyLineRegion(loop, plane)).ToArray();
        Area -= _innerRegions.Sum(region => region.Area);
        Perimeter += _innerRegions.Sum(region => region.Perimeter);
    }

    private static ICollection<PolyLineRegion> CreateFromLoops(ICollection<Loop> loops, Plane plane)
    {
        var polygons = loops.ToDictionary(loop => new PolygonRegion(loop, plane, PolylineOptions.Default));

        var polygonsGrouped = polygons.ToDictionary(
            kv1 => kv1.Key,
            kv1 => polygons.Where(kv2 => kv1.Key != kv2.Key && kv1.Key.Contains(kv2.Key))
                           .Select(kv => kv.Key).ToArray());

        foreach (var kv in polygonsGrouped.ToList())
        {
            foreach (var inner in kv.Value)
            {
                polygonsGrouped.Remove(inner);
            }
        }

        var regions = polygonsGrouped.Select(kv =>
        {
            var region = kv.Key;
            return new PolyLineRegion(polygons[region], kv.Value.Select(polygon => polygons[polygon]).ToArray(), plane);
        }).ToArray();

        return regions;
    }

    public static ICollection<PolyLineRegion> CreateRegions(ICollection<IBoundedCurve> curves,
                                                             Plane plane)
    {
        Argument.IsNotNull(nameof(curves), curves);
        var solver = new PlanarMinCycleSolver(curves, plane);
        var loops = solver.Solve();
        //self-intersection test performed via PolygonRegions self-intersection test
        return CreateFromLoops(loops, plane);        
    }

    public ICollection<PolyLineRegion> Intersect(PolyLineRegion other)
    {
        Argument.IsNotNull(nameof(other), other);
        var loops = GetSplitLoops(other._curves.Concat(other.InnerRegions.SelectMany(region => region.Boundary)).ToArray());
        throw new NotImplementedException();
    }

    public PolyLineRegion Merge(PolyLineRegion other)
    {
        throw new NotImplementedException();
    }

    public ICollection<PolyLineRegion> Subtract(PolyLineRegion other)
    {
        throw new NotImplementedException();
    }

    internal ICollection<Loop> GetSplitLoops(ICollection<IBoundedCurve> splitCurves)
    {
        Argument.IsNotNull(nameof(splitCurves), splitCurves);
        var graph = new Graph<Point, IBoundedCurve>(_graph);
        var intersectionCurves = GetIntersectionCurves(splitCurves, out var map);
        foreach (var kv in map)
        {
            graph.RemoveEdge(kv.Key.StartPoint, kv.Key.EndPoint);
            foreach (var splitted in kv.Value)
            {
                graph.AddEdge(splitted.StartPoint, splitted.EndPoint, splitted);
            }
        }
        int i = 0;
        foreach (var curve in intersectionCurves)
        {
            if (i++ % 2 != 0) continue;
            graph.AddEdge(curve.StartPoint, curve.EndPoint, curve);
        }
        var solver = new PlanarMinCycleSolver(graph.Edges, Plane);
        var loops = solver.Solve();
        return loops;
    }

	internal ICollection<IBoundedCurve> GetIntersectionCurves(ICollection<IBoundedCurve> curves,
															  out Dictionary<IBoundedCurve, ICollection<IBoundedCurve>> splitted)
	{
		Argument.IsNotNull(nameof(curves), curves);

		var intersections = _curves.SelectMany(polygonCurve =>
				curves.SelectMany(curve => polygonCurve.IntersectCurve(curve)
					  .Select(ip => new { PolygonCurve = polygonCurve, SplitCurve = curve, Intersection = ip, Inner = false }))).ToArray();

		var innerIntersections = InnerRegions.SelectMany(region =>
			((PolyLineRegion)region)._curves.SelectMany(polygonCurve =>
				curves.SelectMany(curve => polygonCurve.IntersectCurve(curve)
					   .Select(ip => new { PolygonCurve = polygonCurve, SplitCurve = curve, Intersection = ip, Inner = true })))).ToArray();

		if (!intersections.Any() && !innerIntersections.Any())
		{
			splitted = new();
			return Array.Empty<IBoundedCurve>();
		}

		// check if suitable split curves intersections
		if (intersections.Length % 2 != 0 || innerIntersections.Length % 2 != 0)
		{
			throw new Exception("Region can't be splitted"); //TODO: typed exception
		}

		splitted = intersections.Concat(innerIntersections)
						.GroupBy(intersection => intersection.PolygonCurve)
						.ToDictionary(group => group.Key,
									  group => group.Key.Split(group.Select(inters => inters.Intersection.FirstEvaluation.Param).ToArray()));

		var splitSegments = intersections.Concat(innerIntersections).GroupBy(intersection => intersection.SplitCurve)
										 .SelectMany(group =>
										 {
                                             var splitted = group.Key.Split(group.Select(inters => inters.Intersection.SecondEvaluation.Param).ToArray());
											 return splitted.Where(curve => _polygon.ContainsPoint(curve.MidPoint));
										 }).ToArray();
		return splitSegments;
	}

    public ICollection<PolyLineRegion> Split(ICollection<IBoundedCurve> splitCurves)
    {
        Argument.IsNotNull(nameof(splitCurves), splitCurves);
        var loops = GetSplitLoops(splitCurves);
        return CreateFromLoops(loops, Plane);
    }

    public IEnumerator<IBoundedCurve> GetEnumerator()
    {
        foreach (var curve in _curves) yield return curve;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool IsOuter => _isOuter;

    public double Perimeter { get; }

    public double Area { get; }

    public Plane Plane { get; }

    public ICollection<IBoundedCurve> Boundary => _curves;

    private ReadOnlyCollection<IBoundedCurve>? _curvesReadOnly;
    public ICollection<IBoundedCurve> Curves
    {
        get
        {
            if (_curvesReadOnly == null) _curvesReadOnly = new ReadOnlyCollection<IBoundedCurve>(_curves.ToList());
            return _curvesReadOnly;
        }
    }

    private ReadOnlyCollection<Point>? _verticesReadOnly = null;
    public ICollection<Point> Vertices
    {
        get
        {
            if (_verticesReadOnly == null) _verticesReadOnly = new ReadOnlyCollection<Point>(_graph.Nodes.ToArray());
            return _verticesReadOnly;
        }
    }

    public ICollection<IFlatRegion> InnerRegions => (ICollection<IFlatRegion>)_innerRegions;

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
        throw new System.NotImplementedException();
    }

    public bool Intersects(IFlatRegion other)
    {
        throw new System.NotImplementedException();
    }
}