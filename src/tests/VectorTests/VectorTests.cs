using OpenGeometryEngine;

namespace OpenGeometryEngineTests;

[TestFixture]
public class VectorTests
{
    
#region ANGLE
    [Test]
    public void VECTOR_ANGLE_0()
    {
        var vec1 = new Vector(1d, 0d, 0d);
        var vec2 = new Vector(1d, 0d, 0d);

        var angle = Vector.Angle(vec1, vec2);

        Assert.That(angle, Is.Zero);
    }

    [Test]
    public void VECTOR_ANGLE_90()
    {
        var vec1 = new Vector(1d, 0d, 0d);
        var vec2 = new Vector(0d, 1d, 0d);

        var angle = Vector.Angle(vec1, vec2);

        Assert.That(angle, Is.EqualTo(Math.PI / 2));
    }

    [Test]
    public void VECTOR_ANGLE_180()
    {
        var vec1 = new Vector(1d, 0d, 0d);
        var vec2 = new Vector(-1d, 0d, 0d);

        var angle = Vector.Angle(vec1, vec2);

        Assert.That(angle, Is.EqualTo(Math.PI));
    }
#endregion

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

#region TRY_GET_SIGNED_ANGLE_iN_DIR

// Case when one of the vectors has zero magnitude. Expected output is false.
[Test]
public void TryGetSignedAngleInDir_ZeroMagnitudeVector_ReturnsFalse()
{
    Assert.IsFalse(Vector.TryGetSignedAngleInDir(0, 0, 0, 1, 1, 1, 1, 1, 1, out var angle));
    Assert.That(angle, Is.Zero);
}

// Case when both vectors and direction vector are the same. 
// Because they both point to the same direction, the angle is 0. Expected output is true.
[Test]
public void TryGetSignedAngleInDir_SameVectors_ReturnsTrueAndZeroAngle()
{
    Assert.IsTrue(Vector.TryGetSignedAngleInDir(1, 1, 1, 1, 1, 1, 1, 1, 1, out var angle));
    Assert.That(angle, Is.Zero);
}

// Case when vectors are opposite (180 degrees). Expected output is true.
[Test]
public void TryGetSignedAngleInDir_OppositeVectors_ReturnsTrueAndPiAngle()
{
    Assert.IsTrue(Vector.TryGetSignedAngleInDir(1, 2, 3, -1, -2, -3, 1, 1, 1, out var angle));
    Assert.IsTrue(Accuracy.EqualAngles(Math.PI, angle));
}

// Case when vectors are perpendicular. Because they form a right angle, 
// the value of the angle depends on the direction vector. Expected output is true.
[Test]
public void TryGetSignedAngleInDir_PerpendicularVectors_ReturnsTrueAndHalfPiAngle()
{
    Assert.IsTrue(Vector.TryGetSignedAngleInDir(1, 0, 0, 0, 1, 0, 0, 0, 1, out var angle));
    Assert.IsTrue(Accuracy.EqualAngles(Math.PI / 2, angle));
}

// Case when vectors are perpendicular and the direction is parallel to one of the vectors. Expected output is false.
[Test]
public void TryGetSignedAngleInDir_PerpendicularVectors_ReturnsFalse()
{
    Assert.IsFalse(Vector.TryGetSignedAngleInDir(1, 0, 0, 0, 1, 0, 1, 0, 0, out var angle));
    Assert.That(angle, Is.Zero);
}

#endregion

}