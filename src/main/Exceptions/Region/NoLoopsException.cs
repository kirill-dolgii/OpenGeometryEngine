using System;

namespace OpenGeometryEngine.Exceptions.Region;

public class NoLoopsException : Exception
{
    public NoLoopsException() : base("Can't find closed loops") { }
}