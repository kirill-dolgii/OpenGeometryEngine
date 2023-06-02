namespace OpenGeometryEngine;

public readonly struct Bounds
{
    public readonly double Start;
    public readonly double End;

    public Bounds(double start, double end) => (Start, End) = (start, end);
}