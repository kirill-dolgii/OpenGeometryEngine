using System;

namespace OpenGeometryEngine.Exceptions;

public class DoubleIsNanException : Exception
{
    public DoubleIsNanException(string argName) : base($"{argName} was NaN.") { }
}