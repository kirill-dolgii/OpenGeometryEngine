using System;

namespace OpenGeometryEngine.Exceptions;

public class ProportionOutsideBoundsException : Exception
{
    public ProportionOutsideBoundsException(string argName) 
        : base($"Provided proportion {argName} must be within [0.0, 1.0] interval.") {}
}