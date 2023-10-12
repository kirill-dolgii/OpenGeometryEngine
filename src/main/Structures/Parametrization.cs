using System;

namespace OpenGeometryEngine;

public readonly struct Parametrization : IEquatable<Parametrization>
{
    public static Parametrization Unbounded => new Parametrization(Bounds.Unbounded, Form.Open);

    public readonly Bounds Bounds;
    public readonly Form Form;

    public Parametrization(Bounds bounds, Form form) => (Bounds, Form) = (bounds, form);

    public bool Equals(Parametrization other)
    {
        return Bounds.Equals(other.Bounds) && Form == other.Form;
    }

    public override bool Equals(object obj)
    {
        return obj is Parametrization other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return Bounds.GetHashCode() * 397 ^ (int)Form;
        }
    }
}