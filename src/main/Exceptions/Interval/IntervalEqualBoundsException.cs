using System;

namespace OpenGeometryEngine.Exceptions;

public class IntervalEqualBoundsException : Exception
{
    public IntervalEqualBoundsException(string arg1, string arg2)
        : base($"Can't create interval with equal bounds {arg1} and {arg2}") {}
}
