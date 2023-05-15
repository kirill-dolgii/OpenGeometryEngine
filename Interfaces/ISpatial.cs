namespace OpenGeometryEngine;

public interface ISpatial
{
    public bool ContainsPoint(Point point);
    public Point ProjectPoint(Point point);
}