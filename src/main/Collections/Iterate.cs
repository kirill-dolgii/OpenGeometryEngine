using System.Collections.Generic;
using System.Diagnostics;

namespace OpenGeometryEngine.Collections;

public static class Iterate
{
    [DebuggerStepThrough]
    public static IEnumerable<T> Over<T>(params T[] items)
    {
        foreach (var item in items) yield return item;
    }
}