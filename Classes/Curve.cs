using System.Collections.Generic;

namespace OpenGeometryEngine;

public abstract class Curve
{
    public readonly Parametrization Parametrization;
    public abstract ICollection<IntersectionPoint> IntersectCurve(Curve otherCurve);
    public abstract Point Evaluate(double param);
    public bool ContainsParam(double param)
    {
        return param - Parametrization.Bounds.Start >= Constants.Tolerance && 
               param - Parametrization.Bounds.End <= Constants.Tolerance;
    }

    public Curve(Parametrization parametrization) => Parametrization = parametrization;
}