using System;
using System.Collections.Generic;
using System.Linq;
using OpenGeometryEngine;

namespace DataStructures.Graph;

public class Graph<TNode, TEdge> : IGraph<TNode, TEdge>
{
    protected readonly IDictionary<Node, HashSet<Node>> _adjacent;

    protected readonly IDictionary<TNode, Node> _map;

    protected readonly IDictionary<(Node x, Node y), TEdge> _edges;

    private int _edgesCount;
    
    public IEqualityComparer<TNode> NodeEqualityComparer { get; }

    public IEqualityComparer<TEdge> EdgeEqualityComparer { get; }

    private Graph(IDictionary<TNode, ICollection<TNode>> adjacency,
                  IDictionary<(TNode, TNode), TEdge> edges,
                  IEqualityComparer<TNode> nodeEqualityComparer,
                  IEqualityComparer<TEdge> edgeEqualityComparer,
                  bool directed)
    {
        EdgeEqualityComparer = edgeEqualityComparer;
        NodeEqualityComparer = nodeEqualityComparer;
        Directed = directed;
        _map = adjacency.Keys.ToDictionary(n => n, n => new Node(n), NodeEqualityComparer);
        _adjacent = adjacency.ToDictionary(kv => _map[kv.Key],
            kv => new HashSet<Node>(kv.Value.Select(n => _map[n])));
        _edges = edges.ToDictionary(kv => (_map[kv.Key.Item1], _map[kv.Key.Item2]), kv => kv.Value);
        _edgesCount = Directed ? _edges.Count / 2 : _edges.Count;
    }

    /// <summary>
    /// Constructs an empty Graph object with default EqualityComparer for TEdge and TNode.
    /// </summary>
    public Graph(bool directed) : this(new Dictionary<TNode, ICollection<TNode>>(),
                                       new Dictionary<(TNode, TNode), TEdge>(),
                                       EqualityComparer<TNode>.Default,
                                       EqualityComparer<TEdge>.Default,
                                       directed)
    { }

    public Graph(bool directed, 
                 IEqualityComparer<TNode> nodeEqualityComparer,
                 IEqualityComparer<TEdge> edgeEqualityComparer) : 
        this(new Dictionary<TNode, ICollection<TNode>>(),
             new Dictionary<(TNode, TNode), TEdge>(),
             nodeEqualityComparer,
             edgeEqualityComparer,
             directed)
    { }
    
    public Graph(ICollection<TNode> nodes, Func<TNode, ICollection<TNode>> adjacencyFinder, 
                 Func<(TNode, TNode), TEdge> edgeFinder,
                 IEqualityComparer<TNode> nodeEqualityComparer, 
                 IEqualityComparer<TEdge> edgeEqualityComparer, bool directed) 
        : this(directed, nodeEqualityComparer, edgeEqualityComparer)
    {
        foreach (var node in nodes)
        {
            var neighbours = adjacencyFinder(node);
            foreach (var neighbour in neighbours)
            {
                var edge = edgeFinder.Invoke((node, neighbour));
                AddEdge(node, neighbour, edge);
            }
        }
    }

    public Graph(Graph<TNode, TEdge> other)
    {
        Argument.IsNotNull(nameof(other), other);
        var otherToThisNodes = other._map.ToDictionary(kv => kv.Value, kv => new Node(kv.Value.Item));
		_map = other._map.Values.ToDictionary(node => node.Item, 
                                node => otherToThisNodes[node], NodeEqualityComparer);
        _adjacent = other._adjacent.ToDictionary(kv => otherToThisNodes[kv.Key], 
            kv => new HashSet<Node>(other._adjacent[kv.Key].Select(node => otherToThisNodes[node])));
        _edges = other._edges.ToDictionary(kv => (otherToThisNodes[kv.Key.x], otherToThisNodes[kv.Key.y]),
                                           kv => kv.Value);
		_edgesCount = Directed ? _edges.Count / 2 : _edges.Count;
        NodeEqualityComparer = other.NodeEqualityComparer;
        EdgeEqualityComparer = other.EdgeEqualityComparer;
	}

	public static Graph<TNode, TEdge> Copy(Graph<TNode, TEdge> other, ICollection<TNode> nodes)
    {
        Argument.IsNotNull(nameof(other), other);
        Argument.IsNotNull(nameof(nodes), nodes);

        var adj = other._adjacent.ToDictionary(kv => kv.Key.Item, 
                                               kv => (ICollection<TNode>)kv.Value.Select(n => n.Item).ToArray());
        var edges = other._edges.ToDictionary(kv => (kv.Key.x.Item, kv.Key.y.Item), kv => kv.Value);

        var graph = new Graph<TNode, TEdge>(adj, edges, other.NodeEqualityComparer, 
                                            other.EdgeEqualityComparer, other.Directed);
        var exceptNodes = other.Nodes.Except(nodes, other.NodeEqualityComparer);
        foreach (var node in exceptNodes)
        {
            graph.RemoveNode(node);
        }
        return graph;
    }

    public int NodesCount => _adjacent.Count;

    public ICollection<TNode> Nodes => _adjacent.Keys.Select(n => n.Item).ToArray();

    public ICollection<TNode> AdjacentNodes(TNode node) => _adjacent[_map[node]].Select(n => n.Item).ToArray();
    
    public ICollection<TEdge> AdjacentEdges(TNode x)
    {
        Node xNode = _map[x];
        return _adjacent[_map[x]].Select(y => _edges[(xNode, y)]).ToList();
    }

    public ICollection<TEdge> Edges => _edges.Select(kv => kv.Value).Distinct(EdgeEqualityComparer).ToList();

    public TEdge Edge(TNode x, TNode y) => _edges[(_map[x], _map[y])];

    public int EdgesCount => Directed ? _edgesCount : _edgesCount / 2;

    public bool Directed { get; }

    public int Degree(TNode node) => DegreeImpl(_map[node]);

    protected int DegreeImpl(Node node) => _adjacent[node].Count;

    public bool AddNode(TNode node)
    {
        Node graphNode = new Node(node);
        return AddNodeImpl(graphNode);
    }

    protected bool AddNodeImpl(Node node)
    {
        if (ContainsNodeImpl(node)) return false;
        _map[node.Item] = node;
        _adjacent[node] = new HashSet<Node>();
        return true;
    }

    public bool AddEdge(TNode x, TNode y, TEdge edge)
    {
        if (!ContainsNode(x)) AddNode(x);
        if (!ContainsNode(y)) AddNode(y);
        if (ContainsEdge(x, y)) return false;
        Node xNode = _map[x];
        Node yNode = _map[y];
        AddEdgeImpl(xNode, yNode, edge, Directed);
        return true;
    }

    protected bool AddEdgeImpl(Node x, Node y, TEdge edge, bool directed)
    {
        if (ContainsEdgeImpl(x, y)) return false;
        _edges[(x, y)] = edge;
        _adjacent[x].Add(y);
        _edgesCount++;
        if (!directed) return AddEdgeImpl(y, x, edge, true);
        return true;
    }

    public bool ContainsNode(TNode node)
    {
        return _map.ContainsKey(node) && ContainsNodeImpl(_map[node]);
    }

    protected bool ContainsNodeImpl(Node node)
        => _adjacent.ContainsKey(node);

    public bool ContainsEdge(TNode x, TNode y)
    {
        if (!ContainsNode(x) || ContainsNode(y)) return false;
        var xNode = _map[x];
        var yNode = _map[y];
        return ContainsEdgeImpl(xNode, yNode);
    }

    protected bool ContainsEdgeImpl(Node x, Node y)
    {
        return _edges.ContainsKey((x, y));
    }
    
    public bool RemoveNode(TNode node)
    {
        if (!ContainsNode(node)) return false;
        var adjacentEdges = _edges.Where(kv => NodeEqualityComparer.Equals(kv.Key.y.Item, node)).ToList();
        foreach (var kv in adjacentEdges)
                RemoveEdge(kv.Key.x.Item, kv.Key.y.Item);
        _adjacent.Remove(_map[node]);
        return true;
    }

    public bool RemoveEdge(TNode x, TNode y)
    {
        Node xNode = _map[x];
        Node yNode = _map[y];
        return RemoveEdgeImpl(xNode, yNode);
    }

    protected bool RemoveEdgeImpl(Node x, Node y)
    {
        if (!ContainsEdgeImpl(x, y)) return false;
        _edges.Remove((x, y));
        _adjacent[x].Remove(y);
        _edgesCount--;
        if (!Directed) RemoveEdgeImpl(y, x);
        return true;
    }

    public static Graph<TNode, TEdge> Combine(Graph<TNode, TEdge> first, Graph<TNode, TEdge> second)
    {
        Argument.IsNotNull(nameof(second), second);
        foreach (var edge in second._edges)
        {
            first.AddEdge(edge.Key.x.Item, edge.Key.y.Item, edge.Value);
        }
        return first;
    }

    public ICollection<Graph<TNode, TEdge>> Split()
    {
        if (EdgesCount == 0) throw new Exception("Graph is empty");
        if (NodesCount == 0) throw new Exception("Graph is empty");

        var ret = new LinkedList<Graph<TNode, TEdge>>();

        var nodes = new HashSet<TNode>(Nodes, NodeEqualityComparer);
        while (nodes.Any())
        {
            var start = nodes.First();
            var bfs = BreadthTraversal(start);
            var bfsNodes = bfs.Select(tpl => tpl.Item2).ToArray();
            foreach (var tpl in bfs)
            {
                nodes.Remove(tpl.Item2);
            }
            nodes.Remove(start);
            var newGraph = new Graph<TNode, TEdge>(bfsNodes,
                n => _adjacent[_map[n]].Select(mapped => mapped.Item).ToArray(),
                n => _edges[(_map[n.Item1], _map[n.Item2])],
                NodeEqualityComparer, EdgeEqualityComparer, Directed);
            ret.AddLast(newGraph);
        }
        return ret;
    }

    public ICollection<(TNode, TNode, TEdge)> DepthTraversal(TNode start)
    {
        Argument.IsNotNull(nameof(start), start);
        if (!ContainsNode(start)) throw new ArgumentException(nameof(start));
        var visited = _map.Values.ToDictionary(n => n, _ => false);
        var path = DepthTraversalHelp(_map[start], visited).ToArray();
        return path.Select(tpl => (tpl.Item1.Item, tpl.Item2.Item, tpl.Item3)).ToArray();
    }

    private IEnumerable<(Node, Node, TEdge)> DepthTraversalHelp(Node node, 
                                                                          Dictionary<Node, bool> visited)
    {
        visited[node] = true;
        foreach (var adjacent in _adjacent[node])
        {
            if (visited[adjacent]) continue;
            yield return (node, adjacent, _edges[(node, adjacent)]);
            foreach (var tpl in DepthTraversalHelp(adjacent, visited)) yield return tpl;
        }
    }

    public ICollection<(TNode, TNode, TEdge)> BreadthTraversal(TNode start)
    {
        Argument.IsNotNull(nameof(start), start);
        if (!ContainsNode(start)) throw new ArgumentException(nameof(start));
        var path = new LinkedList<( Node, Node, TEdge)>();
        var queue = new Queue<Node>();
        var visited = _map.Values.ToDictionary(n => n, _ => false);
        var parent = _map.Values.ToDictionary(n => n, n => n);
        var stargGraphNode = _map[start];
        visited[stargGraphNode] = true;
        
        queue.Enqueue(stargGraphNode);
        while (queue.Any())
        {
            var node = queue.Dequeue();
            visited[node] = true;
            var neighbours = _adjacent[node];
            foreach (var nbr in neighbours)
            {
                if (!visited[nbr])
                {
                    queue.Enqueue(nbr);
                    parent[nbr] = node;
                }
            }
            if (NodeEqualityComparer.Equals(node.Item, start)) continue;
            var edge = _edges[(parent[node], node)];
            path.AddLast((parent[node], node, edge));
        }
        return path.Select(tpl => (tpl.Item1.Item, tpl.Item2.Item, tpl.Item3)).ToArray();
    }

    protected class Node
    {
        public readonly TNode Item;

        public Node(TNode item)
        {
            Item = item;
        }
    }
}