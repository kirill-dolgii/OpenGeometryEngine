using System.Collections.Generic;

namespace OpenGeometryEngine.Interfaces;

public interface IGraph<TNode, TEdge>
{
    ICollection<TNode> Nodes { get; }

    ICollection<TEdge> Edges { get; }

    ICollection<TNode> AdjacentNodes(TNode node);

    ICollection<TEdge> AdjacentEdges(TNode node);

    TEdge Edge(TNode x, TNode y);

    bool ContainsNode(TNode node);

    bool ContainsEdge(TNode x, TNode y);

    int EdgesCount { get; }

    int NodesCount { get; }
}