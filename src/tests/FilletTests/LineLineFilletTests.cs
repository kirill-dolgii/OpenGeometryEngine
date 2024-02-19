using OpenGeometryEngine;
using OpenGeometryEngine.Fillet;

namespace OpenGeometryEngineTests.FilletTests;

[TestFixture]
public class LineLineFilletTests
{
    [Test]
    public void LINE_LINE_FILLET_90_DEG()
    {
        var first = new LineSegment(Point.Origin, new Point(1.0, .0, .0));
        var second = new LineSegment(Point.Origin, new Point(.0, 1.0, .0));

        var radius = .1;

        var rounding = LineSegmentFillet.Fillet(first, second, radius);
        var arc = rounding.Fillet;

        Assert.That(arc, Is.Not.Null);

        Assert.That(Accuracy.EqualLengths(arc.Circle.Radius, radius));
        Assert.That(Accuracy.EqualLengths(rounding.Result.ElementAt(0).Length, 0.9));
        Assert.That(Accuracy.EqualLengths(rounding.Result.ElementAt(2).Length, 0.9));
        
        Assert.That(((LineSegment)rounding.Result.First()).StartEndPoints.Contains(new Point(0.1, 0.0, 0.0)), Is.True);
        Assert.That(((LineSegment)rounding.Result.ElementAt(2)).StartEndPoints.Contains(new Point(0.0, 0.1, 0.0)), Is.True);
    }
}
