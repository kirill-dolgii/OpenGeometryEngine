using OpenGeometryEngine;

namespace OpenGeometryEngineTests.IntersectionTests;

[TestFixture]
public class LineLineIntersection
{
    [Test]
    public void PARALLEL_LINES_INTERSECTION()
    {
        var line1 = Line.Create(Point.Origin, Vector.UnitX);
        var line2 = Line.Create(new Point(1, 0, 0), Vector.UnitX);

        var inters = line1.IntersectCurve(line2);

        Assert.That(inters.Count, Is.EqualTo(0));
    }

    [Test]
    public void EQUAL_LINES_INTERSECTION()
    {
        var rand = new Random(0);
        var line1 = Line.Create(new Point(rand.NextDouble(), rand.NextDouble(), rand.NextDouble()), Vector.UnitX);
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
        var line1 = Line.Create(new Point(1, 1, 0), Vector.UnitX);
        var line2 = Line.Create(new Point(0, 0, 0), new Vector(1, 2, 0));

        var inters = line1.IntersectCurve(line2);

        Assert.That(inters.Count, Is.EqualTo(1));
        Assert.That(inters.Single().FirstEvaluation.Param, Is.EqualTo(-0.5));
        Assert.That(Accuracy.CompareLength(inters.Single().SecondEvaluation.Param, 1.11803398875) == 0);
        Assert.That(inters.Single().FirstEvaluation.Point, Is.EqualTo(inters.Single().SecondEvaluation.Point));
    }
}
