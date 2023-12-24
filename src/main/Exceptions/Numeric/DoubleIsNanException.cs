using System;

namespace OpenGeometryEngine;

public class DoubleIsNanException : Exception
{
    public DoubleIsNanException(string argName) : base($"{argName} was NaN.") { }
}