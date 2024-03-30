using DataStructures.Graph;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenGeometryEngine.Misc.Solvers;

internal sealed class PlanarMinCycleSolver : Graph<Point, IBoundedCurve>, ISolver<ICollection<Loop>>
{
	private ICollection<Loop> _loops = new LinkedList<Loop>();
	private bool _solved = false;
	private Dictionary<Node, PlaneEvaluation> _projections;

	public Plane Plane { get; }

	public ICollection<Loop>? Result => _loops;

	public bool Solved => _solved;

	public PlanarMinCycleSolver(ICollection<IBoundedCurve> curves, Plane plane) : base(false)
	{
		//TODO: check if any of curves is not contained by the plane => exception
		foreach (var curve in curves)
		{
			AddEdge(curve.StartPoint, curve.EndPoint, curve);
		}
		Plane = plane;
		_projections = _map.Values.ToDictionary(node => node, node => (PlaneEvaluation)Plane.ProjectPoint(node.Item));
	}

	private double GetSignDot(UnitVec x, UnitVec y)
	{
		return x.X * y.Y - y.Y - x.Y;
	}

	private Node GetLeftMostNode()
	{
		var minDownLeft = _map.Values
			.OrderBy(node => _projections[node].Param.U)
			.ThenBy(node => _projections[node].Param.V).First();
		return minDownLeft;
	}

	private UnitVec GetTangent(IBoundedCurve curve, Node start, Node end)
	{
		if (curve is LineSegment)
			return (end.Item - start.Item).Unit;
		var startProj = curve.ProjectPoint(start.Item);
		var endProj = curve.ProjectPoint(end.Item);
		return startProj.Param > endProj.Param ? startProj.Tangent.Reverse() : startProj.Tangent;
	}

	private (Node x, Node y) GetMostEdge(Node? prev, Node curr, bool isClockWise)
	{
		Node? vNext = null;
		bool vCurrIsConvex = false;
		UnitVec rDCurr, rDNext = UnitVec.UnitX;
		if (prev != null)
		{
			rDCurr = (curr.Item - prev.Item).Unit;
		}
		else
		{
			rDCurr = Plane.Evaluate(new PointUV(0.0, -1.0)).Point.Vector.Unit;
		}

		foreach (var vAdj in _adjacent[curr])
		{
			var edge = _edges[(curr, vAdj)];
			if (edge is LineSegment && vAdj == prev) continue;
			//compute tangent to the IBoundedCurve in direction from curr to adj
			var rDAdj = GetTangent(edge, curr, vAdj); //.ProjectPoint(curr.Item).Tangent;
			if (vNext == null)
			{
				vNext = vAdj;
				rDNext = rDAdj;
				vCurrIsConvex = GetSignDot(rDNext, rDCurr) <= 0;
				continue;
			}

			// Update if the next candidate is more clockWise or counterClockWise of the current
			// clockWise or counterClockWise most vertex.

			if (!Vector.TryGetSignedAngleInDir(rDCurr, rDAdj, Plane.Frame.DirZ, out double angle1)) 
				throw new Exception();
			if (!Vector.TryGetSignedAngleInDir(rDCurr, rDNext, Plane.Frame.DirZ, out double angle2)) 
				throw new Exception();
			if (isClockWise ? angle1 < angle2 : angle1 > angle2) 
			{
				vNext = vAdj;
				rDNext = rDAdj; 
			}
		}
		return (curr, vNext!);
	}

	private (Node x, Node y) GetClockWiseMostEdge(Node? prev, Node curr) 
		=> GetMostEdge(prev, curr, true);

	private (Node x, Node y) GetCounterClockWiseMostEdge(Node? prev, Node curr) 
		=> GetMostEdge(prev, curr, false);

	private IList<Node> ClosedWalk(Node start)
	{
		var closedWalk = new List<Node>();
		var vCurr = start;
		closedWalk.Add(vCurr);
		var edge = GetClockWiseMostEdge(null, start);
		var vAdj = edge.y;
		while (vAdj != start)
		{
			closedWalk.Add(vAdj);
			edge = GetCounterClockWiseMostEdge(vCurr, vAdj);
			vCurr = vAdj;
			vAdj = edge.y;
		}
		closedWalk.Add(start);
		return closedWalk;
	}

	private Loop ExtractLoop(IList<Node> closedWalk)
	{
        var directedEdges = new List<DirectedEdge>();
		var vertices = new List<Point>();
        for (int i = 0; i < closedWalk.Count - 1; i++)
        {
			directedEdges.Add(new DirectedEdge(closedWalk[i].Item, 
											   closedWalk[i + 1].Item, 
											   _edges[(closedWalk[i], closedWalk[i + 1])]));
			vertices.Add(closedWalk[i].Item);
        };
		var loop = new Loop(directedEdges, vertices);
		return loop;
    }

	private void CleanUp(IList<Node> closedWalk)
	{
		var nodesToDelete = closedWalk.Where(node => _adjacent[node].Count == 2).ToArray();
		foreach (var node in nodesToDelete)
		{
			RemoveNode(node.Item);
		}
    }

	public ICollection<Loop> Solve()
	{
		if (_solved) return _loops;
		var loops = new List<Loop>();
		while (NodesCount > 0)
		{
			var startNode = GetLeftMostNode();
			var closedWalk = ClosedWalk(startNode);
			var loop = ExtractLoop(closedWalk);
			CleanUp(closedWalk);
			loops.Add(loop);
		}
		_loops = loops;
		_solved = true;
		return loops;
	}
}
