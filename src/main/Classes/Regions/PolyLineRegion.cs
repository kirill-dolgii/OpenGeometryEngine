using DataStructures.Graph;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using OpenGeometryEngine.Exceptions.Region;

namespace OpenGeometryEngine.Regions;

public class PolyLineRegion : IFlatRegion
{
    private readonly IBoundedCurve[] _curves;
    private readonly Point[] _points;
    private readonly PolygonRegion _polygon;

    public PolyLineRegion(ICollection<IBoundedCurve> curves, Plane plane)
    {
        Argument.IsNotNull(nameof(curves), curves);
        var gr = new Graph<Point, IBoundedCurve>(false, 
                                                 new PointEqualityComparer(), 
                                                 EqualityComparer<IBoundedCurve>.Default);
        foreach (var curve in curves)
        {
            gr.AddEdge(curve.StartPoint, curve.EndPoint, curve);
        }
        if (gr.Nodes.Any(node => gr.Degree(node) != 2)) throw new NoLoopsException();
            
        var splited = gr.Split();
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
        InnerRegions = polygons.Where(p => p.Key != _polygon).Select(p => new PolyLineRegion(p.Value, p.Key, plane))
            .ToArray();
        Area = _polygon.Area - InnerRegions.Sum(ir => ir.Area);
    }

    internal PolyLineRegion(IBoundedCurve[] curves, PolygonRegion polygon, Plane plane)
    {
        _curves = curves;
        _polygon = polygon;
        _points = polygon.Points;
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