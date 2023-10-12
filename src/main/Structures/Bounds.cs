using System;

namespace OpenGeometryEngine;

public readonly struct Bounds : IEquatable<Bounds>
{
    public static readonly Bounds Unbounded = new(null, null);

    public readonly double? Start;
    public readonly double? End;

    public Bounds(double? start, double? end) => (Start, End) = (start, end);

    public bool ContainsParam(double param) =>
        param - Constants.Tolerance >= Start &&
        param + Constants.Tolerance <= End;

    public bool Equals(Bounds other)
    {
        if (Start.HasValue && other.Start.HasValue)
        {
            var startAreEqual = Math.Abs(Start.Value - other.Start.Value) <= Constants.Tolerance;
            if (End.HasValue && other.End.HasValue)
                return startAreEqual && Math.Abs(End.Value - other.End.Value) <= Constants.Tolerance;

        }
        return false;
    }

    public override bool Equals(object obj) => obj is Bounds other && Equals(other);

    public override int GetHashCode()
    {
        unchecked
        {
            return Start.GetHashCode() * 397 ^ End.GetHashCode();
        }
    }
}

public readonly struct Bounds<T>
{
    public T U { get; }
    public T V { get; }
    public Bounds(T u, T v) => (U, V) = (u, v);
}