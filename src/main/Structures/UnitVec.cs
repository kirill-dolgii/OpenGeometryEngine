using System;
using OpenGeometryEngine;
using OpenGeometryEngine.Exceptions;

public readonly struct UnitVec : IEquatable<UnitVec>
{
    public readonly double X;
    public readonly double Y;
    public readonly double Z;

    public UnitVec(double x, double y, double z)
    {
        var magnitude = x * x + y * y + z * z;
        if (Accuracy.CompareWithTolerance(magnitude, 0.0, Accuracy.DefaultDoubleTolerance) == 0)
            throw new ZeroUnitVectorException();
        if (magnitude == 1.0)
        {
            X = x; Y = y; Z = z;
        }
        else
        {
            var sqrtMagnitude = Math.Sqrt(magnitude);
            X = x / sqrtMagnitude; 
            Y = y / sqrtMagnitude; 
            Z = z / sqrtMagnitude;
        }
    }

    public UnitVec(Vector vector) : this(vector.X, vector.Y, vector.Z) {}

    public static bool AreParallel(UnitVec unit1, UnitVec unit2)
    {
        var cross = Vector.Cross(unit1, unit2).Magnitude;
        return Accuracy.CompareWithTolerance(cross, 0.0, Accuracy.AngularTolerance) == 0;
    }

    public static Vector operator *(UnitVec a, double scale) 
        => new Vector(a.X * scale, a.Y * scale, a.Z * scale);

    public bool Equals(UnitVec other)
    {
        return Accuracy.CompareWithTolerance(X, other.X, Accuracy.DefaultDoubleTolerance) == 0 &&
               Accuracy.CompareWithTolerance(Y, other.Y, Accuracy.DefaultDoubleTolerance) == 0 &&
               Accuracy.CompareWithTolerance(Z, other.Z, Accuracy.DefaultDoubleTolerance) == 0;
    }

    public override bool Equals(object obj)
    {
        return obj is UnitVec other && Equals(other);
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
}