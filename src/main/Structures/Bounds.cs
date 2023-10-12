namespace OpenGeometryEngine;

public readonly struct Bounds
{
    public static readonly Bounds Unbounded = new(null, null);

    public readonly double? Start;
    public readonly double? End;

    public Bounds(double? start, double? end) => (Start, End) = (start, end);
}

public readonly struct Bounds<T>
{
    public T U { get; }
    public T V { get; }
    public Bounds(T u, T v) => (U, V) = (u, v);
}