using DataStructures.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace OpenGeometryEngine.Misc.Solvers;

internal sealed class PlanarMinCycleSolver : Graph<Point, IBoundedCurve>, ISolver<ICollection<Loop>>
{
	private Dictionary<Node, bool> _visitedNodes;
	private Dictionary<IBoundedCurve, bool> _visitedEdges;
	private ICollection<Loop> _loops = new LinkedList<Loop>();
	private bool solved = false;
	private Dictionary<Node, PlaneEvaluation> _projections;

	public Plane Plane { get; }

	public ICollection<Loop>? Result => throw new NotImplementedException();

	public bool Solved => throw new NotImplementedException();

	public PlanarMinCycleSolver(PlanarCurveGraph graph) : base(graph)
	{
		_visitedNodes = _map.Values.ToDictionary(node => node, node => false);
		_visitedEdges = Edges.ToDictionary(edge => edge, edge => false);
		Plane = graph.Plane;
		_projections = _map.Values.ToDictionary(node => node, node => (PlaneEvaluation)Plane.ProjectPoint(node.Item));
	}

	private double GetSignDot(UnitVec x, UnitVec y)
	{
		return x.X * y.Y - y.Y - x.Y;
	}

	private Node GetLeftMostNode()
	{
		var minDownLeft = _projections
			.Where(kv => _adjacent[kv.Key].Any(adj => !_visitedEdges[_edges[(kv.Key, adj)]]))
			.OrderBy(kv => kv.Value.Param.U)
			.ThenBy(kv => kv.Value.Param.V).First().Key;
		return minDownLeft;
	}

	private UnitVec GetTangent(IBoundedCurve curve, Node start, Node end)
	{
		if (curve is LineSegment)
			return (end.Item - start.Item).Unit;
		var startProj = curve.ProjectPoint(start.Item);
		var endProj = curve.ProjectPoint(end.Item);
		return startProj.Param > endProj.Param ? startProj.Tangent : startProj.Tangent.Reverse();
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
			rDCurr = (curr.Item - Plane.Evaluate(new PointUV(0.0, -1.0)).Point).Unit;
		}

		foreach (var vAdj in _adjacent[curr])
		{
			if (_visitedEdges[_edges[(curr, vAdj)]]) continue;
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
			double signDot0 = GetSignDot(rDCurr, rDAdj);
			double signDot1 = GetSignDot(rDNext, rDAdj);
			if (vCurrIsConvex)
			{
				if (isClockWise ? signDot0 < 0 || signDot1 < 0 : signDot0 > 0 && signDot1 > 0)
				{
					vNext = vAdj;
					rDNext = rDAdj;
					vCurrIsConvex = GetSignDot(rDNext, rDCurr) <= 0;
				}
			}
			else
			{
				if (isClockWise ? signDot0 < 0 && signDot1 < 0 : signDot0 > 0 || signDot1 > 0)
				{
					vNext = vAdj;
					rDNext = rDAdj;
					vCurrIsConvex = GetSignDot(rDNext, rDCurr) < 0;
				}
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
		_visitedEdges[_edges[edge]] = true;
		var vAdj = edge.y;
		while (vAdj != start)
		{
			closedWalk.Add(vAdj);
			edge = GetCounterClockWiseMostEdge(vCurr, vAdj);
			_visitedEdges[_edges[edge]] = true;
			_visitedNodes[vCurr] = true;
			vCurr = vAdj;
			vAdj = edge.y;
		}

		closedWalk.Add(start);
		return closedWalk;
	}

	//private ICollection<(Node x, Node y)> ExtractCycle(ICollection<Node> closedWalk)
	//{

	//}

	public ICollection<Loop> Solve()
	{
		var loops = new List<List<IBoundedCurve>>();
		while (NodesCount > 0)
		{
			var startNode = GetLeftMostNode();
			_visitedNodes[startNode] = true;
			var closedWalk = ClosedWalk(startNode);
			var edges = new List<IBoundedCurve>();
			for (int i = 0; i < closedWalk.Count - 1; i++)
			{
				edges.Add(_edges[(closedWalk[i], closedWalk[i + 1])]);
			};
			foreach (var node in closedWalk.Take(closedWalk.Count - 1))
			{
				var adjEdges = _adjacent[node].Select(y => _edges[(node, y)]).ToList();
				if (adjEdges.All(edge => _visitedEdges[edge]))
				{
					RemoveNode(node.Item);
					foreach (var edge in adjEdges) _visitedEdges.Remove(edge);
					_projections.Remove(node);
					_visitedNodes.Remove(node);
				}
			}
			foreach (var kv in _visitedEdges) _visitedEdges[kv.Key] = false;
			foreach (var kv in _visitedNodes) _visitedNodes[kv.Key] = false;
			loops.Add(edges);
		}
		return null;
	}
}
