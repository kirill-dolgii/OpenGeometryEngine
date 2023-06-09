using System;

namespace OpenGeometryEngine;

public readonly struct Bounds : IEquatable<Bounds>
{
    public static readonly Bounds Unbounded = new(double.NegativeInfinity, double.PositiveInfinity);
    
    public readonly double Start;
    public readonly double End;

    public Bounds(double start, double end) => (Start, End) = (start, end);

	public bool ContainsParam(double param) =>
		param - Constants.Tolerance >= Start &&
		param + Constants.Tolerance <= End;

    public bool Equals(Bounds other) =>
        Math.Abs(Start - other.Start) <= Constants.Tolerance && 
        Math.Abs(End - other.End) <= Constants.Tolerance;

    public override bool Equals(object obj) => obj is Bounds other && Equals(other);

    public override int GetHashCode()
    {
        unchecked
        {
            return (Start.GetHashCode() * 397) ^ End.GetHashCode();
        }
    }
}