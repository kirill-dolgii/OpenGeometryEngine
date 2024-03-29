﻿using OpenGeometryEngine;
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

        var regions = OpenGeometryEngine.Regions.PolyLineRegion.CreatePolygons(curves, Plane.PlaneXY);

        Assert.That(regions, Has.Count.EqualTo(1));

        var region = regions.Single();

        Assert.That(Accuracy.EqualLengths(region.Length, 0.04570796), Is.True);
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

        var regions = OpenGeometryEngine.Regions.PolyLineRegion.CreatePolygons(curves, Plane.PlaneXY);
        
        Assert.That(regions, Has.Count.EqualTo(1));

        var region = regions.Single();

        Assert.That(Accuracy.EqualLengths(region.Length, 0.069707963267948952), Is.True);
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
                return OpenGeometryEngine.Regions.PolyLineRegion.CreatePolygons(curves, Plane.PlaneXY);
            }, 
            Throws.InstanceOf(typeof(SelfIntersectingRegionException)));
    }

	[Test]
	public void POLYLINE_REGION_GET_iNTERSECTION_CURVES()
	{
		var p0 = Point.Origin;
		var p1 = new Point(0.01, 0.0, 0.0);
		var p2 = new Point(0.01, 0.01, 0.0);
		var p3 = new Point(0.0, 0.01, 0.0);

		var center = new Point(0.005, 0.005, 0.0);
		var ip1 = new Point(0.015, 0.01, 0.0);
		var ip2 = new Point(-0.005, 0.01, 0.0);

		var curves = new IBoundedCurve[]
		{
			new LineSegment(p0, p1),
			new LineSegment(p1, p2),
			new LineSegment(p2, p3),
			new LineSegment(p0, p3),
			new LineSegment(p0, p2),
		};

		var splitLines = new IBoundedCurve[]
		{
			new LineSegment(center, ip1),
			new LineSegment(center, ip2),
		};

        var planarRegion = new PlanarCurveGraph(curves, Plane.PlaneXY);
        var solver = new PlanarMinCycleSolver(planarRegion);
        solver.Solve();

        var ordered = planarRegion.WalkCurves(p3);

		var region = OpenGeometryEngine.Regions.PolyLineRegion.CreatePolygons(curves, Plane.PlaneXY).Single();
		var splitCurves = region.GetIntersectionCurves(splitLines, out var splitted);

        Assert.That(splitCurves, Has.Count.EqualTo(2));
        
        Assert.That(splitted, Has.Count.EqualTo(2));
        Assert.That(splitted.ElementAt(0).Value, Has.Count.EqualTo(2));
        Assert.That(splitted.ElementAt(1).Value, Has.Count.EqualTo(2));

        Assert.Multiple(() =>
        {
            var kv = splitted.ElementAt(0);
            Assert.That(kv.Value.Sum(curve => curve.Length), Is.EqualTo(kv.Key.Length));
            Assert.That(kv.Value.ElementAt(0).StartPoint, Is.EqualTo(Point.Origin));
            Assert.That(kv.Value.ElementAt(0).EndPoint, Is.EqualTo(new Point(0.0, 0.0075, 0.0)));
            Assert.That(kv.Value.ElementAt(1).StartPoint, Is.EqualTo(new Point(0.0, 0.0075, 0.0)));
            Assert.That(kv.Value.ElementAt(1).EndPoint, Is.EqualTo(p3));
        });

        Assert.Multiple(() =>
        {
            var kv = splitted.ElementAt(1);
            Assert.That(kv.Value.Sum(curve => curve.Length), Is.EqualTo(kv.Key.Length));
            Assert.That(kv.Value.ElementAt(0).StartPoint, Is.EqualTo(p2));
            Assert.That(kv.Value.ElementAt(0).EndPoint, Is.EqualTo(new Point(0.01, 0.0075, 0.0)));
            Assert.That(kv.Value.ElementAt(1).StartPoint, Is.EqualTo(new Point(0.01, 0.0075, 0.0)));
            Assert.That(kv.Value.ElementAt(1).EndPoint, Is.EqualTo(p1));
        });
        
        Assert.Multiple(() =>
        {
            var curve = splitCurves.ElementAt(0);
            Assert.That(curve.StartPoint, Is.EqualTo(center));
            Assert.That(curve.EndPoint, Is.EqualTo(new Point(0.0, 0.0075, 0.0)));
        });
        
        Assert.Multiple(() =>
        {
            var curve = splitCurves.ElementAt(1);
            Assert.That(curve.StartPoint, Is.EqualTo(center));
            Assert.That(curve.EndPoint, Is.EqualTo(new Point(0.01, 0.0075, 0.0)));
        });
	}

    [Test]
    public void TEST_LOOPS()
    {
		var p0 = Point.Origin;
		var p1 = new Point(0.01, 0.0, 0.0);
		var p2 = new Point(0.01, 0.01, 0.0);
		var p3 = new Point(0.0, 0.01, 0.0);

		var center = new Point(0.005, 0.005, 0.0);
		var ip1 = new Point(0.015, 0.01, 0.0);
		var ip2 = new Point(-0.005, 0.01, 0.0);

		var curves = new IBoundedCurve[]
		{
			new LineSegment(p0, p1),
			new LineSegment(p1, p2),
			new LineSegment(p2, p3),
			new LineSegment(p0, p3),
			new LineSegment(p0, p2),
		};

        var graph = new PlanarCurveGraph(curves, Plane.PlaneXY);

        var solver = new PlanarMinCycleSolver(graph);
        var result = solver.Solve();
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

        var sp0 = new Point(0.0, 0.005, 0.0);
        var sp1 = new Point(0.01, 0.005, 0.0);

        var curves = new IBoundedCurve[]
        {
            new LineSegment(p0, p1),
            new LineSegment(p1, p2),
            new Arc(arcCenter, p3, p2, arcDir),
            new LineSegment(p0, p3),
        };

        var splitLine = new Line(sp0, sp1);

        var regions = OpenGeometryEngine.Regions.PolyLineRegion.CreatePolygons(curves, Plane.PlaneXY);

        Assert.That(regions, Has.Count.EqualTo(1));

        var region = regions.Single();

        var splited = region.Split(splitLine);
        
        var left = splited.ElementAt(0).Single();
        var right = splited.ElementAt(1).Single();

        Assert.Multiple(() =>
        {
            Assert.That(left.InnerRegions, Has.Count.EqualTo(0));
            Assert.That(left.Curves, Has.Count.EqualTo(4));
            Assert.That(left.Vertices, Has.Count.EqualTo(4));

            Assert.That(left.Vertices, Has.Member(p2));
            Assert.That(left.Vertices, Has.Member(p3));
            Assert.That(left.Vertices, Has.Member(sp0));
            Assert.That(left.Vertices, Has.Member(sp1));
        });

        Assert.Multiple(() =>
        {
            Assert.That(right.InnerRegions, Has.Count.EqualTo(0));
            Assert.That(right.Curves, Has.Count.EqualTo(4));
            Assert.That(right.Vertices, Has.Count.EqualTo(4));

            Assert.That(right.Vertices, Has.Member(p0));
            Assert.That(right.Vertices, Has.Member(p1));
            Assert.That(right.Vertices, Has.Member(sp0));
            Assert.That(right.Vertices, Has.Member(sp1));
        });
    }
}