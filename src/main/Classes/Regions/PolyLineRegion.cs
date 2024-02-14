using DataStructures.Graph;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.ObjectModel;
using OpenGeometryEngine.Exceptions.Region;
using System.Data;
using OpenGeometryEngine.Extensions;
using OpenGeometryEngine.Collections;

namespace OpenGeometryEngine.Regions;

public class PolyLineRegion : IFlatRegion
{
    private readonly IBoundedCurve[] _curves;
    private readonly Point[] _points;
    private readonly PolygonRegion _polygon;
    private readonly Graph<Point, IBoundedCurve> _graph;

    public PolyLineRegion(ICollection<IBoundedCurve> curves, Plane plane)
    {
        Argument.IsNotNull(nameof(curves), curves);
        var graph = new Graph<Point, IBoundedCurve>(false, 
                                                 new PointEqualityComparer(), 
                                                 EqualityComparer<IBoundedCurve>.Default);
        foreach (var curve in curves)
        {
            graph.AddEdge(curve.StartPoint, curve.EndPoint, curve);
        }
        if (graph.Nodes.Any(node => graph.Degree(node) != 2)) throw new NoLoopsException();
            
        var splited = graph.Split();
        var chains = splited.Select(OrderChain).ToArray();

        var orderedChains = chains.Select(ch => ch.Select(tpl =>
        {
            var curve = tpl.Item3;
            switch (curve)
            {
                case LineSegment lineSegment:
                {
                    return (IBoundedCurve)(new LineSegment(tpl.Item1, tpl.Item2));
                }
                case Arc arc:
                {
                    return (IBoundedCurve)(new Arc(arc.Circle.Origin, tpl.Item1, tpl.Item2, arc.Circle.Frame.DirZ));
                }
                default:
                {
                    throw new NotImplementedException();
                }
            }
        }).ToArray()).ToArray();

        var polygons = orderedChains.ToDictionary(ch =>
        {
            var points = ch.SelectMany(curve => curve.GetPolyline(PolylineOptions.Default)
                .Select(eval => eval.Point)).ToArray();
            var polygon = new PolygonRegion(points, plane);
            return polygon;
        });

        var outerPolygon = polygons.Count == 1 ?
            polygons.Single() :
            polygons.Single(kv => polygons.Keys.All(polygon => polygon == kv.Key || kv.Key.Contains(polygon)));

        _points = outerPolygon.Key.Points;
        _polygon = outerPolygon.Key;
        _curves = outerPolygon.Value;
        Plane = plane;
        Length = curves.Sum(curve => curve.Length);
        _graph = new Graph<Point, IBoundedCurve>(directed:false);
        foreach (var curve in _curves) _graph.AddEdge(curve.StartPoint, curve.EndPoint, curve);
        InnerRegions = polygons.Where(p => p.Key != _polygon)
            .Select(p => new PolyLineRegion(p.Value, p.Key, plane))
            .ToArray();
        Area = _polygon.Area - InnerRegions.Sum(ir => ir.Area);
    }

    internal PolyLineRegion(IBoundedCurve[] curves, PolygonRegion polygon, Plane plane)
    {
        _curves = curves;
        _polygon = polygon;
        _points = polygon.Points;
        _graph = new Graph<Point, IBoundedCurve>(directed: false);
        foreach (var curve in _curves) _graph.AddEdge(curve.StartPoint, curve.EndPoint, curve);
        Plane = plane;
        InnerRegions = Array.Empty<PolyLineRegion>();
        Length = curves.Sum(curve => curve.Length);
        Area = _polygon.Area;
    }

    private static ICollection<ValueTuple<Point, Point, IBoundedCurve>> OrderChain(Graph<Point, IBoundedCurve> graph)
    {
        var start = graph.Nodes.First();
        var chain = new LinkedList<ValueTuple<Point, Point, IBoundedCurve>>();
        var current = start;
        do
        {
            var next = graph.AdjacentNodes(current).First();
            var edge = graph.Edge(current, next);
            chain.AddLast((current, next, edge));
            graph.RemoveEdge(current, next, edge);
            current = next;
        } while (current != start);
        return chain;
    }

    public ICollection<PolyLineRegion> Split(Line line)
    {
        Argument.IsNotNull(nameof(line), line);

        var intersections = _curves.SelectMany(curve => curve.IntersectCurve(line))
            .OrderBy(ip => ip.SecondEvaluation.Param).Select(ip => ip.FirstEvaluation.Point).ToArray();

        var splittedMap = _curves.ToDictionary(curve => curve, curve => curve.Split(line))
            .Where(kv => kv.Value.Any()).ToDictionary(kv => kv.Key, kv => kv.Value);

        foreach (var kv in splittedMap)
        {
            _graph.RemoveEdge(kv.Key.StartPoint, kv.Key.EndPoint, kv.Key);
            foreach (var splited in kv.Value)
            {
                _graph.AddEdge(splited.StartPoint, splited.EndPoint, splited);
            }
        }

        var intersectionPairs = intersections.Pairs().ToArray();
        for (int i = 0; i < intersectionPairs.Length; i++)
        {
            if (i % 2 != 0) continue;
            var start = intersectionPairs[i].Item1;
            var end = intersectionPairs[i].Item2;
            _graph.AddEdge(start, end, new LineSegment(start, end));
        }

        var ret = Iterate.Over(1d, -1d).Select(sign =>
        {
            var nodes = _graph.Nodes
                .Where(node => !Accuracy.LengthIsZero((node - line.Origin).Magnitude) &&
                                sign * Vector.SignedAngle(line.Direction, (node - line.Origin).Unit, Frame.World.DirZ) >= 0)
                .ToList();
            if (_graph.ContainsNode(line.Origin)) nodes.Add(line.Origin);
            var g = Graph<Point, IBoundedCurve>.Copy(_graph, nodes);
            return new PolyLineRegion(g.Edges.Distinct().ToArray(), Plane);
        }).ToArray();

        return ret;
    }

    public IEnumerator<IBoundedCurve> GetEnumerator()
    {
        foreach (var curve in _curves) yield return curve;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public double Length { get; }

    public double Area { get; }

    public Plane Plane { get; }

    private ReadOnlyCollection<IBoundedCurve>? _curvesReadOnly = null;
    public ICollection<IBoundedCurve> Curves
    {
        get 
        {
            if (_curvesReadOnly == null) _curvesReadOnly = new ReadOnlyCollection<IBoundedCurve>(_curves);
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