using System;

namespace OpenGeometryEngine.Exceptions;

public class PointOffRegionPlaneException : Exception
{
    public PointOffRegionPlaneException() : base("Point is not contained by the plane of the region") { }
}