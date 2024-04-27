using OpenGeometryEngine.BRep;

namespace OpenGeometryEngine;

internal struct Fin
{
    public Vertex X { get; }

    public Vertex Y { get; }

    public IBoundedCurve Curve { get; }

    internal Fin(Vertex x, Vertex y, IBoundedCurve curve) : this()
    {
        X = x;
        Y = y;
        Curve = curve;
    }
}
