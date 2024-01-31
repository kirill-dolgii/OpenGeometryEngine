using OpenGeometryEngine;

namespace OpenGeometryEngineTests.IntersectionTests.Bounded;

[TestFixture]
public class LineLineIntersectionTests
{
    [Test]
    public void LINESEGMENT_INTERSECTS_LINESEGMENT()
    {
        var lineSegment1 = new LineSegment(new Line(new Point(0, -0.004, 0.027),
                                              new Vector(0, 0.124034734589208, -0.992277876713668).Unit),
                                              new(0.0, 0.0161245154966));
        var lineSegment2 = new LineSegment(new Line(new Point(0, -0.005, 0.015),
                                              new Vector(0, 0.591363663627517, 0.806404995855706).Unit),
            new(0.0, 0.0186010752377));

        var ip = lineSegment1.IntersectCurve(lineSegment2).Single();

        Assert.That(Accuracy.AreEqual(ip.FirstEvaluation.Param, 0.0091580986072905875));
        Assert.That(Accuracy.AreEqual(ip.SecondEvaluation.Param, 0.0036118592694637372));

        Assert.That(ip.FirstEvaluation.Point,
            Is.EqualTo(new Point(0.0, -0.002864077669902917, 0.017912621359223297)));
        Assert.That(ip.FirstEvaluation.Point, Is.EqualTo(ip.SecondEvaluation.Point));

    }

    [Test]
    public void LINESEGMENT_NOT_INTERSECTS_LINESEGMENT()
    {
        var lineSegment1 = new LineSegment(new Line(new Point(0, -0.006, 0.029),
                                                    new Vector(0, 0.447213595499958, -0.894427190999916).Unit),
                                           new(0.0, 0.0111803398875));
        var lineSegment2 = new LineSegment(new Line(new Point(0, 0.007, 0.029),
                                                    new Vector(0, -0.242535625036333, -0.970142500145332).Unit),
                                           new(0.0, 0.0123693168769));

        var inters = lineSegment1.IntersectCurve(lineSegment2);

        Assert.That(inters.Count, Is.EqualTo(0));
    }
}
