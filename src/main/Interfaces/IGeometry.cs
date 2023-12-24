using System;

namespace OpenGeometryEngine;

public interface IGeometry : ISpatial, ICloneable, IHasGeometry
{
    IGeometry CreateTransformedCopy(Matrix transformMatrix);
    new IGeometry Clone();
}