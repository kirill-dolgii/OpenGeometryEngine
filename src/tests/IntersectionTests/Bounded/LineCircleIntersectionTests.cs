using OpenGeometryEngine;

namespace OpenGeometryEngineTests.IntersectionTests.Bounded;

[TestFixture]
public class LineCircleIntersectionTests
{
    [Test]
    public void LINESEGMENT_INTERSECTS_ARC_COPLANAR_2_POINTS()
    {
        var lineSegment = new LineSegment(new Point(0.019, 0.039, 0), new Point(0.055, 0.01, 0));
        
        var circleFrame = new Frame(new Point(0.033, 0.023, 0), UnitVec.UnitX, UnitVec.UnitY, UnitVec.UnitZ);
        var circleSegment = new Arc(new Point(0.033, 0.023, 0.0), 
                                    new Point(0.042544442902576, 0.0101585199575957, 0.0), 
                                    new Point(0.0195315665986117, 0.0316372045079625, 0.0), 
                                    UnitVec.UnitZ);

        var ip = circleSegment.IntersectCurve(lineSegment).Single();

        Assert.That(Accuracy.EqualLengths(ip.FirstEvaluation.Param, 0.00886256313108));
        Assert.That(Accuracy.EqualAngles(0.899296566986, ip.SecondEvaluation.Param));
        Assert.That(ip.FirstEvaluation.Point,
            Is.EqualTo(new Point(-0.0189099494951335, 0.0143175378786498, 0)));
    }
}
