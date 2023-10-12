namespace OpenGeometryEngine;

public interface IGeometry
{
    public bool ContainsPoint(Point point);
    public bool IsCoincident(IGeometry otherGeometry);
    public Point ProjectPoint(Point point);
}