using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Text;

namespace OpenGeometryEngine.Classes;

public static class Argument
{
    public static void IsNotNull(string argName, object obj)
    {
        if (obj == null) throw new ArgumentNullException(argName);
    }

    internal static Exception IsNotNullException(string argName) => new ArgumentNullException(argName);
}

