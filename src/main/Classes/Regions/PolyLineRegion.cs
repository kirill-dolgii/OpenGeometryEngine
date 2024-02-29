using DataStructures.Graph;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.ObjectModel;
using OpenGeometryEngine.Extensions;
using OpenGeometryEngine.Collections;

namespace OpenGeometryEngine.Regions;

public sealed class PolyLineRegion : IFlatRegion
{
    private readonly HashSet<IBoundedCurve> _curves;
    private readonly Point[] _points;
    public readonly PolygonRegion _polygon;
    private readonly Graph<Point, IBoundedCurve> _graph;
    private bool _isOuter;

    public static ICollection<PolyLineRegion> CreatePolygons(ICollection<IBoundedCurve> curves,
                                                             Plane plane)
    {
        Argument.IsNotNull(nameof(curves), curves);
        var orderedChains = CurveGraph.GetCounterClockWiseChains(curves, plane, out var graph);
        var polygons = orderedChains.ToDictionary(ch =>
        {
            var points = ch.SelectMany(curve => curve.GetPolyline(PolylineOptions.Default)
                .Select(eval => eval.Point)).ToArray();
            var polygon = new PolygonRegion(points, plane);
            return polygon;
        });

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
            var innerRegions = kv.Value;
            return new PolyLineRegion(polygons[region], region,
                innerRegions.Select(r => polygons[r]).ToArray(), innerRegions, plane);
        }).ToArray();

        return regions;
    }

    private PolyLineRegion(IBoundedCurve[] outerCurves,
                           PolygonRegion outerPolygon,
                           ICollection<IBoundedCurve[]> innerCurves,
                           ICollection<PolygonRegion> innerRegions,
                           Plane plane)
    {
        _points = outerPolygon.Points;
        _polygon = outerPolygon;
        _curves = new HashSet<IBoundedCurve>(outerCurves);
        Plane = plane;
        _graph = new CurveGraph(outerCurves.Concat(innerCurves.SelectMany(curves => curves)).ToArray(), plane);
        _isOuter = true;
        InnerRegions = innerRegions
            .Zip(innerCurves,
                (polygon, curves) =>
                {
                    var region = new PolyLineRegion(curves, polygon,
                            Array.Empty<IBoundedCurve[]>(), Array.Empty<PolygonRegion>(), plane);
                    region._isOuter = false;
                    return region;
                }).ToArray();

        Area = _polygon.Area - InnerRegions.Sum(ir => ir.Area);
        Length = _curves.Sum(curve => curve.Length) + InnerRegions.Sum(inner => inner.Length);
    }

    public Pair<ICollection<PolyLineRegion>> Split(Line line)
    {
        Argument.IsNotNull(nameof(line), line);

        var graph = Graph<Point, IBoundedCurve>.Copy(_graph, _graph.Nodes);

        var pntComparer = new PointEqualityComparer();

        var intersections = graph.Edges.SelectMany(curve => curve.IntersectCurve(line))
            .OrderBy(ip => ip.SecondEvaluation.Param).Select(ip => ip.FirstEvaluation.Point)
            .Distinct(pntComparer)
            .ToArray();

        var splittedMap = graph.Edges.ToDictionary(curve => curve, curve => curve.Split(line))
            .Where(kv => kv.Value.Any()).ToDictionary(kv => kv.Key, kv => kv.Value);

        foreach (var kv in splittedMap)
        {
            graph.RemoveEdge(kv.Key.StartPoint, kv.Key.EndPoint);
            foreach (var splited in kv.Value)
            {
                graph.AddEdge(splited.StartPoint, splited.EndPoint, splited);
            }
        }

        var intersectionPairs = intersections.Pairs().ToArray();
        for (int i = 0; i < intersectionPairs.Length; i++)
        {
            if (i % 2 != 0) continue;
            graph.AddEdge(intersectionPairs[i].Item1, intersectionPairs[i].Item2,
                new LineSegment(intersectionPairs[i].Item1, intersectionPairs[i].Item2));
        }

        var ret = Iterate.Over(1d, -1d).Select(sign =>
        {
            var nodes = graph.Nodes
                .Where(node => !Accuracy.LengthIsZero((node - line.Origin).Magnitude) &&
                        (sign * Vector.SignedAngle(line.Direction, (node - line.Origin).Unit, Frame.World.DirZ) >= 0 ||
                         Accuracy.LengthIsZero(Vector.Cross(line.Direction, (node - line.Origin).Unit).Magnitude)))
                .ToList();
            if (nodes.Count == 0) return Array.Empty<PolyLineRegion>();
            var graphs = Graph<Point, IBoundedCurve>.Copy(graph, nodes).Split();
            return CreatePolygons(graphs.SelectMany(gr => gr.Edges).ToArray(), Plane).ToArray();
        }).Where(p => p != null).ToArray();

        return new Pair<ICollection<PolyLineRegion>>(ret[0], ret[1]);
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

    public double Length { get; }

    public double Area { get; }

    public Plane Plane { get; }

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
            if (_verticesReadOnly == null) _verticesReadOnly = new ReadOnlyCollection<Point>(_graph.Nodes.ToList());
            return _verticesReadOnly;
        }
    }

    public ICollection<IFlatRegion> InnerRegions { get; }

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