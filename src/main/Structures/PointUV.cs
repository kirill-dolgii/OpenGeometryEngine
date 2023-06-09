namespace OpenGeometryEngine;

public readonly struct PointUV
{
    public double U { get; }
    public double V { get; }

    public static PointUV Zero => new PointUV(0, 0);

    public PointUV(double u, double v) => (U, V) = (u, v);
}