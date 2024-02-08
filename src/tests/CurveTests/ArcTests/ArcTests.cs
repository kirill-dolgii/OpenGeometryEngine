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
        var arc = new Arc(frame, 1.0, new Interval(.0, 2 * Math.PI));

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
        var arc = new Arc(Frame.World, 1.0, new Interval(.0, 0.75 * Math.PI));

        var box = arc.GetBoundingBox();

        var minCorner = new Point(arc.EndPoint.X, 0.0, 0.0);
        var maxCorner = new Point(1.0, 1.0, 0.0);

        Assert.That(box.MinCorner, Is.EqualTo(minCorner));
        Assert.That(box.MaxCorner, Is.EqualTo(maxCorner));
    }

    //[Test]
    //public void ARC_SEGMENT_GENERAL_CASE_BOUNDING_BOX()
    //{
    //var frameDirX = new Vector(0.687834219796478, 0.351255122458403, 0.635219588035274);
    //var frameDirY = new Vector(-0.691074294163565, 0.584586693378039, 0.425058487589251);
    //var frameDirZ = new Vector(-0.222036947428355, -0.731353701619171, 0.644842117967364);
    //var frameOrigin = new Point(-0.00291090438078573, -0.00958804702822733, 0.00845388016655214);

    //var frame = new Frame(frameOrigin, frameDirX, frameDirY, frameDirZ);
    //var arc = new Arc(frame, 0.0155241746963, new Interval(.0, 2 * Math.PI));

    //var box = arc.GetBoundingBox();

    //}

    #region Split Tests

    [Test]
    public void CLOSED_ARC_SPLIT_2_POINTS()
    {
        var arc = new Arc(Frame.World, 0.01, new Interval(0.0, 2 * Math.PI));
        var splitted = arc.Split(new Line(Point.Origin, Frame.World.DirX));

        Assert.That(splitted.Count, Is.EqualTo(2));

        var first = splitted.ElementAt(0);
        var second = splitted.ElementAt(1);

        var p1 = new Point(0.01, 0.0, 0.0);
        var p2 = new Point(-0.01, 0.0, 0.0);

        Assert.That(first.StartPoint, Is.EqualTo(p1));
        Assert.That(first.EndPoint, Is.EqualTo(p2));

        Assert.That(second.StartPoint, Is.EqualTo(p2));
        Assert.That(second.EndPoint, Is.EqualTo(p1));

        Assert.That(first.Interval.Start, Is.EqualTo(0.0));
        Assert.That(first.Interval.End, Is.EqualTo(Math.PI));

        Assert.That(second.Interval.Start, Is.EqualTo(Math.PI));
        Assert.That(second.Interval.End, Is.EqualTo(2 * Math.PI));
    }

    [Test]
    public void NOT_CLOSED_ARC_SPLIT_2_POINTS_1_ON_BOUND()
    {
        var arc = new Arc(Frame.World, 0.01, new Interval(0.0, 1.5 * Math.PI));
        var splitted = arc.Split(new Line(Point.Origin, Frame.World.DirX));

        Assert.That(splitted.Count, Is.EqualTo(2));

        var first = splitted.ElementAt(0);
        var second = splitted.ElementAt(1);

        var p1 = new Point(0.01, 0.0, 0.0);
        var p2 = new Point(-0.01, 0.0, 0.0);
        var p3 = arc.Circle.Evaluate(1.5 * Math.PI).Point;

        Assert.That(first.StartPoint, Is.EqualTo(p1));
        Assert.That(first.EndPoint, Is.EqualTo(p2));

        Assert.That(second.StartPoint, Is.EqualTo(p2));
        Assert.That(second.EndPoint, Is.EqualTo(p3));

        Assert.That(first.Interval.Start, Is.EqualTo(0.0));
        Assert.That(first.Interval.End, Is.EqualTo(Math.PI));

        Assert.That(second.Interval.Start, Is.EqualTo(Math.PI));
        Assert.That(second.Interval.End, Is.EqualTo(1.5 * Math.PI));
    }

    [Test]
    public void NOT_CLOSED_ARC_SPLIT_2_POINTS_2_ON_BOUNDS()
    {
        var arc = new Arc(Frame.World, 0.01, new Interval(0.0, 1 * Math.PI));
        var splitted = arc.Split(new Line(Point.Origin, Frame.World.DirX));

        Assert.That(splitted.Count, Is.EqualTo(0));
    }

    [Test]
    public void NOT_CLOSED_ARC_SPLIT_2_POINTS()
    {
        var arc = new Arc(Frame.World, 0.01, new Interval(0.0, 1.9 * Math.PI));
        var splitted = arc.Split(new Line(Point.Origin, Frame.World.DirY));

        Assert.That(splitted.Count, Is.EqualTo(3));

        var first = splitted.ElementAt(0);
        var second = splitted.ElementAt(1);
        var third = splitted.ElementAt(2);
        
        var p1 = arc.Circle.Evaluate(0).Point;
        var p2 = arc.Circle.Evaluate(0.5 * Math.PI).Point;
        var p3 = arc.Circle.Evaluate(1.5 * Math.PI).Point;
        var p4 = arc.Circle.Evaluate(1.9 * Math.PI).Point;

        Assert.That(first.StartPoint, Is.EqualTo(p1));
        Assert.That(first.EndPoint, Is.EqualTo(p2));

        Assert.That(second.StartPoint, Is.EqualTo(p2));
        Assert.That(second.EndPoint, Is.EqualTo(p3));
        
        Assert.That(third.StartPoint, Is.EqualTo(p3));
        Assert.That(third.EndPoint, Is.EqualTo(p4));

        Assert.That(first.Interval.Start, Is.EqualTo(0.0));
        Assert.That(first.Interval.End, Is.EqualTo(0.5 * Math.PI));

        Assert.That(second.Interval.Start, Is.EqualTo(0.5 *  Math.PI));
        Assert.That(second.Interval.End, Is.EqualTo(1.5 * Math.PI));

        Assert.That(third.Interval.Start, Is.EqualTo(1.5 * Math.PI));
        Assert.That(third.Interval.End, Is.EqualTo(1.9 * Math.PI));
    }

    [Test]
    public void NOT_CLOSED_ARC_SPLIT_1_POINT()
    {
        var arc = new Arc(Frame.World, 0.01, new Interval(0.0, Math.PI));
        var splitted = arc.Split(new Line(Point.Origin, Frame.World.DirY));

        Assert.That(splitted.Count, Is.EqualTo(2));

        var first = splitted.First();
        var second = splitted.ElementAt(1);

        var p1 = arc.Circle.Evaluate(0).Point;
        var p2 = arc.Circle.Evaluate(0.5 * Math.PI).Point;
        var p3 = arc.Circle.Evaluate(Math.PI).Point;

        Assert.That(first.StartPoint, Is.EqualTo(p1));
        Assert.That(first.EndPoint, Is.EqualTo(p2));
        
        Assert.That(second.StartPoint, Is.EqualTo(p2));
        Assert.That(second.EndPoint, Is.EqualTo(p3));

        Assert.That(first.Interval.Start, Is.EqualTo(0.0));
        Assert.That(first.Interval.End, Is.EqualTo(0.5 * Math.PI));

        Assert.That(second.Interval.Start, Is.EqualTo(0.5 * Math.PI));
        Assert.That(second.Interval.End, Is.EqualTo(Math.PI));
    }

    #endregion
}
