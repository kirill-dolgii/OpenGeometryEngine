using OpenGeometryEngine.Extensions;
using System;

namespace OpenGeometryEngine;

/// <summary>
/// Represents a 3D vector with X, Y, and Z components.
/// </summary>
public readonly struct Vector : IEquatable<Vector>
{
    public readonly double X;
    public readonly double Y;
    public readonly double Z;

    public static Vector Zero => new(0, 0, 0);
    public static Vector UnitX => new(1, 0, 0);
    public static Vector UnitY => new(0, 1, 0);
    public static Vector UnitZ => new(0, 0, 1);

    /// <summary>
    /// The magnitude (length) of the vector.
    /// </summary>
    public readonly double Magnitude;

    public UnitVec Unit => new(X / Magnitude, Y / Magnitude, Z / Magnitude);

    /// <summary>
    /// Initializes a new instance of the <see cref="Vector"/> struct
    /// with the specified X, Y, and Z components.
    /// </summary>
    /// <param name="x">The X component of the vector.</param>
    /// <param name="y">The Y component of the vector.</param>
    /// <param name="z">The Z component of the vector.</param>
    public Vector(double x, double y, double z)
    {
        (X, Y, Z) = (x, y, z);
        Magnitude = CaclMagnitude(x, y, z);
    }

    public Vector(UnitVec unit)
    {
        (X, Y, Z) = (unit.X, unit.Y, unit.Z);
        Magnitude = 1.0;
    }

    /// <summary>
    /// Adds two vectors together component-wise.
    /// </summary>
    /// <param name="a">The first vector.</param>
    /// <param name="b">The second vector.</param>
    /// <returns>New vector that represents the result of the addition.</returns>
    public static Vector operator +(Vector a, Vector b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    /// <summary>
    /// Multiplies a vector by a scalar value.
    /// </summary>
    /// <param name="vector">The vector to multiply.</param>
    /// <param name="scalar">The scalar value.</param>
    /// <returns>New vector that represents the result of the multiplication.</returns>
    public static Vector operator *(Vector vector, double scalar) 
        => new(vector.X * scalar, vector.Y * scalar, vector.Z * scalar);

    public static Vector operator /(Vector vector, double scalar) 
        => new (vector.X / scalar, vector.Y / scalar, vector.Z / scalar);

    /// <summary>
    /// Multiplies a vector by a scalar value.
    /// </summary>
    /// <param name="a">The vector to multiply.</param>
    /// <param name="b">The scalar value.</param>
    /// <returns>New vector that represents the result of the subtraction.</returns>
    public static Vector operator -(Vector a, Vector b) => new (a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    public static Vector operator -(Vector a) => a * -1;

    private static double Dot(double x1, double y1, double z1,
                              double x2, double y2, double z2)
        => x1 * x2 + y1 * y2 + z1 * z2;

    /// <summary>
    /// Calculates the dot product of two vectors.
    /// </summary>
    /// <param name="a">The first vector.</param>
    /// <param name="b">The second vector.</param>
    /// <returns>The dot product of the two vectors.</returns>
    public static double Dot(Vector a, Vector b) => Dot(a.X, a.Y, a.Z, b.X, b.Y, b.Z);

    public static double Dot(Vector a, UnitVec b) => Dot(a.X, a.Y, a.Z, b.X, b.Y, b.Z);

    public static double Dot(UnitVec a, UnitVec b) => Dot(a.X, a.Y, a.Z, b.X, b.Y, b.Z);

    public static double Dot(UnitVec a, Vector b) => Dot(a.X, a.Y, a.Z, b.X, b.Y, b.Z);
    
    private static Vector Cross(double x1, double y1, double z1,
                                double x2, double y2, double z2)
    {
        double crossProductX = y1 * z2 - z1 * y2;
        double crossProductY = z1 * x2 - x1 * z2;
        double crossProductZ = x1 * y2 - y1 * x2;
        return new Vector(crossProductX, crossProductY, crossProductZ);
    }

    public static Vector Cross(Vector vector1, Vector vector2)
        => Cross(vector1.X, vector1.Y, vector1.Z, vector2.X, vector2.Y, vector2.Z);

    public static Vector Cross(Vector vector1, UnitVec vector2)
        => Cross(vector1.X, vector1.Y, vector1.Z, vector2.X, vector2.Y, vector2.Z);    

    public static Vector Cross(UnitVec vector1, UnitVec vector2) 
        => Cross(vector1.X, vector1.Y, vector1.Z, vector2.X, vector2.Y, vector2.Z);

    public static Vector Cross(UnitVec vector1, Vector vector2)
        => Cross(vector1.X, vector1.Y, vector1.Z, vector2.X, vector2.Y, vector2.Z);

    /// <summary>
    /// Returns a new vector with the same direction as the original vector,
    /// but with a magnitude of 1 (a unit vector).
    /// </summary>
    /// <returns>A normalized vector.</returns>
    public Vector Normalize() => new (X / Magnitude, Y / Magnitude, Z / Magnitude);

    //public bool IsParallel(Vector other)
    //    => Accuracy.AngleIsZero(Angle(other));

    private static double CaclMagnitude(double x, double y, double z)
        => Math.Sqrt(x * x + y * y + z * z);

    private static double Angle(double x1, double y1, double z1,
                                double x2, double y2, double z2)
    {
        var firstMagnitude = CaclMagnitude(x1, y1, z1);
        var secondMagnitude = CaclMagnitude(x2, y2, z2);
        var rawNum = Dot(x1, y1, z1, x2, y2, z2) / (firstMagnitude * secondMagnitude);
        var checkNum = rawNum.Clamp(-1, 1);
        return Math.Acos(checkNum);
    }

    public static double Angle(Vector vector1, Vector vector2) 
        => Angle(vector1.X, vector1.Y, vector1.Z, vector2.X, vector2.Y, vector2.Z);
    
    public static double Angle(Vector vector1, UnitVec vector2) 
        => Angle(vector1.X, vector1.Y, vector1.Z, vector2.X, vector2.Y, vector2.Z);
    
    public static double Angle(UnitVec vector1, Vector vector2) 
        => Angle(vector1.X, vector1.Y, vector1.Z, vector2.X, vector2.Y, vector2.Z);
    
    public static double Angle(UnitVec vector1, UnitVec vector2) 
        => Angle(vector1.X, vector1.Y, vector1.Z, vector2.X, vector2.Y, vector2.Z);
    
    public static double SignedAngle(double x1, double y1, double z1,
        double x2, double y2, double z2, 
        double axisX, double axisY, double axisZ)
    {
        var cross = Cross(x1, y1, z1, x2, y2, z2);
        var angle = Angle(x1, y1, z1, x2, y2, z2);
        if (Accuracy.LengthIsZero(cross.Magnitude))
        {
            var dot = Vector.Dot(x1, y1, z1, x2, y2, z2);
            return dot > 0 ? angle : -angle;
        }
        var sign = Math.Sign(Dot(cross.X, cross.Y, cross.Z, axisX, axisY, axisZ));
        return sign * angle;
    }

    public static bool TryGetAngle(double x1, double y1, double z1,
                                   double x2, double y2, double z2, 
                                   out double angle)
    {
        if (Accuracy.IsZero(CaclMagnitude(x1, y1, z1)) || Accuracy.IsZero(CaclMagnitude(x2, y2, z2)))
        {
            angle = 0.0;
            return false;
        }
        var dot = Dot(x1, y1, z1, x2, y2, z2);
        var magnitude = Cross(x1, y1, z1, x2, y2, z2).Magnitude;
        if (Accuracy.AngleIsZero(magnitude))
        {
            angle = dot > 0.0 ? 0.0 : Math.PI;
        }
        else
        {
            angle = Math.Atan2(magnitude, dot);
        }
        return true;
    }

    public static bool TryGetAngle(Vector vec1, Vector vec2, out double angle) 
        => TryGetAngle(vec1.X, vec1.Y, vec1.Z, vec2.X, vec2.Y, vec2.Z, out angle);

    public static bool TryGetAngleClockWiseInDir(double x1, double y1, double z1,
                                                 double x2, double y2, double z2,
                                                 double dirX, double dirY, double dirZ,
                                                 out double angle)
    {
        if (!TryGetAngle(x1, y1, z1, x2, y2, z2, out angle)) return false;
        var cross = Cross(x1, y1, z1, x2, y2, z2);
        if (Dot(cross.X, cross.Y, cross.Z, dirX, dirY, dirZ) < 0.0)
        {
            angle = 2 * Math.PI - angle;
        }
        return true;
    }

    public static bool TryGetAngleClockWiseInDir(Vector vec1, Vector vec2, Vector dir, out double angle) 
        => TryGetAngleClockWiseInDir(vec1.X, vec1.Y, vec1.Z, vec2.X, vec2.Y, vec2.Z, dir.X, dir.Y, dir.Z, out angle);

    public static double SignedAngle(Vector vector1, Vector vector2, Vector axis)
        => SignedAngle(vector1.X, vector1.Y, vector1.Z, 
            vector2.X, vector2.Y, vector2.Z, 
            axis.X, axis.Y, axis.Z);    
    
    public static double SignedAngle(Vector vector1, Vector vector2, UnitVec axis)
        => SignedAngle(vector1.X, vector1.Y, vector1.Z, 
            vector2.X, vector2.Y, vector2.Z, 
            axis.X, axis.Y, axis.Z);    
    
    public static double SignedAngle(UnitVec vector1, UnitVec vector2, UnitVec axis)
        => SignedAngle(vector1.X, vector1.Y, vector1.Z, 
            vector2.X, vector2.Y, vector2.Z, 
            axis.X, axis.Y, axis.Z);    
    
    public static double SignedAngle(UnitVec vector1, UnitVec vector2, Vector axis)
        => SignedAngle(vector1.X, vector1.Y, vector1.Z, 
            vector2.X, vector2.Y, vector2.Z, 
            axis.X, axis.Y, axis.Z);    

    public bool Equals(Vector other)
    {
        return Accuracy.CompareWithTolerance(X, other.X, Accuracy.DefaultDoubleTolerance) == 0 &&
               Accuracy.CompareWithTolerance(Y, other.Y, Accuracy.DefaultDoubleTolerance) == 0 &&
               Accuracy.CompareWithTolerance(Z, other.Z, Accuracy.DefaultDoubleTolerance) == 0;
    }

    public override bool Equals(object obj)
    {
        return obj is Vector other && Equals(other);
    }

    public override int GetHashCode() => Hashing.GetHashCode(this);

    public static bool operator ==(Vector left, Vector right)=> left.Equals(right);

    public static bool operator !=(Vector left, Vector right) => !(left == right);

    public override string ToString() => $"Vector [{X:F7}, {Y:F7}, {Z:F7}]";
}