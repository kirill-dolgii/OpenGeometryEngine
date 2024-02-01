using OpenGeometryEngine;

namespace OpenGeometryEngineTests;

[TestFixture]
public class PolyLineRegionTests : PolyLineRegionTestBase
{
    [Test]
    public void SELF_INTERSECTING_REGION_TEST()
    {
        var points = new[]
        {
            Point.Origin,
            new (1.0, 1.0, 0.0),
            new (0.0, 1.0, 0.0),
            new (1.0, 0.0, 0.0),
        };

        Assert.That(() => new PolyLineRegion(points, Plane.PlaneXY), Throws.Exception);
    }

    [Test]
    public void RECT_AREA_TEST()
    {
        var points = new[]
        {
            Point.Origin,
            new (1.0, 0.0, 0.0),
            new (1.0, 1.0, 0.0),
            new (0.0, 1.0, 0.0),
        };

        var polygon = new OpenGeometryEngine.PolyLineRegion(points, Plane.PlaneXY);
        Assert.That(polygon.Area, Is.EqualTo(1.0));
    }

    [Test]
    public void AREA_TEST_1()
    {
        var file = TestDataDirectory.GetFiles("Concave1.json").Single();
        var data = File.ReadAllText(file.FullName);
        var polygon = FlatPolyLineRegionDeserializer.Deserialize(data);

        Assert.That(polygon.Area, Is.EqualTo(747586.5));
    }

    [Test]
    public void AREA_TEST_2()
    {
        var file = TestDataDirectory.GetFiles("Convex1.json").Single();
        var data = File.ReadAllText(file.FullName);
        var polygon = FlatPolyLineRegionDeserializer.Deserialize(data);

        Assert.That(polygon.Area, Is.EqualTo(133000));
    }

    [Test]
    public void AREA_TEST_3()
    {
        var file = TestDataDirectory.GetFiles("Concave2.json").Single();
        var data = File.ReadAllText(file.FullName);
        var polygon = FlatPolyLineRegionDeserializer.Deserialize(data);

        Assert.That(polygon.Area, Is.EqualTo(109000));
    }

    [Test]
    public void RECT_CONTAINS_POINT_TEST()
    {
        var points = new[]
        {
            Point.Origin,
            new (1.0, 0.0, 0.0),
            new (1.0, 1.0, 0.0),
            new (0.0, 1.0, 0.0),
        };

        var innerPoint = new Point(0.5, 0.5, 0.0);
        var outerPoint = new Point(1.5, 0.5, 0.0);

        var polygon = new OpenGeometryEngine.PolyLineRegion(points, Plane.PlaneXY);

        Assert.That(polygon.ContainsPoint(innerPoint), Is.True);
        Assert.That(polygon.ContainsPoint(outerPoint), Is.False);
        Assert.That(polygon.ContainsPoint(Point.Origin), Is.True);
    }

    [Test]
    public void IS_CONVEX_TEST_TRUE_Convex1()
    {
        var file = TestDataDirectory.GetFiles("Convex1.json").Single();
        var data = File.ReadAllText(file.FullName);
        var polygon = FlatPolyLineRegionDeserializer.Deserialize(data);

        Assert.That(polygon.IsConvex, Is.True);
    }
    
    [Test]
    public void IS_CONVEX_TEST_FALSE_Concave1()
    {
        var file = TestDataDirectory.GetFiles("Concave1.json").Single();
        var data = File.ReadAllText(file.FullName);
        var polygon = FlatPolyLineRegionDeserializer.Deserialize(data);

        Assert.That(polygon.IsConvex, Is.False);
    }
    
    [Test]
    public void IS_CONVEX_TEST_FALSE_Concave2()
    {
        var file = TestDataDirectory.GetFiles("Concave2.json").Single();
        var data = File.ReadAllText(file.FullName);
        var polygon = FlatPolyLineRegionDeserializer.Deserialize(data);

        Assert.That(polygon.IsConvex, Is.False);
    }
}
