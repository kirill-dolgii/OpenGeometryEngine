using System.Collections.Generic;

namespace OpenGeometryEngine;

public interface IFlatRegion : IEnumerable<ITrimmedCurve>
{
    double Length { get; }

    double Area { get; }

    Plane Plane { get; }

    int WindingNumber(Point point);

    IFlatRegion Offset(double offset);

    bool Contains(IFlatRegion other);

    bool Intersects(IFlatRegion other);
}
