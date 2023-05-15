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
    public static Vector operator +(Vector a, Vector b)
    {
        return new Vector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }

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
    public static Vector operator -(Vector a, Vector b)
    {
        return new Vector(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }

    /// <summary>
    /// Calculates the dot product of two vectors.
    /// </summary>
    /// <param name="a">The first vector.</param>
    /// <param name="b">The second vector.</param>
    /// <returns>The dot product of the two vectors.</returns>
    public static double Dot(Vector a, Vector b)
    {
        return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
    }

    /// <summary>
    /// Returns a new vector with the same direction as the original vector,
    /// but with a magnitude of 1 (a unit vector).
    /// </summary>
    /// <returns>A normalized vector.</returns>
    public Vector Normalize() => new Vector(X / Magnitude, Y / Magnitude, Z / Magnitude);
}