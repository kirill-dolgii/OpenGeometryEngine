using Newtonsoft.Json;
using OpenGeometryEngine;

namespace OpenGeometryEngineTests;

public record Point2D(double x, double y);

public static class FlatPolyLineRegionDeserializer
{
    public static PolyLineRegion Deserialize(string json)
    {
        Argument.IsNotNull(json, nameof(json));
        var points2d = JsonConvert.DeserializeObject<Point2D[]>(json);
        var points = points2d.Select(p => new Point(p.x, p.y, 0.0)).ToArray();
        var polygon = new PolyLineRegion(points, Plane.PlaneXY);
        return polygon;
    }
}