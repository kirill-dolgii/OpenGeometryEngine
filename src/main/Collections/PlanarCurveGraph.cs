

using System;
using System.Collections.Generic;
using System.Linq;
using DataStructures.Graph;

namespace OpenGeometryEngine;

internal sealed class PlanarCurveGraph : Graph<Point, IBoundedCurve>
{
    public Plane Plane { get; }

    public IBoundedCurve[] _curves;

    private PlanarCurveGraph() : base(false, new PointEqualityComparer(), EqualityComparer<IBoundedCurve>.Default)
    {
    }

    public PlanarCurveGraph(ICollection<IBoundedCurve> curves, Plane plane) : this()
    {
        Plane = plane;
        _curves = new IBoundedCurve[curves.Count];
        // TODO: check if plane doesn't contain any of curves then throw an error 
        foreach (var curve in curves)
        {
            AddEdge(curve.StartPoint, curve.EndPoint, curve);
        }

    }

    public ICollection<IBoundedCurve> WalkCurves(Point point)
    {
        if (!ContainsNode(point)) throw new Exception("no such point contained by the graph");
        var current = _map[point];
        var prev = _map[point];
        List<IBoundedCurve> curves = new();
        do
        {
            var neighbours = _adjacent[current];
            current = neighbours.OrderBy(node => 
            {
                var tangent = _edges[(current, node)].Geometry switch 
                {
                    LineSegment lineSegment => (node.Item - current.Item).Normalize().Unit,
                    Arc arc => Vector.Dot(arc.Circle.Frame.DirZ, Plane.Frame.DirZ) > 0.0 ? 
                               arc.ProjectPoint(current.Item).Tangent : 
                               arc.ProjectPoint(current.Item).Tangent.Reverse(),
                    _ => throw new NotImplementedException()
                };
                if (!Vector.TryGetSignedAngleInDir(Frame.World.DirX, tangent, Plane.Frame.DirZ, out var angle)) 
                    throw new Exception();
                return angle;
            }).First();
            curves.Add(_edges[(current, prev)]);
        } while (NodeEqualityComparer.Equals(point, current));
        
    }
}