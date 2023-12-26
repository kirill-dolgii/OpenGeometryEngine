//using OpenGeometryEngine;

//namespace OpenGeometryEngineTests.IntersectionTests.Bounded;

//[TestFixture]
//public class LineCircleIntersectionTests
//{
//    [Test]
//    public void LINESEGMENT_INTERSECTS_ARC()
//    {
//        var lineSegment = new LineSegment(new Line(new Point(-0.026, 0.009, 0), new UnitVec(0.8, 0.6, 0)),
//                                      new Interval(.0, 0.015));
//        var circleFrame = new Frame(new Point(-0.022, 0.004, 0),
//                                    new Vector(0.928476690885259, 0.371390676354104, 0),
//                                    new Vector(-0.371390676354104, 0.928476690885259, 0),
//                                    new Vector(0, 0, 1));
//        var circleSegment = new Arc(circleFrame, 0.0107703296143, new Interval(.0, 1.89891622181));

//        var ip = circleSegment.IntersectCurve(lineSegment).Single();

//        Assert.That(Accuracy.EqualLengths(ip.FirstEvaluation.Param, 0.00886256313108));
//        Assert.That(Accuracy.EqualAngles(0.899296566986, ip.SecondEvaluation.Param));
//        Assert.That(ip.FirstEvaluation.Point,
//            Is.EqualTo(new Point(-0.0189099494951335, 0.0143175378786498, 0)));
//    }
//}
