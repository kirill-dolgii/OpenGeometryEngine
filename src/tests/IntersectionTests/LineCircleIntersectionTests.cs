using OpenGeometryEngine;
using static OpenGeometryEngine.Intersection.LineCircleIntersection;

namespace OpenGeometryEngineTests.IntersectionTests;

public class LineCircleIntersectionTests
{
    [Test]
    public void LINE_INTERSECT_CIRCLE_2_POINTS()
    {
        var line = new Line(Point.Origin, new Vector(0, 1, 0));
        var circle = Circle.Create(Frame.World, 1);
        var inters = LineIntersectCircle(line, circle);
        Assert.That(inters, Has.Count.EqualTo(2));

        Assert.That(inters.ElementAt(0).FirstEvaluation.Point, Is.EqualTo(new Point(0, 1, 0)));
        Assert.That(inters.ElementAt(0).FirstEvaluation.Param, Is.EqualTo(1));

        Assert.That(inters.ElementAt(1).FirstEvaluation.Point, Is.EqualTo(new Point(0, -1, 0)));
        Assert.That(inters.ElementAt(1).FirstEvaluation.Param, Is.EqualTo(-1));

        Assert.That(inters.ElementAt(0).SecondEvaluation.Point, Is.EqualTo(new Point(0, 1, 0)));
        Assert.That(inters.ElementAt(0).SecondEvaluation.Param, Is.EqualTo(Math.PI / 2));

        Assert.That(inters.ElementAt(1).SecondEvaluation.Point, Is.EqualTo(new Point(0, -1, 0)));
        Assert.That(inters.ElementAt(1).SecondEvaluation.Param, Is.EqualTo(-Math.PI / 2));
    }

    [Test]
    public void LINE_INTERSECT_CIRCLE_1_POINT()
    {
        var line = new Line(new Point(0, 1, 1), new Vector(0, 0, 1));
        var circle = Circle.Create(Frame.World, 1);
        var inters = LineIntersectCircle(line, circle);

        Assert.That(inters, Has.Count.EqualTo(1));

        Assert.That(inters.Single().FirstEvaluation.Point, Is.EqualTo(new Point(0, 1, 0)));
        Assert.That(inters.Single().FirstEvaluation.Param, Is.EqualTo(-1));

        Assert.That(inters.Single().SecondEvaluation.Point, Is.EqualTo(new Point(0, 1, 0)));
        Assert.That(inters.Single().SecondEvaluation.Param, Is.EqualTo(Math.PI / 2));
    }
}