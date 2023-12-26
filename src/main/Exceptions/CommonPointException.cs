using System;

namespace OpenGeometryEngine;

public class CommonPointException : Exception
{
    public CommonPointException(string arg1, string arg2) 
        : base($"Can't find common point of specified {arg1} and {arg2}") { }
}