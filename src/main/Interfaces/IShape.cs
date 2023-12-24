using OpenGeometryEngine;

public interface IShape
{
    public IGeometry Geometry { get; }
    public bool IsReversed { get; }
    public T GetGeometry<T>() where T : IGeometry;
}