using OpenGeometryEngine;

namespace OpenGeometryEngineTests;

[TestFixture]
public class VectorTests
{
    [Test]
    public void VECTOR_ANGLE_90()
    {
        var vec1 = new Vector(1d, 0d, 0d);
        var vec2 = new Vector(0d, 1d, 0d);

        var angle = Vector.Angle(vec1, vec2);

        Assert.That(angle, Is.EqualTo(Math.PI / 2));
    }

    [Test]

    public void VECTOR_ANGLE_0()
    {
        var vec1 = new Vector(1d, 0d, 0d);
        var vec2 = new Vector(1d, 0d, 0d);

        var angle = Vector.Angle(vec1, vec2);

        Assert.That(angle, Is.EqualTo(0));
    }

    [Test]

    public void VECTOR_ANGLE_180()
    {
        var vec1 = new Vector(1d, 0d, 0d);
        var vec2 = new Vector(-1d, 0d, 0d);

        var angle = Vector.Angle(vec1, vec2);

        Assert.That(angle, Is.EqualTo(Math.PI));
    }


    [Test]

    public void VECTOR_SIGNED_ANGLE_180()
    {
        var vec1 = new Vector(1d, 0d, 0d);
        var vec2 = new Vector(-1d, 0d, 0d);
        var axis = new Vector(0d, 1d, 0d);

        var angle1 = Vector.SignedAngle(vec1, vec2, axis);
        var angle2 = Vector.SignedAngle(vec2, vec1, axis);

        Assert.That(angle1, Is.EqualTo(Math.PI));
        Assert.That(angle2, Is.EqualTo(Math.PI * -1));
    }

    [Test]

    public void VECTOR_SIGNED_ANGLE_90()
    {
        var vec1 = new Vector(1d, 0d, 0d);
        var vec2 = new Vector(0d, 1d, 0d);
        var axis = new Vector(0d, 0d, 1d);

        var angle1 = Vector.SignedAngle(vec1, vec2, axis);
        var angle2 = Vector.SignedAngle(vec2, vec1, axis);

        Assert.That(angle1, Is.EqualTo(Math.PI / 2));
        Assert.That(angle2, Is.EqualTo(-Math.PI / 2));
    }

    [Test]

    public void VECTOR_SIGNED_ANGLE_0()
    {
        var vec1 = new Vector(1d, 0d, 0d);
        var vec2 = new Vector(1d, 0d, 0d);
        var axis = new Vector(0d, 1d, 0d);

        var angle1 = Vector.SignedAngle(vec1, vec2, axis);
        var angle2 = Vector.SignedAngle(vec2, vec1, axis);

        Assert.That(angle1, Is.EqualTo(0));
        Assert.That(angle2, Is.EqualTo(0));
    }
}