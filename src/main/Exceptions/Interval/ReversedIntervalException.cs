using System;

namespace OpenGeometryEngine.Exceptions.Interval;

public class ReversedIntervalException : Exception
{
    public ReversedIntervalException()
        : base($"Interval has reversed start and end bounds.")
    { }
}
