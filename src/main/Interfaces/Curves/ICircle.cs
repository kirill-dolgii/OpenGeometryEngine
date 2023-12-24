using OpenGeometryEngine;
using OpenGeometryEngine.Interfaces.Curves;

public interface ICircle : ICurve, IHasDirection, IHasOrigin, IHasPlane, IHasFrame, IHasRadius
{
    new ICircle Clone();
    new ICircle CreateTransformedCopy(Matrix transformMatrix);
}