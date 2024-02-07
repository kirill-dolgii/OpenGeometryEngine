using System;

namespace OpenGeometryEngine.Exceptions;

public class SelfIntersectingRegionException : Exception
{
    public SelfIntersectingRegionException() : base("Region contains self-intersections") {}
}