using OpenGeometryEngine;

namespace OpenGeometryEngineTests;

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
        var line1 = new Line(Point.Origin, new Vector(1, 1, 1).Unit);
        var line2 = new Line(Point.Origin, new Vector(2, 2, 2).Unit);

        Assert.That(Line.AreCoincident(line1, line2), Is.True);
    }

    [Test]
    public void TEST_IS_COINCIDENT_UNSUCCESSFUL()
    {
        var line1 = new Line(Point.Origin, new Vector(1, 1, 1).Unit);
        var line2 = new Line(Point.Origin, new Vector(1, 2, 2).Unit);

        Assert.That(Line.AreCoincident(line1, line2), Is.False);
    }

    [Test]
    public void TEST_PROJECT_POINT()
    {
        var line = new Line(Point.Origin, Vector.UnitZ.Unit);
        var point = new Point(0, 0, 12);
        var result = line.ProjectPoint(point);
        Assert.Multiple(() =>
        {
            Assert.That(Accuracy.CompareWithTolerance((result.Point - point).Z, 0.0, Accuracy.LinearTolerance), Is.EqualTo(0));
            Assert.That(result.Param, Is.EqualTo(point.Z));
        });
    }

    [Test]
    public void TEST_EVALUATE()
    {
        var line = new Line(Point.Origin, Vector.UnitZ.Unit);
        var result = line.Evaluate(10);
        Assert.That(result.Param, Is.EqualTo(10));
    }
}