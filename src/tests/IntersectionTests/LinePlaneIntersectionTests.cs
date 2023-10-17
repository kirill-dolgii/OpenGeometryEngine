using OpenGeometryEngine;
using static OpenGeometryEngine.Intersection.Unbounded.LinePlaneIntersection;

[TestFixture]
public class LinePlaneIntersectionTests
{

    [SetUp]
    public void SetUp()
    {
    }

    [Test]
    public void LINE_INTERSECTS_PLANE()
    {
        var line = Line.Create(new Point(1, 2, 15), new Vector(1, 2, 3));
        var plane = new Plane(Frame.World);
        var intersection = LineIntersectPlane(line, plane);
        
        Assert.That(intersection.Count, Is.EqualTo(1));
        Assert.IsTrue(intersection.First().FirstEvaluation.Point == new Point(-4, -8, 0));

        var line1 = Line.Create(new Point(1, 1, 1), new Vector(0, 1, 0));
        intersection = LineIntersectPlane(line1, plane);
        Assert.That(intersection.Count, Is.EqualTo(0));
    }
}