using OpenGeometryEngine;
using OpenGeometryEngine.Collections;
using OpenGeometryEngine.Exceptions;
using OpenGeometryEngine.Misc.Solvers;
using System.ComponentModel.DataAnnotations;

namespace OpenGeometryEngineTests.Regions.PolyLineRegion;

[TestFixture]
public class PolyLineRegionTests
{
    [Test]
    public void POLYLINE_REGION_CONSTRUCTION_SIMPLE()
    {
        var curves = new IBoundedCurve[]
        {
            new LineSegment(Point.Origin, new Point(0.01, 0.0, 0.0)),
            new LineSegment(new Point(0.01, 0.0, 0.0), new Point(0.01, 0.01, 0.0)),
            new Arc(new Point(0.005, 0.01, 0.0), new Point(0.0, 0.01, 0.0), new Point(0.01, 0.01, 0.0), new UnitVec(0.0, 0.0, -1.0)),
            new LineSegment(Point.Origin, new Point(0.0, 0.01, 0.0)),
        };

        var regions = OpenGeometryEngine.Regions.PolyLineRegion.CreateRegions(curves, Plane.PlaneXY);

        Assert.That(regions, Has.Count.EqualTo(1));

        var region = regions.Single();

        Assert.That(Accuracy.EqualLengths(region.Perimeter, 0.04570796), Is.True);
        Assert.That(Accuracy.CompareWithTolerance(region.Area, 0.0001392699, Math.Sqrt(Accuracy.LinearTolerance)) == 0, Is.True);
    }

    [Test]
    public void POLYLINE_REGION_CONSTRUCTION_WITH_INNER_REGION()
    {
        var curves = new IBoundedCurve[]
        {
            new LineSegment(Point.Origin, new Point(0.01, 0.0, 0.0)),
            new LineSegment(new Point(0.01, 0.0, 0.0), new Point(0.01, 0.01, 0.0)),
            new Arc(new Point(0.005, 0.01, 0.0), new Point(0.0, 0.01, 0.0), new Point(0.01, 0.01, 0.0), new UnitVec(0.0, 0.0, -1.0)),
            new LineSegment(Point.Origin, new Point(0.0, 0.01, 0.0)),
            new LineSegment(new Point(0.003, 0.002, 0.0), new Point(0.007, 0.002, 0.0)),
            new LineSegment(new Point(0.007, 0.002, 0.0), new Point(0.007, 0.01, 0.0)),
            new LineSegment(new Point(0.007, 0.01, 0.0), new Point(0.003, 0.01, 0.0)),
            new LineSegment(new Point(0.003, 0.01, 0.0), new Point(0.003, 0.002, 0.0)),
        };

        var regions = OpenGeometryEngine.Regions.PolyLineRegion.CreateRegions(curves, Plane.PlaneXY);
        
        Assert.That(regions, Has.Count.EqualTo(1));

        var region = regions.Single();

        Assert.That(Accuracy.EqualLengths(region.Perimeter, 0.069707963267948952), Is.True);
        Assert.That(Accuracy.CompareWithTolerance(region.Area, 0.00010726990816987235, Math.Sqrt(Accuracy.LinearTolerance)) == 0, Is.True);
    }

    [Test]
    public void POLYLINE_REGION_CONSTRUCTION_SELF_INTERSECTING()
    {
        var curves = new IBoundedCurve[]
        {
            new LineSegment(Point.Origin, new Point(0.01, 0.0, 0.0)),
            new LineSegment(new Point(0.01, 0.0, 0.0), new Point(0.0, 0.01, 0.0)),
            new LineSegment(new Point(0.0, 0.01, 0.0), new Point(0.01, 0.01, 0.0)),
            new LineSegment(Point.Origin, new Point(0.01, 0.01, 0.0)),
        };

        Assert.That(
        () =>
            {
                return OpenGeometryEngine.Regions.PolyLineRegion.CreateRegions(curves, Plane.PlaneXY);
            }, 
            Throws.InstanceOf(typeof(SelfIntersectingRegionException)));
    }

	[Test]
    public void POLYLINE_REGION_SPLIT()
    {
        var p0 = Point.Origin;
        var p1 = new Point(0.01, 0.0, 0.0);
        var p2 = new Point(0.01, 0.01, 0.0);
        var arcCenter = new Point(0.005, 0.01, 0.0);
        var arcDir = new UnitVec(0d, 0d, -1d);
        var p3 = new Point(0.0, 0.01, 0.0);

        var sp0 = new Point(-0.01, 0.005, 0.0);
        var sp1 = new Point(0.01, 0.005, 0.0);

        var ip0 = new Point(0.0, 0.005, 0.0);
        var ip1 = new Point(0.01, 0.005, 0.0);

        var curves = new IBoundedCurve[]
        {
            new LineSegment(p0, p1),
            new LineSegment(p1, p2),
            new Arc(arcCenter, p3, p2, arcDir),
            new LineSegment(p0, p3),
        };

        var splitSegment = new LineSegment(sp0, sp1);

        var regions = OpenGeometryEngine.Regions.PolyLineRegion.CreateRegions(curves, Plane.PlaneXY);

        Assert.That(regions, Has.Count.EqualTo(1));

        var region = regions.Single();

        var splited = region.Split(Iterate.Over(splitSegment).ToArray());
        
        var left = splited.ElementAt(0);
        var right = splited.ElementAt(1);

        Assert.Multiple(() =>
        {
            Assert.That(left.InnerRegions, Has.Count.EqualTo(0));
            Assert.That(left.Curves, Has.Count.EqualTo(4));
            Assert.That(left.Vertices, Has.Count.EqualTo(4));

            Assert.That(left.Vertices, Has.Member(p0));
            Assert.That(left.Vertices, Has.Member(p1));
            Assert.That(left.Vertices, Has.Member(ip0));
            Assert.That(left.Vertices, Has.Member(ip1));
        });

        Assert.Multiple(() =>
        {
            Assert.That(right.InnerRegions, Has.Count.EqualTo(0));
            Assert.That(right.Curves, Has.Count.EqualTo(4));
            Assert.That(right.Vertices, Has.Count.EqualTo(4));

            Assert.That(right.Vertices, Has.Member(p2));
            Assert.That(right.Vertices, Has.Member(p3));
            Assert.That(right.Vertices, Has.Member(ip0));
            Assert.That(right.Vertices, Has.Member(ip1));
        });
    }
}