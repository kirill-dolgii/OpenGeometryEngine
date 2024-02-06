using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenGeometryEngine.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<Tuple<T, T>> Pairs<T> (this IEnumerable<T> items, bool closed = false)
    {
        Argument.IsNotNull(nameof(items), items);
        using var enumerator = items.GetEnumerator();
        if (!enumerator.MoveNext()) yield break;
        var first = enumerator.Current;
        while (enumerator.MoveNext())
        {
            yield return Tuple.Create(first, enumerator.Current);
            first = enumerator.Current;
        }
        if (closed) yield return Tuple.Create(first, items.First());
    }
}