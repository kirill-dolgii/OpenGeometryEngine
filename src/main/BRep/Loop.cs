using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OpenGeometryEngine;

internal struct Loop : ICollection<Fin>
{
    private Fin[] _edges;

    public ICollection<Vertex> Vertices { get; }

    internal Loop(ICollection<Fin> edges, ICollection<Vertex> vertices)
    {
        _edges = edges.ToArray();
        Vertices = vertices;
    }

    public int Count => ((ICollection<Fin>)_edges).Count;

    public bool IsReadOnly => ((ICollection<Fin>)_edges).IsReadOnly;

    public void Add(Fin item)
    {
        ((ICollection<Fin>)_edges).Add(item);
    }

    public void Clear()
    {
        ((ICollection<Fin>)_edges).Clear();
    }

    public bool Contains(Fin item)
    {
        return ((ICollection<Fin>)_edges).Contains(item);
    }

    public void CopyTo(Fin[] array, int arrayIndex)
    {
        ((ICollection<Fin>)_edges).CopyTo(array, arrayIndex);
    }

    public IEnumerator<Fin> GetEnumerator()
    {
        return ((IEnumerable<Fin>)_edges).GetEnumerator();
    }

    public bool Remove(Fin item)
    {
        return ((ICollection<Fin>)_edges).Remove(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _edges.GetEnumerator();
    }
}
