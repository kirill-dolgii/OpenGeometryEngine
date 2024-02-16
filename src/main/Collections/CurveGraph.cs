using System;
using System.Collections.Generic;
using System.Linq;
using DataStructures.Graph;

namespace OpenGeometryEngine.Collections;

internal sealed class CurveGraph : Graph<Point, IBoundedCurve>
{
    public Plane Plane { get; }

    public CurveGraph(ICollection<IBoundedCurve> curves, Plane plane) 
        : base(false, new PointEqualityComparer(), EqualityComparer<IBoundedCurve>.Default)
    {
        Argument.IsNotNull(nameof(curves), curves);
        Argument.IsNotNull(nameof(plane), plane);
        Plane = plane;
        foreach (var curve in curves)
        {
            AddEdge(curve.StartPoint, curve.EndPoint, curve);
        }
    }

    public static ICollection<IBoundedCurve[]> GetCounterClockWiseChains(ICollection<IBoundedCurve> curves, 
                                                                         Plane plane, 
                                                                         out CurveGraph graph)
    {
        Argument.IsNotNull(nameof(curves), curves);
        graph = new CurveGraph(curves, plane);
        return graph.Split().Select(g =>
        {
            var chain = OrderCounterClockWise(g, plane);
            return chain.Select(tpl =>
            {
                var curve = tpl.Item3;
                return (IBoundedCurve)(curve switch
                {
                    LineSegment lineSegment => (new LineSegment(tpl.Item1, tpl.Item2)),
                    Arc arc => (new Arc(arc.Circle.Origin, tpl.Item1, tpl.Item2, 
                                               tpl.Item1 == arc.StartPoint ? 
                                               arc.Circle.Frame.DirZ :
                                               arc.Circle.Frame.DirZ.Reverse())),
                    _ => throw new NotImplementedException()
                });
            }).ToArray();
        }).ToArray();
    }

    private static ICollection<ValueTuple<Point, Point, IBoundedCurve>> OrderCounterClockWise(Graph<Point, IBoundedCurve> graph, Plane plane)
    {
        var start = graph.Nodes.First();
        var chain = new LinkedList<ValueTuple<Point, Point, IBoundedCurve>>();
        var current = start;
        do
        {
            var next = graph.AdjacentNodes(current).First();
            var edge = graph.Edge(current, next);
            chain.AddLast((current, next, edge));
            graph.RemoveEdge(current, next);
            current = next;
        } while (current != start);
        return chain;
    }
}