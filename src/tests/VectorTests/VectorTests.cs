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

        Assert.That(angle, Is.Zero);
    }

    [Test]

    public void VECTOR_ANGLE_180()
    {
        var vec1 = new Vector(1d, 0d, 0d);
        var vec2 = new Vector(-1d, 0d, 0d);

        var angle = Vector.Angle(vec1, vec2);

        Assert.That(angle, Is.EqualTo(Math.PI));
    }

#region TRY_GET_ANGLE

    [Test]
    public void VECTOR_TRY_GET_ANGLE_ZERO_VECTOR()
    {
        var vec1 = Vector.Zero;
        var vec2 = new Vector(1.0, 0.0, 0.0);

        Assert.That(Vector.TryGetAngle(vec1, vec2, out var angle), Is.False);
        Assert.That(angle, Is.Zero);
    }

    [Test]
    public void VECTOR_TRY_GET_ANGLE_0()
    {
        var vec1 = new Vector(1.0, 0.0, 0.0);
        var vec2 = new Vector(1.0, 0.0, 0.0);

        Assert.That(Vector.TryGetAngle(vec1, vec2, out var angle), Is.True);
        Assert.That(angle, Is.Zero);
    }

    [Test]
    public void VECTOR_TRY_GET_ANGLE_90()
    {
        var vec1 = new Vector(1.0, 0.0, 0.0);
        var vec2 = new Vector(0.0, 1.0, 0.0);

        Assert.That(Vector.TryGetAngle(vec1, vec2, out var angle), Is.True);
        Assert.That(angle, Is.EqualTo(Math.PI / 2));
    }

    [Test]
    public void VECTOR_TRY_GET_ANGLE_180()
    {
        var vec1 = new Vector(1.0, 0.0, 0.0);
        var vec2 = new Vector(-1.0, 0.0, 0.0);

        Assert.That(Vector.TryGetAngle(vec1, vec2, out var angle), Is.True);
        Assert.That(angle, Is.EqualTo(Math.PI));
    }

#endregion 

#region TRY_GET_ANGLE_CLOCKWISE_IN_DIR

    [Test]
    public void TRY_GET_ANGLE_CLOCKWISE_IN_DIR_0()
    {
        var vec1 = new Vector(1.0, 0.0, 0.0);
        var vec2 = new Vector(1.0, 0.0, 0.0);

        Assert.That(Vector.TryGetAngleClockWiseInDir(vec1, vec2, Vector.UnitZ, out var angle), Is.True);
        Assert.That(angle, Is.Zero);
    }

    [Test]
    public void TRY_GET_ANGLE_CLOCKWISE_IN_DIR_90()
    {
        var vec1 = new Vector(1.0, 0.0, 0.0);
        var vec2 = new Vector(0.0, 1.0, 0.0);

        Assert.That(Vector.TryGetAngleClockWiseInDir(vec1, vec2, Vector.UnitZ, out var angle), Is.True);
        Assert.That(angle, Is.EqualTo(Math.PI / 2));
    }

    [Test]
    public void TRY_GET_ANGLE_CLOCKWISE_IN_DIR_180()
    {
        var vec1 = new Vector(1.0, 0.0, 0.0);
        var vec2 = new Vector(-1.0, 0.0, 0.0);

        Assert.That(Vector.TryGetAngleClockWiseInDir(vec1, vec2, Vector.UnitZ, out var angle), Is.True);
        Assert.That(angle, Is.EqualTo(Math.PI));
    }

    [Test]
    public void TRY_GET_ANGLE_CLOCKWISE_IN_DIR_270()
    {
        var vec1 = new Vector(1.0, 0.0, 0.0);
        var vec2 = new Vector(0.0, -1.0, 0.0);

        Assert.That(Vector.TryGetAngleClockWiseInDir(vec1, vec2, Vector.UnitZ, out var angle), Is.True);
        Assert.That(angle, Is.EqualTo(1.5 * Math.PI));
    }

#endregion

}