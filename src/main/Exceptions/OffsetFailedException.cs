using System;

namespace OpenGeometryEngine.Exceptions;

internal class OffsetFailedException : Exception
{
    public OffsetFailedException(double offset) : base($"Failed to create {offset:F4} offset.") {}
}
