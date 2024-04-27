using System;
using System.Collections.Generic;

namespace OpenGeometryEngine.Misc;

internal class FunctionalEqualityComparer<T> : IEqualityComparer<T>
{
	private Func<T, T, bool> _equalityFunc;
	private Func<T, int>? _hashCodeFunc;

	public FunctionalEqualityComparer(Func<T, T, bool> equalityFunc, 
									  Func<T, int>? hashCodeFunc)
	{
		Argument.IsNotNull(nameof(equalityFunc), equalityFunc);
		_equalityFunc = equalityFunc;
		_hashCodeFunc = hashCodeFunc;
	}

	public bool Equals(T x, T y)
	{
		if (ReferenceEquals(x, y)) return true;
		if (ReferenceEquals(x, null)) return false;
		if (ReferenceEquals(y, null)) return false;
		if (x.GetType() != y.GetType()) return false;
		return _equalityFunc(x, y);
	}

	public int GetHashCode(T obj)
	{
		if (ReferenceEquals(obj, null)) throw new ArgumentNullException(nameof(obj));
		if (_hashCodeFunc != null)
		{
			return _hashCodeFunc(obj);
		}
		return obj.GetHashCode();
	}
}
