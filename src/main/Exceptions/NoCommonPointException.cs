using System;

namespace OpenGeometryEngine.Exceptions;

public class NoCommonPointException : Exception
{
    public NoCommonPointException(string arg1, string arg2) 
        : base($"Can't find common point of specified {arg1} and {arg2}") { }
}