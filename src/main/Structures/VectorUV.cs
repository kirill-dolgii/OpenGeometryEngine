using System;

namespace OpenGeometryEngine;

public readonly struct VectorUV
{
    public readonly double U;
    public readonly double V;

    public readonly double Magnitude;

    public VectorUV(double u, double v)
    { 
        U = u; V = v;
        Magnitude = Math.Sqrt(Math.Pow(u, 2) + Math.Pow(v, 2));
    }

    public static VectorUV operator +(VectorUV a, VectorUV b) =>
        new(a.U + b.U, a.V + b.V);

    public static VectorUV operator -(VectorUV a, VectorUV b) =>
        new(a.U - b.U, a.V - b.V);

    public static VectorUV operator *(VectorUV vector, double scalar) 
        => new(vector.U * scalar, vector.V * scalar);

    public static VectorUV operator -(VectorUV a) => a * -1;
}