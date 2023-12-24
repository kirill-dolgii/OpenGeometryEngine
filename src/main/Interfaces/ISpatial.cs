namespace OpenGeometryEngine;

public interface ISpatial
{
    bool ContainsPoint(Point point);
    Point ProjectPoint(Point point);
}