using OpenGeometryEngine.Extensions;
using System;

namespace OpenGeometryEngine;

/// <summary>
/// Represents a 3D vector with X, Y, and Z components.
/// </summary>
public readonly struct Vector
{
    public readonly double X;
    public readonly double Y;
    public readonly double Z;

    public static Vector Zero => new (0, 0, 0);
    public static Vector UnitX => new (1, 0, 0);
    public static Vector UnitY => new (0, 1, 0);
    public static Vector UnitZ => new (0, 0, 1);
    /// <summary>
    /// The magnitude (length) of the vector.
    /// </summary>
    public readonly double Magnitude;

    /// <summary>
    /// Initializes a new instance of the <see cref="Vector"/> struct with the specified X, Y, and Z components.
    /// </summary>
    /// <param name="x">The X component of the vector.</param>
    /// <param name="y">The Y component of the vector.</param>
    /// <param name="z">The Z component of the vector.</param>
    public Vector(double x, double y, double z)
    {
        (X, Y, Z) = (x, y, z);
        Magnitude = Math.Sqrt(X * X + Y * Y + Z * Z);
    }

    /// <summary>
    /// Adds two vectors together component-wise.
    /// </summary>
    /// <param name="a">The first vector.</param>
    /// <param name="b">The second vector.</param>
    /// <returns>New vector that represents the result of the addition.</returns>
    public static Vector operator +(Vector a, Vector b) => 
        new Vector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    /// <summary>
    /// Multiplies a vector by a scalar value.
    /// </summary>
    /// <param name="vector">The vector to multiply.</param>
    /// <param name="scalar">The scalar value.</param>
    /// <returns>New vector that represents the result of the multiplication.</returns>
    public static Vector operator *(Vector vector, double scalar)
    {
        return new Vector(vector.X * scalar, vector.Y * scalar, vector.Z * scalar);
    }

    /// <summary>
    /// Multiplies a vector by a scalar value.
    /// </summary>
    /// <param name="a">The vector to multiply.</param>
    /// <param name="b">The scalar value.</param>
    /// <returns>New vector that represents the result of the subtraction.</returns>
    public static Vector operator -(Vector a, Vector b) => 
        new Vector(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    public static Vector operator -(Vector a) => a * -1;

    /// <summary>
    /// Calculates the dot product of two vectors.
    /// </summary>
    /// <param name="a">The first vector.</param>
    /// <param name="b">The second vector.</param>
    /// <returns>The dot product of the two vectors.</returns>
    public static double Dot(Vector a, Vector b) => 
        a.X * b.X + a.Y * b.Y + a.Z * b.Z;

    public static Vector Cross(Vector vector1, Vector vector2)
    {
        double crossProductX = vector1.Y * vector2.Z - vector1.Z * vector2.Y;
        double crossProductY = vector1.Z * vector2.X - vector1.X * vector2.Z;
        double crossProductZ = vector1.X * vector2.Y - vector1.Y * vector2.X;

        return new Vector(crossProductX, crossProductY, crossProductZ);
    }

    /// <summary>
    /// Returns a new vector with the same direction as the original vector,
    /// but with a magnitude of 1 (a unit vector).
    /// </summary>
    /// <returns>A normalized vector.</returns>
    public Vector Normalize() => new Vector(X / Magnitude, Y / Magnitude, Z / Magnitude);

    public bool IsParallel(Vector other) =>
        Math.Abs(Vector.Dot(other, this.Normalize()) - other.Magnitude) <= Constants.Tolerance;

    public double Angle(Vector other)
    {
        double dot = Dot(this, other);
        double magnitudeProduct = Magnitude * other.Magnitude;
        // Ensure that the dot product is within the valid range of [-1, 1]
        dot = dot.Clamp(-1, 1);
        // Calculate the angle in radians
        return Math.Acos(dot / magnitudeProduct);
    }

    public double SignedAngle(Vector other, Vector axis)
    {
        var cross = Cross(this, other);
        var sign = Math.Sign(Dot(cross, axis));
        return sign * Angle(other);
    }
}