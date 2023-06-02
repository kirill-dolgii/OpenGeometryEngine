using System;

namespace OpenGeometryEngine;

public readonly struct Parametrization
{
    public readonly Bounds Bounds;
    public readonly Form Form;

    public Parametrization(Bounds bounds, Form form) => (Bounds, Form) = (bounds, form);
}