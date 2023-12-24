using System;

namespace OpenGeometryEngine;

public class ZeroUnitVectorException : Exception
{
    public ZeroUnitVectorException() 
        : base($"Can't create zero length UnitVector") {}
}