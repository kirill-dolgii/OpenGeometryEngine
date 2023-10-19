using System;

namespace OpenGeometryEngine;

/// <summary>
/// Represents a point in 3D space with X, Y, and Z coordinates.
/// </summary>
public readonly struct Point : IEquatable<Point>
{
    public readonly double X; 
    public readonly double Y; 
    public readonly double Z;

    /// <summary>
    /// Radius vector representation of point.
    /// </summary>
    public Vector Vector { get; }

    public static Point Origin => new (0, 0, 0);

    /// <summary>
    /// Initializes a new instance of the Origin struct with the specified X, Y, and Z coordinates.
    /// </summary>
    /// <param name="x">The X-coordinate of the point.</param>
    /// <param name="y">The Y-coordinate of the point.</param>
    /// <param name="z">The Z-coordinate of the point.</param>
    public Point (double x, double y, double z) 
        => (X, Y, Z, Vector) = (x, y, z, new Vector(x, y, z));

    /// <summary>
    /// Subtracts a vector from the point, resulting in a new point.
    /// </summary>
    /// <param name="point">The point to subtract the vector from.</param>
    /// <param name="vector">The vector to subtract from the point.</param>
    /// <returns>A new point resulting from the subtraction.</returns>
    public static Point operator -(Point point, Vector vector) => 
        new(point.X - vector.X, point.Y - vector.Y, point.Z - vector.Z);

    /// <summary>
    /// Adds a vector to the point, resulting in a new point.
    /// </summary>
    /// <param name="point">The point to add the vector to.</param>
    /// <param name="vector">The vector to add to the point.</param>
    /// <returns>A new point resulting from the addition.</returns>
    public static Point operator +(Point point, Vector vector) => 
        new (point.X + vector.X, point.Y + vector.Y, point.Z + vector.Z);

    public static Point operator *(Point point, double scale)
        => new(point.X * scale, point.Y * scale, point.Z * scale);

    public static Point operator /(Point point, double scale)
    {
        if (scale == 0) throw new DivideByZeroException("scale was zero.");
        return new Point(point.X / scale, point.Y / scale, point.Z / scale);
    }

    /// <summary>
    /// Subtracts one point from another, resulting in a new vector.
    /// </summary>
    /// <param name="point1">The point to subtract from.</param>
    /// <param name="point2">The point to subtract.</param>
    /// <returns>A new vector resulting from the subtraction.</returns>
    public static Vector operator -(Point point1, Point point2) =>
        new (point1.X - point2.X, point1.Y - point2.Y, point1.Z - point2.Z);

    public bool Equals(Point other)
    {
        return Accuracy.EqualLengths(X, other.X) &&
               Accuracy.EqualLengths(Y, other.Y) &&
               Accuracy.EqualLengths(Z, other.Z);
    }

    public override bool Equals(object obj)
    {
        return obj is Point other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = X.GetHashCode();
            hashCode = (hashCode * 397) ^ Y.GetHashCode();
            hashCode = (hashCode * 397) ^ Z.GetHashCode();
            return hashCode;
        }
    }

    public static bool operator ==(Point a, Point b) => a.Equals(b);

    public static bool operator !=(Point a, Point b) => !(a == b);
}