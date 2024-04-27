using OpenGeometryEngine.Collections;
using OpenGeometryEngine.Misc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OpenGeometryEngine.BRep;

internal class Wire : ICollection<Fin>
{
	internal Fin[] _fins;
	internal Vertex[] _vertices;

	public Wire(ICollection<Fin> fins)
	{
		Argument.IsNotNull(nameof(fins), fins);
		Vertex? prev = null;
		var wireFins = new List<Fin>();
		foreach (Fin f in fins)
		{
			if (prev == null || f.X == prev)
			{
				wireFins.Add(f);
			}
			else
			{
				throw new System.Exception("not ordered fins");
			}
			prev = f.Y;
		}
		_fins = wireFins.ToArray();
		_vertices = wireFins.SelectMany(w => Iterate.Over(w.X, w.Y)).Distinct().ToArray();
	}

	public int Count => ((ICollection<Fin>)_fins).Count;

	public bool IsReadOnly => ((ICollection<Fin>)_fins).IsReadOnly;

	public void Add(Fin item)
	{
		((ICollection<Fin>)_fins).Add(item);
	}

	public void Clear()
	{
		((ICollection<Fin>)_fins).Clear();
	}

	public bool Contains(Fin item)
	{
		return ((ICollection<Fin>)_fins).Contains(item);
	}

	public void CopyTo(Fin[] array, int arrayIndex)
	{
		((ICollection<Fin>)_fins).CopyTo(array, arrayIndex);
	}

	public IEnumerator<Fin> GetEnumerator()
	{
		return ((IEnumerable<Fin>)_fins).GetEnumerator();
	}

	public bool Remove(Fin item)
	{
		return ((ICollection<Fin>)_fins).Remove(item);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _fins.GetEnumerator();
	}
}

internal class WireEqualtiComparer : FunctionalEqualityComparer<Wire>
{
	public static WireEqualtiComparer Default => new();

	private WireEqualtiComparer() : 
		base((x, y) => x._vertices.Intersect(y._vertices).Count() == x._vertices.Length, x => 0)
	{ }
}