using System;

namespace OpenGeometryEngine;

public readonly struct Bounds
{
    public readonly double Start;
    public readonly double End;

    public Bounds(double start, double end) => (Start, End) = (start, end);

	public bool ContainsParam(double param) =>
		Math.Abs(param - Start) <= Constants.Tolerance &&
		Math.Abs(param - End) <= Constants.Tolerance;
}