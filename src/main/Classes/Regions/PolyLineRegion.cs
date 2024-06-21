using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.ObjectModel;
using OpenGeometryEngine.Collections;
using OpenGeometryEngine.Misc;

namespace OpenGeometryEngine;

public sealed class PolyLineRegion : IFlatRegion
{
    private readonly HashSet<IBoundedCurve> _curves;
    public readonly PolygonRegion _polygon;
    private bool _isOuter;
    private ICollection<PolyLineRegion> _innerRegions;
    private Graph<Point, IBoundedCurve> _graph;
    private Loop _loop;

	/// <summary>
	/// Creates PolyLineRegion with the only outer boundary i.e. there are no InnerRegions.
	/// </summary>
	/// <param name="loop"></param>
	/// <param name="plane"></param>
	internal PolyLineRegion(Loop loop, Plane plane)
    {
        _graph = new Graph<Point, IBoundedCurve>(false, PointEqualityComparer.Default, EqualityComparer<IBoundedCurve>.Default);
        foreach (var edge in loop)
        {
            _graph.AddEdge(edge.X._point, edge.Y._point, edge.Curve);
        }
        Plane = plane;
        _loop = loop;
        _curves = new HashSet<IBoundedCurve>(_graph.Edges);
        _polygon = new PolygonRegion(loop, plane, PolylineOptions.Default);
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

        var regions = polygonsGrouped
            .Select(kv => new PolyLineRegion(polygons[kv.Key], 
                            kv.Value.Select(polygon => polygons[polygon]).ToArray(), plane))
            .ToArray();

        foreach (var region in regions)
        {
            region._isOuter = true;
        }

        return regions;
    }

    public static ICollection<PolyLineRegion> CreateRegions(ICollection<IBoundedCurve> curves,
                                                            Plane plane)
    {
        Argument.IsNotNull(nameof(curves), curves);
        var solver = new LoopWire2dSolver(curves, Array.Empty<IBoundedCurve>(), plane);
        solver.Solve();
        //self-intersection test performed via PolygonRegion's self-intersection test
        return CreateFromLoops(solver.Loops, plane);        
    }

    public ICollection<PolyLineRegion> Intersect(PolyLineRegion other)
    {
        Argument.IsNotNull(nameof(other), other);
		//if (!other.Intersects(this))
		//{
		//	throw new Exception("Regions do not intersect");
		//}

		var classifiedWires = GetClassifiedIntersectionWires(this, other);
        var intersectionCurves = classifiedWires.aInsideB
						 .Concat(classifiedWires.bInsideA)
						 .Concat(classifiedWires.common)
                         .SelectMany(w => w.Select(fin => fin.Curve)).ToList();
		return CreateRegions(intersectionCurves, Plane);
	}

    public PolyLineRegion Merge(PolyLineRegion other)
    {
        Argument.IsNotNull(nameof(other), other);
        //if (!other.Intersects(this))
        //{
        //    throw new Exception("Regions do not intersect");
        //}

		var classifiedWires = GetClassifiedIntersectionWires(this, other);
		var intersectionCurves = classifiedWires.aOutsideB
						 .Concat(classifiedWires.bOutsideA)
						 .Concat(classifiedWires.common)
						 .SelectMany(w => w.Select(fin => fin.Curve)).ToList();
		return CreateRegions(intersectionCurves, Plane).Single();
	}

    public ICollection<PolyLineRegion> Subtract(PolyLineRegion other)
    {
		Argument.IsNotNull(nameof(other), other);
		//if (!other.Intersects(this))
		//{
		//	throw new Exception("Regions do not intersect");
		//}
            
		var classifiedWires = GetClassifiedIntersectionWires(this, other);
		var intersectionCurves = classifiedWires.aOutsideB
						 .Concat(classifiedWires.bInsideA)
						 .SelectMany(w => w.Select(fin => fin.Curve)).ToList();
		return CreateRegions(intersectionCurves, Plane);
	}

	public bool GetIntersectionCurves(ICollection<IBoundedCurve> curves,
										out Dictionary<IBoundedCurve, ICollection<IBoundedCurve>> map, 
                                        out ICollection<IBoundedCurve> intersectionCurves)
	{
		Argument.IsNotNull(nameof(curves), curves);
    
		var intersections = _curves.SelectMany(polygonCurve =>
				curves.SelectMany(curve => polygonCurve.IntersectCurve(curve)
					  .Select(ip => new { PolygonCurve = polygonCurve, SplitCurve = curve, Intersection = ip, Inner = false }))).ToArray();

		var innerIntersections = InnerRegions.SelectMany(region =>
			((PolyLineRegion)region)._curves.SelectMany(polygonCurve =>
				curves.SelectMany(curve => polygonCurve.IntersectCurve(curve)
					   .Select(ip => new { PolygonCurve = polygonCurve, SplitCurve = curve, Intersection = ip, Inner = true })))).ToArray();
		
        map = new();
        intersectionCurves = new List<IBoundedCurve>();
        
        //empty intersection
		if (!intersections.Any() && !innerIntersections.Any())
		{
			return false;
		}

        map = intersections.Concat(innerIntersections)
						.GroupBy(intersection => intersection.PolygonCurve)
						.ToDictionary(group => group.Key,
									  group => group.Key.Split(group.Select(inters => inters.Intersection.FirstEvaluation.Param).ToArray()));

        var groups = intersections.Concat(innerIntersections).GroupBy(intersection => intersection.SplitCurve).ToList();
        var splitSegments = new List<IBoundedCurve>();
		foreach (var group in groups)
        {
			var splitted = group.Key.Split(group.Select(inters => inters.Intersection.SecondEvaluation.Param).ToArray())
                                    .Where(curve => ContainsPoint(curve.MidPoint)).ToArray();
            splitSegments.AddRange(splitted);
		};

        intersectionCurves = splitSegments;
		return true;
	}

    public ICollection<PolyLineRegion> Split(ICollection<IBoundedCurve> splitCurves)
    {
        Argument.IsNotNull(nameof(splitCurves), splitCurves);
        var myCurves = Boundary.Concat(InnerRegions.SelectMany(region => region.Boundary)).ToList();
		
        if (!GetIntersectionCurves(splitCurves, out var map, out var intersectionCurves))
        {
            throw new Exception("failed");
        }

		var innerBoundary = new HashSet<IBoundedCurve>(InnerRegions
            .SelectMany(region => region.Boundary)
            .SelectMany(curve => map.ContainsKey(curve) && map[curve].Any() ? map[curve] : Iterate.Over(curve))
            .ToArray());

        foreach (var kv in map)
		{
            if (!kv.Value.Any()) continue;
			myCurves.Remove(kv.Key);
			foreach (var splitted in kv.Value)
			{
				myCurves.Add(splitted);
			}
		}
		foreach (var curve in intersectionCurves)
		{
			myCurves.Add(curve);
		}
		var solver = new LoopWire2dSolver(myCurves, Array.Empty<IBoundedCurve>(), Plane);
		solver.Solve();
        var outerLoops = solver.Loops.Where(loop => !loop.All(fin => innerBoundary.Contains(fin.Curve))).ToList();
        return CreateFromLoops(outerLoops, Plane);
    }

    private static (ICollection<Wire> aInsideB, ICollection<Wire> aOutsideB, 
                    ICollection<Wire> bOutsideA, ICollection<Wire> bInsideA, 
                    ICollection<Wire> common) 
        GetClassifiedIntersectionWires(PolyLineRegion a, PolyLineRegion b)
    {
		var myCurves = a.Boundary.Concat(a.InnerRegions.SelectMany(r => r.Boundary)).ToList();
		var otherCurves = b.Boundary.Concat(b.InnerRegions.SelectMany(r => r.Boundary)).ToList();

        a.GetIntersectionCurves(otherCurves, out var myMap, out var mySplitCurves);
        b.GetIntersectionCurves(myCurves, out var otherMap, out var otherSplitCurves);
		//if (!) || 
		//    !))
		//{
		//    throw new Exception("failed");
		//}

		foreach (var kv in myMap)
		{
			if (kv.Value.Count == 0) continue;
			if (myCurves.Contains(kv.Key))
			{
				myCurves.Remove(kv.Key);
				foreach (var splitted in kv.Value)
				{
					myCurves.Add(splitted);
				}

			}
		}

		foreach (var kv in otherMap)
		{
			if (kv.Value.Count == 0) continue;
			if (otherCurves.Contains(kv.Key))
			{
				otherCurves.Remove(kv.Key);
				foreach (var splitted in kv.Value)
				{
					otherCurves.Add(splitted);
				}

			}
		}

		var solver = new LoopWire2dSolver(myCurves, otherCurves, a.Plane);
		solver.Solve();

		var aInsideB = new HashSet<Wire>(WireEqualtiComparer.Default);
        var aOutsideB = new HashSet<Wire>(WireEqualtiComparer.Default);
        var bOutsideA = new HashSet<Wire>(WireEqualtiComparer.Default);
        var bInsideA = new HashSet<Wire>(WireEqualtiComparer.Default);
        var commonWires = new HashSet<Wire>(WireEqualtiComparer.Default);

		foreach (var wire in solver.WiresA)
        {
            if (b.ContainsPoint(wire.First().Curve.EvaluateAtProportion(0.5).Point)) 
                aInsideB.Add(wire);
            else aOutsideB.Add(wire);
		}
        
        foreach (var wire in solver.WiresB)
        {
            if (a.ContainsPoint(wire.First().Curve.EvaluateAtProportion(0.5).Point)) 
                bInsideA.Add(wire);
            else bOutsideA.Add(wire);
		}

		foreach (var wire in solver.Common)
        {
            var testPoint = wire.First().Curve.EvaluateAtProportion(0.5).Point;
            var cond1 = a.PointIsOnBound(testPoint);
            var cond2 = b.PointIsOnBound(testPoint);
			if (cond1 && cond2)
            {
				commonWires.Add(wire);
			}
            else if (cond1 && b.ContainsPoint(testPoint))
            {
                aInsideB.Add(wire);
			}
            else if (cond2 && a.ContainsPoint(testPoint))
            {
                bInsideA.Add(wire);
            }
        }

		return (aInsideB, aOutsideB, bOutsideA, bInsideA, commonWires);
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

    public bool PointIsOnBound(Point point)
    {
        if (!Plane.ContainsPoint(point)) return false;
        var allCurves = Boundary.Concat(InnerRegions.SelectMany(region => region.Boundary)).ToArray();
        foreach (var curve in allCurves)
        {
            if (curve.ContainsPoint(point)) return true;
        }
        return false;
    }

    public bool ContainsPoint(Point point)
    {
        if (!Plane.ContainsPoint(point)) return false;
        if (PointIsOnBound(point)) return true;
        if (!_polygon.ContainsPoint(point)) return false;
        foreach (var polygon in InnerRegions.Select(inner => ((PolyLineRegion)inner)._polygon))
        {
            if (polygon.ContainsPoint(point)) return false;
        }
        return true;
    }

    public bool Intersects(IFlatRegion other)
    {
        throw new System.NotImplementedException();
    }
}