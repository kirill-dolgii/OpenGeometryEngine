using DataStructures.Graph;
using OpenGeometryEngine.BRep;
using OpenGeometryEngine.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace OpenGeometryEngine.Misc.Solvers;

internal class LoopWire2dSolver : Graph<Point, IBoundedCurve>, ISolver<ICollection<Wire>>
{
	private HashSet<Node> _nodesA; 
	private HashSet<Node> _nodesB;
	private HashSet<Node> _intersections;

	private ISurface _surface;
	private Dictionary<Node, ISurfaceEvaluation> _evaluations;
    internal Dictionary<IBoundedCurve, bool> _visitedEdges;

    public LoopWire2dSolver(ICollection<IBoundedCurve> aCurves, 
						ICollection<IBoundedCurve> bCurves, 
						ISurface surface) : 
		base(false, PointEqualityComparer.Default, EqualityComparer<IBoundedCurve>.Default)
	{
		_surface = surface;

		foreach (IBoundedCurve curve in aCurves)
		{
			AddEdge(curve.StartPoint, curve.EndPoint, curve);
		}

		_nodesA = new (_map.Values);
		_intersections = new ();

		foreach (IBoundedCurve curve in bCurves)
		{
			var cond1 = _map.TryGetValue(curve.StartPoint, out Node x);
			var cond2 = _map.TryGetValue(curve.EndPoint, out Node y);
			if (!ContainsEdgeImpl(x, y))
			{
				AddEdge(curve.StartPoint, curve.EndPoint, curve);
			}
			if (cond1 && _nodesA.Contains(x))
			{
				_nodesA.Remove(x);
				_intersections.Add(x);
			}
			if (cond2 && _nodesA.Contains(y))
			{
				_nodesA.Remove(y);
				_intersections.Add(y);
			}
		}
		_nodesB = new (_map.Values.Except(_nodesA).Except(_intersections));
		_evaluations = _map.Values.ToDictionary(node => node, node => _surface.ProjectPoint(node.Item));
		_visitedEdges = Edges.ToDictionary(edge => edge, edge => false);
    }

	private Node GetLeftMostNode()
	{
		var minDownLeft = _map.Values
			.OrderBy(node => _evaluations[node].Param.U)
			.ThenBy(node => _evaluations[node].Param.V).First();
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

	public (Node x, Node y) GetFirstEdge(Node start)
	{
		var tangent = _surface.Evaluate(new PointUV(0.0, -1.0)).Point.Vector.Unit;
		var neighbours = _adjacent[start].Where(node => !_visitedEdges[_edges[(start, node)]]).ToList();
		if (neighbours.Count == 1) return (start, neighbours.Single());
		var neighbourTangents = neighbours.Select(n => GetTangent(_edges[(start, n)], start, n)).ToList();
		//change the tangent to be the most concave for simple comparison
		tangent = neighbourTangents.OrderBy(neighbourTangent =>
		{
			Vector.TryGetSignedAngleInDir(tangent, neighbourTangent, _evaluations[start].Normal, out var angle);
			if (angle < 0.0) angle += 2 * System.Math.PI;
			return angle;
		}).First();

		Node? best = null;
		double minAngle = double.MaxValue;
		for (int i = 0; i < neighbours.Count; i++)
		{
			Vector.TryGetSignedAngleInDir(tangent, neighbourTangents[i], _evaluations[start].Normal, out var angle);
			if (angle < 0.0) angle += 2 * System.Math.PI;
			if (angle < minAngle)
			{
				minAngle = angle;
				best = neighbours[i];
			}
		}
		return (start, best!);
	}

	public (Node x, Node y) GetNextEdge(Node start, Node prev)
	{
		var edge = _edges[(start, prev)];
		var tangent = GetTangent(edge, prev, start);
		var neighbours = _adjacent[start].Where(node => !_visitedEdges[_edges[(start, node)]]).ToList();
		if (neighbours.Count == 1) return (start, neighbours.Single());
		var neighbourTangents = neighbours.Select(n => GetTangent(_edges[(start, n)], start, n)).ToList();
		//change the tangent to be the most concave for simple comparison
		tangent = neighbourTangents.OrderBy(neighbourTangent =>
		{
			Vector.TryGetSignedAngleInDir(tangent, neighbourTangent, _evaluations[start].Normal, out var angle);
			return angle;
		}).First();

		Node? best = null;
		double maxAngle = double.MinValue;
		for (int i = 0; i < neighbours.Count; i++)
		{
			Vector.TryGetSignedAngleInDir(tangent, neighbourTangents[i], _evaluations[start].Normal, out var angle);
			if (angle < 0.0) angle += 2 * System.Math.PI;
			if (angle > maxAngle)
			{
				maxAngle = angle;
				best = neighbours[i];
			}
		}
		return (start, best!);
	}

	private IList<Node> ClosedWalk(Node start)
	{
		var closedWalk = new List<Node>();
		var vCurr = start;
		closedWalk.Add(vCurr);
		var edge = GetFirstEdge(start);
		Node vAdj = edge.y;
		while (vAdj != start)
		{
			if (closedWalk.Contains(vAdj)) // start is not in the loop
			{
				var vAdjI = closedWalk.IndexOf(vAdj);
				closedWalk.RemoveRange(0, vAdjI);
				closedWalk.Add(vAdj);
				return closedWalk;
			}
			_visitedEdges[_edges[edge]] = true;
			closedWalk.Add(vAdj);
			edge = GetNextEdge(vAdj, vCurr);
			vCurr = vAdj;
			vAdj = edge.y;
		}
		closedWalk.Add(start);
		return closedWalk;
	}

	private Loop ExtractLoop(IList<Node> closedWalk)
	{
		var directedEdges = new List<Fin>();
		var vertices = new HashSet<Vertex>();
		for (int i = 0; i < closedWalk.Count - 1; i++)
		{
			var x = new Vertex(closedWalk[i].Item);
			var y = new Vertex(closedWalk[i + 1].Item);
			directedEdges.Add(new Fin(x, y, _edges[(closedWalk[i], closedWalk[i + 1])]));
			vertices.Add(x);
		};
		var loop = new Loop(directedEdges, vertices);
		return loop;
	}

	private List<HashSet<Node>> SplitSequence(IList<Node> sequence)
	{
		List<HashSet<Node>> result = new ();
		HashSet<Node> currentSegment = new ();

		//shift so that inters node is first
		// TODO : Add extension method to perform Shift
		var i = 0;
		for (; i < sequence.Count && !_intersections.Contains(sequence[i]); i++) { }
		var shifted = new List<Node>(sequence);
		if (i != 0)
		{
			var shift = sequence.Skip(1).Take(i - 1);
			shifted = sequence.Skip(i).Concat(shift).ToList();
			shifted.Add(shifted[0]);
		}

		for (int j = 0; j < shifted.Count; j++)
		{
			if (_intersections.Contains(shifted[j]))
			{
				if (currentSegment.Count > 0)
				{
					currentSegment.Add(shifted[j--]);
					result.Add(currentSegment);
					currentSegment = new ();
				}
				else
				{
					currentSegment.Add(shifted[j]);
				}
			}
			else
			{
				currentSegment.Add(shifted[j]);
			}
		}

		if (currentSegment.Count > 1)
		{
			result.Add(currentSegment);
		}

		return result;

	}

	private void CleanUp(IList<Node> closedWalk)
	{
		var counterClockWiseRemoveNodes = closedWalk.TakeWhile(node => _adjacent[node].Count == 2).ToArray();
		var clockWiseRemoveNodes = closedWalk.Reverse().TakeWhile(node => _adjacent[node].Count == 2).ToArray();
		var removeNodes = counterClockWiseRemoveNodes.Concat(clockWiseRemoveNodes).Distinct().ToList();
		foreach (var node in removeNodes)
		{
			RemoveNode(node.Item);
		}
		RemoveEdgeImpl(closedWalk[0], closedWalk[1]);

		while (_adjacent.Any(kv => kv.Value.Count < 2))
		{
			RemoveNode(_adjacent.First(kv => kv.Value.Count < 2).Key.Item);
		}
        _visitedEdges = Edges.ToDictionary(edge => edge, edge => false);
    }

    private bool _solved;
	private HashSet<Wire> _wires = new (WireEqualtiComparer.Default);
	private HashSet<Loop> _loops = new (); 

	public HashSet<Wire> WiresA = new (WireEqualtiComparer.Default);
	public HashSet<Wire> WiresB = new (WireEqualtiComparer.Default);
	public HashSet<Wire> Common = new (WireEqualtiComparer.Default);

	public ICollection<Wire> Solve()
	{
		if (Solved) return _wires;
		var nodesToVertices = _map.Values.ToDictionary(n => n, n => new Vertex(n.Item));
		while (NodesCount > 0)
		{
			var startNode = GetLeftMostNode();
			var closedWalk = ClosedWalk(startNode);
			var seq = SplitSequence(closedWalk);
			foreach (var s in seq)
			{
				var w = new Wire(s
						.Pairs(closed: false)
						.Select(pair => new Fin(nodesToVertices[pair.First],
												nodesToVertices[pair.Second], 
									   			_edges[(pair.First, pair.Second)])).ToArray());
				if (!_wires.Add(w)) continue;
				if (s.Any(node => _nodesA.Contains(node)))
				{
					WiresA.Add(w);
				}
				else if (s.Any(node => _nodesB.Contains(node)))
				{
					WiresB.Add(w);
				}
				else
				{
					Common.Add(w);
				}
			}
			var loop = ExtractLoop(closedWalk);
			CleanUp(closedWalk);
			_loops.Add(loop);
		}
		_solved = true;
		return _wires;
	}

	public ICollection<Wire> Result => _wires;

	public ICollection<Loop> Loops => _loops;

	public bool Solved => _solved;
}
