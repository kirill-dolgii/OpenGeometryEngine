using DataStructures.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace OpenGeometryEngine.Misc.Solvers;

internal sealed class PlanarMinCycleSolver : Graph<Point, IBoundedCurve>, ISolver<ICollection<Loop>>
{
	private ICollection<Loop> _loops = new LinkedList<Loop>();
	private bool solved = false;
	private Dictionary<Node, PlaneEvaluation> _projections;

	public Plane Plane { get; }

	public ICollection<Loop>? Result => throw new NotImplementedException();

	public bool Solved => throw new NotImplementedException();

	public PlanarMinCycleSolver(PlanarCurveGraph graph) : base(graph)
	{
		Plane = graph.Plane;
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

			//double signDot0 = GetSignDot(rDCurr, rDAdj);
			//double signDot1 = GetSignDot(rDNext, rDAdj);
			//if (vCurrIsConvex)
			//{
			//	if (isClockWise ? signDot0 > 0 || signDot1 > 0 : signDot0 < 0 && signDot1 < 0)
			//	{
			//		vNext = vAdj;
			//		rDNext = rDAdj;
			//		vCurrIsConvex = GetSignDot(rDNext, rDCurr) <= 0;
			//	}
			//}
			//else
			//{
			//	if (isClockWise ? signDot0 > 0 && signDot1 > 0 : signDot0 < 0 || signDot1 < 0)
			//	{
			//		vNext = vAdj;
			//		rDNext = rDAdj;
			//		vCurrIsConvex = GetSignDot(rDNext, rDCurr) < 0;
			//	}
			//}
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
		//var edgesToDelete = new List<(Node x, Node y)>() { (closedWalk[0], closedWalk[1]) };

		//for (int i = 1; i < closedWalk.Count - 1; ++i)
		//{
		//	if (_adjacent[closedWalk[i]].Count < 2)
		//	{
		//		edgesToDelete.Add((closedWalk[i], closedWalk[i + 1]));
		//		continue;
		//	}
		//	break;
		//}
		//for (int i = closedWalk.Count - 1; i > 2; i--)
		//{
		//	if (_adjacent[closedWalk[i - 1]].Count < 2)
		//	{
		//		edgesToDelete.Add((closedWalk[i - 1], closedWalk[i]));
		//		continue;
		//	}
		//	break;
		//}
		//foreach (var edge in edgesToDelete) RemoveEdgeImpl(edge.x, edge.y);

    }

	public ICollection<Loop> Solve()
	{
		var loops = new List<Loop>();
		while (NodesCount > 0)
		{
			var startNode = GetLeftMostNode();
			var closedWalk = ClosedWalk(startNode);
			var loop = ExtractLoop(closedWalk);
			CleanUp(closedWalk);
			loops.Add(loop);
		}
		return loops;
	}
}
