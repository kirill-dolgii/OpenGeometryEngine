using OpenGeometryEngine;

[TestFixture]
public class LineTests
{

    [SetUp]
    public void SetUp()
    {
    }

    [Test]
    public void TEST_IS_COINCIDENT_SUCCESSFUL()
    {
        var line1 = new Line(Point.Origin, new Vector(1, 1, 1));
        var line2 = new Line(Point.Origin, new Vector(2, 2, 2));

        Assert.IsTrue(line1.IsCoincident(line2));
    }

    [Test]
    public void TEST_IS_COINCIDENT_UNSUCCESSFUL()
    {
        var line1 = new Line(Point.Origin, new Vector(1, 1, 1));
        var line2 = new Line(Point.Origin, new Vector(1, 2, 2));

        Assert.IsFalse(line1.IsCoincident(line2));
    }

    [Test]
    public void TEST_PROJECT_POINT()
    {
        var line = new Line(Point.Origin, Vector.UnitZ);
        var point = new Point(0, 0, 12);
        var result = line.ProjectPoint(point);
        Assert.IsTrue((result.Point - point).Z <= Constants.Tolerance);
        Assert.That(result.Param, Is.EqualTo(point.Z));
    }

    [Test]
    public void TEST_EVALUATE()
    {
        var line = new Line(Point.Origin, Vector.UnitZ);
        var result = line.Evaluate(10);
        Assert.That(result.Param, Is.EqualTo(10));
    }
}