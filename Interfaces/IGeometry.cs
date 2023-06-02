namespace OpenGeometryEngine;

public interface IGeometry : ISpatial
{
    public IGeometry CreateTransformedCopy(Matrix transformationMatrix);

    public bool IsCoincident(IGeometry otherGeometry);
}