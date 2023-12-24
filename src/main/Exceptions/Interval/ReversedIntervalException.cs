using System;

namespace OpenGeometryEngine.Exceptions;

public class ReversedIntervalException : Exception
{
    public ReversedIntervalException()
        : base($"Interval has reversed start and end bounds.")
    { }
}
