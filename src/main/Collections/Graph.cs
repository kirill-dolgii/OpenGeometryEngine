using System;
using System.Collections.Generic;
using System.Linq;
using OpenGeometryEngine;

namespace DataStructures.Graph;

public class Graph<TNode, TEdge>
{
    private readonly IDictionary<Node, HashSet<Node>> _adjacent;

    private readonly IDictionary<TNode, Node> _map;

    private readonly IDictionary<(Node x, Node y), TEdge> _edges;

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

    public Graph(bool directed, 
                 IEqualityComparer<TNode> nodeEqualityComparer,
                 IEqualityComparer<TEdge> edgeEqualityComparer) : 
        this(new Dictionary<TNode, ICollection<TNode>>(),
             new Dictionary<(TNode, TNode), TEdge>(),
             nodeEqualityComparer,
             edgeEqualityComparer,
             directed)
    { }

    private Graph(IDictionary<TNode, ICollection<TNode>> adjacency,
                  IDictionary<(TNode, TNode), TEdge> edges,
                  IEqualityComparer<TNode> nodeEqualityComparer,
                  IEqualityComparer<TEdge> edgeEqualityComparer,
                  bool directed)
    {
        NodeEqualityComparer = nodeEqualityComparer;
        _map = adjacency.Keys.ToDictionary(n => n, n => new Node(n), NodeEqualityComparer);
        _adjacent = adjacency.ToDictionary(kv => _map[kv.Key],
                                           kv => new HashSet<Node>(kv.Value.Select(n => _map[n])));
        _edges = edges.ToDictionary(kv => (_map[kv.Key.Item1], _map[kv.Key.Item2]), kv => kv.Value);
        EdgeEqualityComparer = edgeEqualityComparer;
        NodeEqualityComparer = nodeEqualityComparer;
        Directed = directed;
    }

    public Graph(ICollection<TNode> nodes, Func<TNode, ICollection<TNode>> adjecencyFinder, 
        Func<(TNode, TNode), TEdge> edgeFinder, bool directed) : this(directed)
    {
        foreach (var node in nodes)
        {
            var neighbours = adjecencyFinder(node);
            foreach (var neighbour in neighbours)
            {
                var edge = edgeFinder.Invoke((node, neighbour));
                AddEdge(node, neighbour, edge);
            }
        }
    }

    public ICollection<TNode> AdjacentNodes(TNode node) => _adjacent[_map[node]].Select(n => n.Item).ToArray();

    public TEdge Edge(TNode x, TNode y) => _edges[(_map[x], _map[y])];

    public ICollection<TEdge> Edges(TNode x)
    {
        Node xNode = _map[x];
        return _adjacent[_map[x]].Select(y => _edges[(xNode, y)]).ToList();
    }

    public ICollection<(TNode x, TNode y, TEdge edge)> Edges()
        => _edges.Select(kv => (kv.Key.x.Item, kv.Key.y.Item, kv.Value)).ToList();

    public int EdgesCount => Directed ? _edgesCount : _edgesCount / 2;

    public bool Directed { get; }

    public bool AddNode(TNode node)
    {
        Node graphNode = new Node(node);
        return AddNodeImpl(graphNode);
    }

    private bool AddNodeImpl(Node node)
    {
        if (ContainsNodeImpl(node)) return false;
        _map[node.Item] = node;
        _adjacent[node] = new HashSet<Node>();
        return true;
    }

    public bool ContainsNode(TNode node)
    {
        return _map.ContainsKey(node) && ContainsNodeImpl(_map[node]);
    }

    private bool ContainsNodeImpl(Node node)
        => _adjacent.ContainsKey(node);

    public bool RemoveNode(TNode node)
    {
        if (!ContainsNode(node)) return false;
        foreach (var kv in _edges.Where(kv => NodeEqualityComparer.Equals(kv.Key.y.Item, node)))
                RemoveEdge(kv.Key.x.Item, kv.Key.y.Item, kv.Value);
        _adjacent.Remove(_map[node]);
        return true;
    }

    public int Degree(TNode node) => _adjacent[_map[node]].Count;

    public bool AddEdge(TNode x, TNode y, TEdge edge)
    {
        if (!ContainsNode(x)) AddNode(x);
        if (!ContainsNode(y)) AddNode(y);
        if (ContainsEdge(x, y, edge)) return false;
        Node xNode = _map[x];
        Node yNode = _map[y];
        AddEdgeImpl(xNode, yNode, edge, Directed);
        return true;
    }

    private bool AddEdgeImpl(Node x, Node y, TEdge edge, bool directed)
    {
        if (ContainsEdgeImpl(x, y, edge)) return false;
        _edges[(x, y)] = edge;
        _adjacent[x].Add(y);
        _edgesCount++;
        if (!directed) return AddEdgeImpl(y, x, edge, true);
        return true;
    }

    public bool ContainsEdge(TNode x, TNode y, TEdge edge)
    {
        var graphX = _map[x];
        var graphY = _map[y];
        return ContainsEdgeImpl(graphX, graphY, edge);
    }

    private bool ContainsEdgeImpl(Node x, Node y, TEdge edge)
    {
        var tpl = (x, y);
        return _edges.ContainsKey(tpl) && EdgeEqualityComparer.Equals(_edges[tpl], edge);
    }

    public bool RemoveEdge(TNode x, TNode y, TEdge edge)
    {
        Node xNode = _map[x];
        Node yNode = _map[y];
        return RemoveEdgeImpl(xNode, yNode, edge, Directed);
    }

    private bool RemoveEdgeImpl(Node x, Node y, TEdge edge, bool directed)
    {
        if (!ContainsEdgeImpl(x, y, edge)) return false;
        _edges.Remove((x, y));
        _adjacent[x].Remove(y);
        _edgesCount--;
        if (!directed) RemoveEdgeImpl(y, x, edge, true);
        return true;
    }

    public ICollection<ValueTuple<TNode, TNode, TEdge>> DepthTraversal(TNode start)
    {
        Argument.IsNotNull(nameof(start), start);
        if (!ContainsNode(start)) throw new ArgumentException(nameof(start));
        var visited = _map.Values.ToDictionary(n => n, _ => false);
        var path = DepthTraversalHelp(_map[start], visited).ToArray();
        return path.Select(tpl => (tpl.Item1.Item, tpl.Item2.Item, tpl.Item3)).ToArray();
    }

    private IEnumerable<ValueTuple<Node, Node, TEdge>> DepthTraversalHelp(Node node, 
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

    public ICollection<ValueTuple<TNode, TNode, TEdge>> BreadthTraversal(TNode start)
    {
        Argument.IsNotNull(nameof(start), start);
        if (!ContainsNode(start)) throw new ArgumentException(nameof(start));
        var path = new LinkedList<ValueTuple<Node, Node, TEdge>>();
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
                n => _edges[(_map[n.Item1], _map[n.Item2])], Directed);
            ret.AddLast(newGraph);
        }
        return ret;
    }

    public ICollection<TNode> Nodes => _adjacent.Keys.Select(n => n.Item).ToArray();

    public int NodesCount => _adjacent.Count;

    private class Node
    {
        public readonly TNode Item;

        public Node(TNode item)
        {
            Item = item;
        }
    }
}