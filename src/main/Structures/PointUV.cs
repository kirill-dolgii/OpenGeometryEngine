namespace OpenGeometryEngine;

public readonly struct PointUV
{
    public readonly double U;
    public readonly double V;

    public readonly VectorUV Vector;

    public static PointUV Zero => new(0, 0);

    public PointUV(double u, double v)
    {
        (U, V) = (u, v);
        Vector = new(u, v);
    }

    public static PointUV operator +(PointUV pointUv, VectorUV vectorUv)
        => new(pointUv.U + vectorUv.U, pointUv.V + vectorUv.V);
}