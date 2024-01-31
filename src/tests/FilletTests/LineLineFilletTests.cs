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

        var rounding = LinesegmentFillet.Fillet(first, second, radius);
        var arc = (Arc)rounding.ElementAt(1);

        Assert.That(Accuracy.EqualLengths(arc.Circle.Radius, radius));
        Assert.That(Accuracy.EqualLengths(rounding.First().Length, 0.9));
        Assert.That(Accuracy.EqualLengths(rounding.ElementAt(2).Length, 0.9));
        
        Assert.That(((LineSegment)rounding.First()).StartEndPoints.Contains(new Point(0.1, 0.0, 0.0)), Is.True);
        Assert.That(((LineSegment)rounding.ElementAt(2)).StartEndPoints.Contains(new Point(0.0, 0.1, 0.0)), Is.True);
    }
}
