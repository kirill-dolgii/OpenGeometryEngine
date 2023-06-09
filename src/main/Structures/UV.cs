namespace OpenGeometryEngine;

public readonly struct UV<T>
{
    public T U { get; }
    public T V { get; }
    internal UV(T u, T v) => (U, V) = (u, v);
}