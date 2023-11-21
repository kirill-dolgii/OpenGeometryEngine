using OpenGeometryEngine;

namespace OpenGeometryEngineTests.CurveTests.ArcTests;

[TestFixture]
public class ArcTests
{
    [Test]
    public void ARC_TRANSFORMED_COPY()
    {
        var frameDirX = new Vector(0.687834219796478, 0.351255122458403, 0.635219588035274);
        var frameDirY = new Vector(-0.691074294163565, 0.584586693378039, 0.425058487589251);
        var frameDirZ = new Vector(-0.222036947428355, -0.731353701619171, 0.644842117967364);
        var frameOrigin = new Point(0.12314121, 0.01231231, 0.0031324);
        
        var frame = new Frame(frameOrigin, frameDirX, frameDirY, frameDirZ);
        var arc = Arc.Create(Circle.Create(frame, 1.0), new Interval(.0, 2 * Math.PI));

        var mapping = Matrix.CreateMapping(frame).Inverse();
        var mappedArc = arc.CreateTransformedCopy(mapping);

        var mappedCircle = mappedArc.GetGeometry<Circle>();
        Assert.NotNull(mappedCircle);

        Assert.That(mappedCircle.Frame.Origin, Is.EqualTo(Point.Origin));
        Assert.That(mappedCircle.Frame.DirX, Is.EqualTo(Frame.World.DirX));
        Assert.That(mappedCircle.Frame.DirY, Is.EqualTo(Frame.World.DirY));
        Assert.That(mappedCircle.Frame.DirZ, Is.EqualTo(Frame.World.DirZ));

        Assert.That(mappedArc.StartPoint, Is.EqualTo(new Point(1.0, .0, .0)));
        Assert.That(mappedArc.EndPoint, Is.EqualTo(new Point(1.0, .0, .0)));
        Assert.That(mappedArc.Interval.Span, Is.EqualTo(2 * Math.PI));
    }

    [Test]
    public void ARC_SEGMENT_BOUNDING_BOX()
    {
        var circle = Circle.Create(Frame.World, 1.0);
        var arc = Arc.Create(circle, new Interval(.0, 0.75 * Math.PI));

        var box = arc.GetBoundingBox();

        var minCorner = new Point(arc.EndPoint.X, 0.0, 0.0);
        var maxCorner = new Point(1.0, 1.0, 0.0);

        Assert.That(box.MinCorner, Is.EqualTo(minCorner));
        Assert.That(box.MaxCorner, Is.EqualTo(maxCorner));
    }

    [Test]
    public void ARC_SEGMENT_GENERAL_CASE_BOUNDING_BOX()
    {
        var frameDirX = new Vector(0.687834219796478, 0.351255122458403, 0.635219588035274);
        var frameDirY = new Vector(-0.691074294163565, 0.584586693378039, 0.425058487589251);
        var frameDirZ = new Vector(-0.222036947428355, -0.731353701619171, 0.644842117967364);
        var frameOrigin = new Point(-0.00291090438078573, -0.00958804702822733, 0.00845388016655214);

        var frame = new Frame(frameOrigin, frameDirX, frameDirY, frameDirZ);
        var arc = Arc.Create(Circle.Create(frame, 0.0155241746963), new Interval(.0, 2 * Math.PI));

        var box = arc.GetBoundingBox();

    }
}
