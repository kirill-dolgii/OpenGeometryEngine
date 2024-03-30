using OpenGeometryEngine;
using OpenGeometryEngine.Collections;
using OpenGeometryEngine.Extensions;
using OpenGeometryEngine.Regions;

namespace OpenGeometryEngineTests.Solvers;

[TestFixture]
public class PlanarMinCycleSolverTests
{
    //[Test]
    //public void RECT_INTERSECTOIN_CYCLES()
    //{
    //    var p0 = Point.Origin;
    //    var p1 = new Point(0.01, 0.0, 0.0);
    //    var p2 = new Point(0.01, 0.01, 0.0);
    //    var p3 = new Point(0.00, 0.01, 0.0);

    //    var p4 = new Point(0.001, 0.001, 0.0);
    //    var p5 = new Point(0.009, 0.001, 0.0);
    //    var p6 = new Point(0.009, 0.009, 0.0);
    //    var p7 = new Point(0.001, 0.009, 0.0);

    //    var splitPoint0 = new Point(-0.005, 0.005, 0.0);
    //    var splitPoint1 = new Point(0.025, 0.005, 0.0);

    //    var splitCurve = new LineSegment(splitPoint0, splitPoint1);

    //    var firstOuter = Iterate.Over(p0, p1, p2, p3).Pairs(closed: true).Select(pair => new LineSegment(pair.First, pair.Second));
    //    var firstInner = Iterate.Over(p4, p5, p6, p7).Pairs(closed: true).Select(pair => new LineSegment(pair.First, pair.Second));

    //    var polyLineRegion = PolyLineRegion.CreatePolygons(firstOuter.Concat(firstInner).ToArray(), Plane.PlaneXY).Single();

    //    polyLineRegion.Split(Iterate.Over(splitCurve).ToArray());
    //}
}
