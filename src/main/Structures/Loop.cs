using OpenGeometryEngine.Collections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OpenGeometryEngine;

public struct Loop : ICollection<DirectedEdge>
{
	private DirectedEdge[] _edges;

	public ICollection<Point> Vertices { get; }

	internal Loop(ICollection<DirectedEdge> edges)
	{
		_edges = ([.. edges]);
		Vertices = _edges.SelectMany(de => Iterate.Over(de.X, de.Y)).
						  Distinct(new PointEqualityComparer()).ToArray();
	}

	public int Count => ((ICollection<DirectedEdge>)_edges).Count;

	public bool IsReadOnly => ((ICollection<DirectedEdge>)_edges).IsReadOnly;

	public void Add(DirectedEdge item)
	{
		((ICollection<DirectedEdge>)_edges).Add(item);
	}

	public void Clear()
	{
		((ICollection<DirectedEdge>)_edges).Clear();
	}

	public bool Contains(DirectedEdge item)
	{
		return ((ICollection<DirectedEdge>)_edges).Contains(item);
	}

	public void CopyTo(DirectedEdge[] array, int arrayIndex)
	{
		((ICollection<DirectedEdge>)_edges).CopyTo(array, arrayIndex);
	}

	public IEnumerator<DirectedEdge> GetEnumerator()
	{
		return ((IEnumerable<DirectedEdge>)_edges).GetEnumerator();
	}

	public bool Remove(DirectedEdge item)
	{
		return ((ICollection<DirectedEdge>)_edges).Remove(item);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _edges.GetEnumerator();
	}
}
