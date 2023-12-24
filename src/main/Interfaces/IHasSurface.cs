using OpenGeometryEngine.Interfaces.Surfaces;

namespace OpenGeometryEngine;

public interface IHasSurface : ISpatial, IHasGeometry
{
    ISurface Surface { get; }
    ISurfaceEvaluation Evaluate(PointUV param);
    new ISurfaceEvaluation ProjectPoint(Point Point);
    UV<Parametrization> Parametrization { get; }
}

