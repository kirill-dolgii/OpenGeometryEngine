//using System;
//using System.Collections.Generic;
//using System.Linq;
//using DataStructures.Graph;

//namespace OpenGeometryEngine;

//internal sealed class PlanarCurveGraph : Graph<Point, IBoundedCurve>
//{
//    public Plane Plane { get; }

//    public IBoundedCurve[] _curves;

//    private PlanarCurveGraph() : base(false, new PointEqualityComparer(), EqualityComparer<IBoundedCurve>.Default)
//    {
//    }

//    public PlanarCurveGraph(ICollection<IBoundedCurve> curves, Plane plane) : this()
//    {
//        Plane = plane;
//        _curves = new IBoundedCurve[curves.Count];
//        // TODO: check if plane doesn't contain any of curves then throw an error 
//        foreach (var curve in curves)
//        {
//            AddEdge(curve.StartPoint, curve.EndPoint, curve);
//        }

//    }

//    public ICollection<IBoundedCurve> WalkCurves(Point point)
//    {
//        if (!ContainsNode(point)) throw new Exception("no such point contained by the graph");
//        var current = _map[point];
//        var prev = _map[point];
//        var finalNode = _map[point];
//        var visited = _map.Values.ToDictionary(node => node, node => false);
//        Dictionary<Node, UnitVec?> tangents = _map.Values.ToDictionary(node => node, 
//                                                                       node => new Nullable<UnitVec>());
//        List<IBoundedCurve> curves = new();
//        do
//        {
//            visited[current] = true;
//            var neighbours = _adjacent[current].Where(node => (node == finalNode && prev != node) || 
//                                                               !visited[node]).ToArray();
//			var prevTangent = tangents[prev].HasValue ? tangents[prev].Value : UnitVec.UnitX;
//			var neighboursTangents = neighbours.Select(node =>
//            {
//                var tangent = _edges[(current, node)].Geometry switch
//                {
//                    Line line => (node.Item - current.Item).Normalize().Unit,
//                    Circle circle => Vector.Dot(circle.Frame.DirZ, Plane.Frame.DirZ) > 0.0 ?
//                               circle.ProjectPoint(current.Item).Tangent :
//                               circle.ProjectPoint(current.Item).Tangent.Reverse(),
//                    _ => throw new NotImplementedException()
//                };
//    //            if (Vector.Dot(Vector.Cross(prevTangent, tangent), Plane.Frame.DirZ) <= 0)
//    //                prevTangent = prevTangent.Reverse();
//				//if (!Vector.TryGetSignedAngleInDir(prevTangent, tangent, Plane.Frame.DirZ, out var angle))
//    //                throw new Exception();
//                return new { Tangent = tangent, Angle = Vector.Angle(prevTangent, tangent), Node = node };
//            }).ToArray();
//			var next = neighboursTangents.OrderBy(t => t.Angle).First();
//			prev = current;
//            tangents[current] = next.Tangent;
//            current = next.Node;
//			curves.Add(_edges[(current, prev)]);
//        } while (current != finalNode);
//        return curves;
//    }
//}