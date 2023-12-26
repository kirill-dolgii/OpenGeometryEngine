using OpenGeometryEngine;

namespace OpenGeometryEngineTests.IntersectionTests.Unbounded;

[TestFixture]
public class LineLineIntersection
{
    [Test]
    public void PARALLEL_LINES_INTERSECTION()
    {
        var line1 = new Line(Point.Origin, Vector.UnitX.Unit);
        var line2 =  new Line(new Point(1, 0, 0), Vector.UnitX.Unit);

        var inters = line1.IntersectCurve(line2);

        Assert.That(inters.Count, Is.EqualTo(0));
    }

    [Test]
    public void EQUAL_LINES_INTERSECTION()
    {
        var rand = new Random(0);
        var line1 = new Line(new Point(rand.NextDouble(), rand.NextDouble(), rand.NextDouble()), Vector.UnitX.Unit);
        var line2 = line1;

        var inters = line1.IntersectCurve(line2);

        Assert.That(inters.Count, Is.EqualTo(1));
        Assert.That(inters.Single().FirstEvaluation.Param, Is.EqualTo(0));
        Assert.That(inters.Single().SecondEvaluation.Param, Is.EqualTo(0));
        Assert.That(inters.Single().FirstEvaluation.Point, Is.EqualTo(inters.Single().SecondEvaluation.Point));
    }

    [Test]
    public void LINES_INTERSECTION()
    {
        var line1 = new Line(new Point(1, 1, 0), Vector.UnitX.Unit);
        var line2 = new Line(new Point(0, 0, 0), new Vector(1, 2, 0).Unit);

        var inters = line1.IntersectCurve(line2);

        Assert.That(inters.Count, Is.EqualTo(1));
        Assert.That(inters.Single().FirstEvaluation.Param, Is.EqualTo(-0.5));
        Assert.That(Accuracy.CompareWithTolerance(inters.Single().SecondEvaluation.Param, 1.11803398875, Accuracy.LinearTolerance) == 0);
        Assert.That(inters.Single().FirstEvaluation.Point, Is.EqualTo(inters.Single().SecondEvaluation.Point));
    }
}
