namespace OpenGeometryEngine.Interfaces.Surfaces;

public interface ISurface : IGeometry, IHasSurface
{
    new ISurface CreateTransformedCopy(Matrix transfMatrix);
    new ISurface Clone();
}