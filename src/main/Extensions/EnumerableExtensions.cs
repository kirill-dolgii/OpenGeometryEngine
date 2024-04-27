using System.Collections.Generic;
using System.Linq;

namespace OpenGeometryEngine.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<(T First, T Second)> Pairs<T> (this IEnumerable<T> items, bool closed = false)
    {
        Argument.IsNotNull(nameof(items), items);
        using var enumerator = items.GetEnumerator();
        if (!enumerator.MoveNext()) yield break;
        var first = enumerator.Current;
        while (enumerator.MoveNext())
        {
            yield return (first, enumerator.Current);
            first = enumerator.Current;
        }
        if (closed) yield return (first, items.First());
    }
}