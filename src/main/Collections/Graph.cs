using OpenGeometryEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataStructures.Graph;

public class Graph<TNode, TEdge>
{
    private readonly IDictionary<TNode, HashSet<TNode>> _adjacent;

    private readonly IDictionary<(TNode x, TNode y), TEdge> _edges;

    public IEqualityComparer<TNode> NodeEqualityComparer { get; }

    public IEqualityComparer<TEdge> EdgeEqualityComparer { get; }

    private int _edgesCount;

    /// <summary>
    /// Constructs an empty Graph object with default EqualityComparer for TEdge and TNode.
    /// </summary>
    public Graph(bool directed) : this(new Dictionary<TNode, ICollection<TNode>>(),
                                       new Dictionary<(TNode, TNode), TEdge>(),
                                       EqualityComparer<TNode>.Default,
                                       EqualityComparer<TEdge>.Default,
                                       directed)
    { }

    private Graph(IDictionary<TNode, ICollection<TNode>> adjacency,
                  IDictionary<(TNode, TNode), TEdge> edges,
                  IEqualityComparer<TNode> nodeEqualityComparer,
                  IEqualityComparer<TEdge> edgeEqualityComparer,
                  bool directed)
    {
        NodeEqualityComparer = nodeEqualityComparer;
        _adjacent = adjacency.ToDictionary(kv => kv.Key,
                                           kv => new HashSet<TNode>(kv.Value, NodeEqualityComparer),
                                           NodeEqualityComparer);
        _edges = edges.ToDictionary(kv => kv.Key, kv => kv.Value);
        EdgeEqualityComparer = edgeEqualityComparer;
        NodeEqualityComparer = nodeEqualityComparer;
        Directed = directed;
    }

    public Graph(ICollection<TNode> nodes, Func<TNode, ICollection<TNode>> adjecencyFinder, 
        Func<(TNode, TNode), ICollection<TEdge>> edgeFinder, bool directed) : this(directed)
    {
        foreach (var node in nodes)
        {
            var neighbours = adjecencyFinder(node);
            foreach (var neighbour in neighbours)
            {
                var edges = edgeFinder.Invoke((node, neighbour));
                foreach (var edge in edges)
                {
                    AddEdge(node, neighbour, edge);
                }
            }
        }
    }

    public ICollection<TNode> AdjacentNodes(TNode node) => _adjacent[node];

    public TEdge Edge(TNode x, TNode y) => _edges[(x, y)];

    public ICollection<TEdge> Edges(TNode x) => _adjacent[x].Select(y => _edges[(x, y)]).ToList();

    public ICollection<(TNode x, TNode y, TEdge edge)> Edges()
        => _edges.Select(kv => (kv.Key.x, kv.Key.y, kv.Value)).ToList();

    public int EdgesCount => Directed ? _edgesCount : _edgesCount / 2;

    public bool Directed { get; }

    public void AddNode(TNode node)
    {
        if (ContainsNode(node)) return;
        _adjacent[node] = new HashSet<TNode>(NodeEqualityComparer);
    }

    public bool ContainsNode(TNode node) => _adjacent.ContainsKey(node);

    public bool RemoveNode(TNode node)
    {
        if (!ContainsNode(node)) return false;
        foreach (var kv in _edges.Where(kv => NodeEqualityComparer.Equals(kv.Key.y, node)))
                RemoveEdge(kv.Key.x, kv.Key.y, kv.Value);
        _adjacent.Remove(node);
        return true;
    }

    public int Degree(TNode node) => _adjacent[node].Count;

    public void AddEdge(TNode x, TNode y, TEdge edge) => AddEdgeImpl(x, y, edge, Directed);

    private void AddEdgeImpl(TNode x, TNode y, TEdge edge, bool directed)
    {
        if (ContainsEdge(x, y, edge)) return;
        if (!ContainsNode(x)) AddNode(x);
        if (!ContainsNode(y)) AddNode(y);
        _edges[(x, y)] = edge;
        _adjacent[x].Add(y);
        _edgesCount++;
        if (!directed) AddEdgeImpl(y, x, edge, true);
    }

    public bool ContainsEdge(TNode x, TNode y, TEdge edge) =>
        _edges.ContainsKey((x, y)) && EdgeEqualityComparer.Equals(_edges[(x, y)], edge);

    public bool RemoveEdge(TNode x, TNode y, TEdge edge) => RemoveEdgeImpl(x, y, edge, Directed);

    private bool RemoveEdgeImpl(TNode x, TNode y, TEdge edge, bool directed)
    {
        if (!ContainsEdge(x, y, edge)) return false;
        _edges.Remove((x, y));
        _edgesCount--;
        if (!directed) RemoveEdgeImpl(y, x, edge, true);
        return true;
    }

    public ICollection<(TNode, TNode, TEdge)> DepthTraversal(TNode startNode)
    {
        Argument.IsNotNull(nameof(startNode), startNode);
        if (!ContainsNode(startNode)) throw new ArgumentException(nameof(startNode));
        var visited = Nodes.ToDictionary(n => n, _ => false, NodeEqualityComparer);
        var path = DepthTraversalHelp(startNode, visited).ToArray();
        return path;
    }

    private IEnumerable<(TNode, TNode, TEdge)> DepthTraversalHelp(TNode node, 
        Dictionary<TNode, bool> visited)
    {
        visited[node] = true;
        foreach (var adjacent in _adjacent[node])
        {
            if (visited[adjacent]) continue;
            yield return (node, adjacent, Edge(node, adjacent));
            foreach (var tpl in DepthTraversalHelp(adjacent, visited)) yield return tpl;
        }
    }

    public ICollection<TNode> Nodes => _adjacent.Keys;

    public int NodesCount => _adjacent.Count;
}