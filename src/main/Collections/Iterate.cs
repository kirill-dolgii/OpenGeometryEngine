using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGeometryEngine.Collections;

public static class Iterate
{
    public static IEnumerable<T> Over<T>(params T[] items)
    {
        foreach (var item in items) yield return item;
    }
}