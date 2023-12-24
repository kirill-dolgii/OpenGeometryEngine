using System;

namespace OpenGeometryEngine.Exceptions;

public class ZeroUnitVectorException : Exception
{
    public ZeroUnitVectorException() 
        : base($"Can't create zero length UnitVector") {}
}