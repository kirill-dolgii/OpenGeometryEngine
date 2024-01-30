namespace OpenGeometryEngine;

public interface IHasGeometry
{
    IGeometry Geometry { get; }
    TGeometry? GetGeometry<TGeometry>() where TGeometry : class, IGeometry;
    bool IsGeometry<TGeometry>() where TGeometry : class, IGeometry;
}