using System;

namespace OpenGeometryEngine.Exceptions;

public class SingularIntervalException : Exception
{
    public SingularIntervalException() : base("Specified interval has equal start and end bounds") { }
}
