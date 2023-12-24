using OpenGeometryEngine;
using OpenGeometryEngine.Interfaces.Curves;

public interface ILine : ICurve, IHasDirection, IHasOrigin
{
    new ILine CreateTransformedCopy(Matrix transformMatrix);
    new ILine Clone();
}