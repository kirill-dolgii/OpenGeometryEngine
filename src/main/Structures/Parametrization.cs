using OpenGeometryEngine.Misc.Enums;

namespace OpenGeometryEngine;

public readonly struct Parametrization
{
    public static Parametrization Unbounded 
        => new Parametrization(Bounds.Unbounded, Form.Open);

    public readonly Bounds Bounds;
    public readonly Form Form;

    public Parametrization(Bounds bounds, Form form) => (Bounds, Form) = (bounds, form);
}