using OpenGeometryEngine;

namespace OpenGeometryEngineTests.IntersectionTests.Bounded;

[TestFixture]
public class LineCircleIntersectionTests
{
    [Test]
    public void LINESEGMENT_INTERSECTS_ARC_COPLANAR_1_POINT()
    {
        var lineSegment = new LineSegment(new Line(new Point(-0.026, 0.009, 0), new UnitVec(0.8, 0.6, 0)),
                                      new Interval(.0, 0.015));
        var circleFrame = new Frame(new Point(-0.022, 0.004, 0),
                                    new Vector(0.928476690885259, 0.371390676354104, 0),
                                    new Vector(-0.371390676354104, 0.928476690885259, 0),
                                    new Vector(0, 0, 1));
        var circleSegment = new Arc(circleFrame, 0.0107703296143, new Interval(.0, 1.89891622181));

        var ip = circleSegment.IntersectCurve(lineSegment).Single();

        Assert.That(Accuracy.EqualLengths(ip.SecondEvaluation.Param, 0.00886256313108));
        Assert.That(Accuracy.EqualAngles(0.899296566986, ip.FirstEvaluation.Param));
        Assert.That(ip.SecondEvaluation.Point,
            Is.EqualTo(new Point(-0.0189099494951335, 0.0143175378786498, 0)));
    }

    [Test]
    public void LINESEGMENT_INTERSECTS_ARC_COPLANAR_2_POINTS()
    {
        var lineSegment = new LineSegment(new Point(0.019, 0.039, 0), new Point(0.055, 0.01, 0));

        var circleFrame = new Frame(new Point(0.033, 0.023, 0), UnitVec.UnitX, UnitVec.UnitY, UnitVec.UnitZ);
        var circleSegment = new Arc(new Point(0.033, 0.023, 0.0),
                                    new Point(0.042544442902576, 0.0101585199575957, 0.0),
                                    new Point(0.0195315665986117, 0.0316372045079625, 0.0),
                                    UnitVec.UnitZ);

        var ips = circleSegment.IntersectCurve(lineSegment);

        Assert.That(ips.Count == 2);
            
        Assert.That(Accuracy.EqualLengths(ips.First().SecondEvaluation.Param, 0.036511480063));
        Assert.That(Accuracy.EqualAngles(0.48542313761, ips.First().FirstEvaluation.Param));
        Assert.That(ips.First().SecondEvaluation.Point,
            Is.EqualTo(new Point(0.0474334578255152, 0.0160952700850016, 0)));

        Assert.That(Accuracy.EqualLengths(ips.ElementAt(1).SecondEvaluation.Param, 0.00536817460321));
        Assert.That(Accuracy.EqualAngles(3.16318807653, ips.ElementAt(1).FirstEvaluation.Param));
        Assert.That(ips.ElementAt(1).SecondEvaluation.Point,
            Is.EqualTo(new Point(0.02318048695689, 0.0356323855069497, 0)));
    }
}
