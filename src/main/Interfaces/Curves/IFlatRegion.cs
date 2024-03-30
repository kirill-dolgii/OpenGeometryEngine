using System.Collections.Generic;

namespace OpenGeometryEngine;

public interface IFlatRegion : IEnumerable<IBoundedCurve>
{
    double Perimeter { get; }

    double Area { get; }

    Plane Plane { get; }

    ICollection<IBoundedCurve> Boundary { get; }

    public ICollection<IFlatRegion> InnerRegions { get; }

    int WindingNumber(Point point);

    IFlatRegion Offset(double offset);

    bool Contains(IFlatRegion other);

    bool Intersects(IFlatRegion other);
}
