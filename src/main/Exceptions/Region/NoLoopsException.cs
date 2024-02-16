using System;

namespace OpenGeometryEngine.Exceptions;

public class NoLoopsException : Exception
{
    public NoLoopsException() : base("Can't find closed loops") { }
}