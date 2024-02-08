using NUnit.Framework.Constraints;
using OpenGeometryEngine;

namespace OpenGeometryEngineTests.CurveTests.LineSegmentTests;

[TestFixture]
public class LineSegmentTests
{
    [Test]
    public void LINE_SEGMENT_SPLIT_1_POINT()
    {
        var lineSegment = new LineSegment(Point.Origin, new Point(0.01, 0.0, 0.0));
        var splitted = lineSegment.Split(new Line(new Point(0.005, 0.0, 0.0), Frame.World.DirY));

        Assert.That(splitted.Count, Is.EqualTo(2));

        var first = splitted.ElementAt(0);
        var second = splitted.ElementAt(1);

        var p1 = lineSegment.StartPoint;
        var p2 = new Point(0.005, 0.0, 0.0);
        var p3 = lineSegment.EndPoint;

        Assert.That(first.StartPoint, Is.EqualTo(p1));
        Assert.That(first.EndPoint, Is.EqualTo(p2));
        
        Assert.That(second.StartPoint, Is.EqualTo(p2));
        Assert.That(second.EndPoint, Is.EqualTo(p3));

        Assert.That(first.Interval.Start, Is.EqualTo(0.0));
        Assert.That(first.Interval.End, Is.EqualTo(0.005));
        
        Assert.That(second.Interval.Start, Is.EqualTo(0.005));
        Assert.That(second.Interval.End, Is.EqualTo(0.01));
    }

    [Test]
    public void LINE_SEGMENT_SPLIT_COINCIDENT()
    {
        var lineSegment = new LineSegment(Point.Origin, new Point(0.01, 0.0, 0.0));
        var splitted = lineSegment.Split(new Line(Point.Origin, Frame.World.DirX));

        Assert.That(splitted.Count, Is.EqualTo(0));
    }
}