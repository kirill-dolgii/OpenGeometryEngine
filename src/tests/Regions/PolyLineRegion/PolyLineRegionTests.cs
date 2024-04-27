using OpenGeometryEngine;
using OpenGeometryEngine.Collections;
using OpenGeometryEngine.Exceptions;
using OpenGeometryEngine.Regions;

namespace OpenGeometryEngineTests.Regions;

[TestFixture]
public class PolyLineRegionTests : PolyLineRegionTestBase
{
    private PolyLineRegion CreateFromFile(string path)
    {
        var result = Translators.DxfTranslator.Read(path, out var curves);
        var polygon = PolyLineRegion.CreateRegions(curves, Plane.PlaneXY).Single();
        return polygon;
    }

	[Test]
    public void INTERSECT_RECTS()
    {
		var fi = base.TestDataDirectory.EnumerateFiles().FirstOrDefault(fi => fi.Name == "rect0.dxf");
		if (fi == null) throw new Exception("failed to read rect0.dxf");
        var rect0 = CreateFromFile(fi.FullName);
		fi = base.TestDataDirectory.EnumerateFiles().FirstOrDefault(fi => fi.Name == "rect1.dxf");
		if (fi == null) throw new Exception("failed to read rect1.dxf");
		var rect1 = CreateFromFile(fi.FullName);

        var inters = rect0.Intersect(rect1);

        Assert.That(inters, Has.Count.EqualTo(1));
        var region = inters.Single();

        Assert.That(region.Boundary, Has.Count.EqualTo(4));
        Assert.That(region.IsOuter, Is.True);

        var area = 0.000025;
        var perimeter = 0.02;

        Assert.That(Accuracy.AreEqual(region.Area, area, Math.Sqrt(Accuracy.LinearTolerance)));
        Assert.That(Accuracy.EqualLengths(perimeter, region.Perimeter));
	}
    
	[Test]
    public void INTERSECT_TOUCH_RECTS()
    {
		var fi = base.TestDataDirectory.EnumerateFiles().FirstOrDefault(fi => fi.Name == "rect small.dxf");
		if (fi == null) throw new Exception("rect small.dxf");
        var rect0 = CreateFromFile(fi.FullName);
		fi = base.TestDataDirectory.EnumerateFiles().FirstOrDefault(fi => fi.Name == "rect0.dxf");
		if (fi == null) throw new Exception("rect0.dxf");
		var rect1 = CreateFromFile(fi.FullName);

        var inters = rect0.Intersect(rect1);

		Assert.That(inters, Has.Count.EqualTo(1));
		var region = inters.Single();

		Assert.That(region.Boundary, Has.Count.EqualTo(4));
		Assert.That(region.IsOuter, Is.True);

		var area = 0.000025;
		var perimeter = 0.02;

		Assert.That(Accuracy.AreEqual(region.Area, area, Math.Sqrt(Accuracy.LinearTolerance)));
		Assert.That(Accuracy.EqualLengths(perimeter, region.Perimeter));
	}

	[Test]
    public void INTERSECT_RECT_PROFILES()
    {
		var fi = base.TestDataDirectory.EnumerateFiles().FirstOrDefault(fi => fi.Name == "rectangular profile 0.dxf");
		if (fi == null) throw new Exception("rectangular profile 0.dxf");
        var rect0 = CreateFromFile(fi.FullName);
		fi = base.TestDataDirectory.EnumerateFiles().FirstOrDefault(fi => fi.Name == "rectangular profile 1.dxf");
		if (fi == null) throw new Exception("rectangular profile.dxf");
		var rect1 = CreateFromFile(fi.FullName);

        var inters = rect0.Intersect(rect1);

		Assert.That(inters, Has.Count.EqualTo(2));
		
        var region1 = inters.ElementAt(0);
        var region2 = inters.ElementAt(1);
        
		Assert.That(region1.Boundary, Has.Count.EqualTo(4));
		Assert.That(region1.IsOuter, Is.True);
        
		Assert.That(region2.Boundary, Has.Count.EqualTo(4));
		Assert.That(region2.IsOuter, Is.True);

		var area = 0.000001;
		var perimeter = 0.004;

		Assert.That(Accuracy.AreEqual(region1.Area, area, Math.Sqrt(Accuracy.LinearTolerance)));
		Assert.That(Accuracy.EqualLengths(perimeter, region1.Perimeter));
        
		Assert.That(Accuracy.AreEqual(region2.Area, area, Math.Sqrt(Accuracy.LinearTolerance)));
		Assert.That(Accuracy.EqualLengths(perimeter, region2.Perimeter));
	}

    [Test]
    public void MERGE_RECT_PROFILES()
    {
		var fi = base.TestDataDirectory.EnumerateFiles().FirstOrDefault(fi => fi.Name == "rectangular profile 0.dxf");
		if (fi == null) throw new Exception("rectangular profile 0.dxf");
		var rect0 = CreateFromFile(fi.FullName);
		fi = base.TestDataDirectory.EnumerateFiles().FirstOrDefault(fi => fi.Name == "rectangular profile 1.dxf");
		if (fi == null) throw new Exception("rectangular profile.dxf");
		var rect1 = CreateFromFile(fi.FullName);

		var region = rect0.Merge(rect1);

        var area = 0.000070;
        var perimeter = 0.136;

		Assert.Multiple(() =>
		{
			Assert.That(Accuracy.AreEqual(region.Area, area, Math.Sqrt(Accuracy.LinearTolerance)));
			Assert.That(Accuracy.EqualLengths(region.Perimeter, perimeter));
		});

        Assert.That(region.InnerRegions, Has.Count.EqualTo(3));

        var innerRegion1 = region.InnerRegions.First();
		var innerArea1 = 0.000048;
		var innerPerimeter1 = 0.032;

		Assert.Multiple(() =>
		{
			Assert.That(Accuracy.AreEqual(innerRegion1.Area, innerArea1, Math.Sqrt(Accuracy.LinearTolerance)));
			Assert.That(Accuracy.EqualLengths(innerRegion1.Perimeter, innerPerimeter1));
		});
        
        var innerRegion2 = region.InnerRegions.ElementAt(1);
		var innerArea2 = 0.000048;
		var innerPerimeter2 = 0.032;

		Assert.Multiple(() =>
		{
			Assert.That(Accuracy.AreEqual(innerRegion2.Area, innerArea2, Math.Sqrt(Accuracy.LinearTolerance)));
			Assert.That(Accuracy.EqualLengths(innerRegion2.Perimeter, innerPerimeter2));
		});
        
        var innerRegion3 = region.InnerRegions.ElementAt(2);
		var innerArea3 = 0.000009;
		var innerPerimeter3 = 0.012;

		Assert.Multiple(() =>
		{
			Assert.That(Accuracy.AreEqual(innerRegion3.Area, innerArea3, Math.Sqrt(Accuracy.LinearTolerance)));
			Assert.That(Accuracy.EqualLengths(innerRegion3.Perimeter, innerPerimeter3));
		});
	}
    
    [Test]
    public void MERGE_TOUCH_RECTS()
    {
		var fi = base.TestDataDirectory.EnumerateFiles().FirstOrDefault(fi => fi.Name == "rect small.dxf");
		if (fi == null) throw new Exception("rect small.dxf");
		var rect0 = CreateFromFile(fi.FullName);
		fi = base.TestDataDirectory.EnumerateFiles().FirstOrDefault(fi => fi.Name == "rect0.dxf");
		if (fi == null) throw new Exception("rect0.dxf");
		var rect1 = CreateFromFile(fi.FullName);

		var region = rect0.Merge(rect1);

        Assert.That(region.Boundary, Has.Count.EqualTo(6));
        Assert.That(region.IsOuter, Is.True);

        Assert.That(region.InnerRegions, Is.Empty);

        var area = 0.000100;
        var perimeter = 0.04;

        Assert.That(Accuracy.AreEqual(region.Area, area, Math.Sqrt(Accuracy.LinearTolerance)));
        Assert.That(Accuracy.EqualLengths(perimeter, region.Perimeter));
    }

	[Test]
	public void SUBTRACT_RECT_PROFILES()
	{
		var fi = base.TestDataDirectory.EnumerateFiles().FirstOrDefault(fi => fi.Name == "rectangular profile 0.dxf");
		if (fi == null) throw new Exception("rectangular profile 0.dxf");
		var rect0 = CreateFromFile(fi.FullName);
		fi = base.TestDataDirectory.EnumerateFiles().FirstOrDefault(fi => fi.Name == "rectangular profile 1.dxf");
		if (fi == null) throw new Exception("rectangular profile.dxf");
		var rect1 = CreateFromFile(fi.FullName);

		var regions = rect0.Subtract(rect1);

        Assert.That(regions, Has.Count.EqualTo(2));

        var region1 = regions.First();
		var areaRegion1 = 0.000027;
		var perimeterRegion1 = 0.056;

        Assert.Multiple(() =>
        {
            Assert.That(Accuracy.AreEqual(region1.Area, areaRegion1, Math.Sqrt(Accuracy.LinearTolerance)));
            Assert.That(Accuracy.EqualLengths(region1.Perimeter, perimeterRegion1));
        });
        
        var region2 = regions.ElementAt(1);
		var areaRegion2 = 0.000007;
		var perimeterRegion2 = 0.016;

        Assert.Multiple(() =>
        {
            Assert.That(Accuracy.AreEqual(region2.Area, areaRegion2, Math.Sqrt(Accuracy.LinearTolerance)));
            Assert.That(Accuracy.EqualLengths(region2.Perimeter, perimeterRegion2));
        });
    }

	[Test]
	public void SUBTRACT_TOUCH_RECTS()
	{
		var fi = base.TestDataDirectory.EnumerateFiles().FirstOrDefault(fi => fi.Name == "rect small.dxf");
		if (fi == null) throw new Exception("rect small.dxf");
		var rect0 = CreateFromFile(fi.FullName);
		fi = base.TestDataDirectory.EnumerateFiles().FirstOrDefault(fi => fi.Name == "rect0.dxf");
		if (fi == null) throw new Exception("rect0.dxf");
		var rect1 = CreateFromFile(fi.FullName);

		var regions = rect0.Subtract(rect1);
        Assert.That(regions, Is.Empty);

        regions = rect1.Subtract(rect0);
        Assert.That(regions, Has.Count.EqualTo(1));

        var region = regions.Single();
        Assert.That(region.InnerRegions, Is.Empty);

		var area = 0.000075;
		var perimeter = 0.040;

		Assert.Multiple(() =>
		{
			Assert.That(Accuracy.AreEqual(region.Area, area, Math.Sqrt(Accuracy.LinearTolerance)));
			Assert.That(Accuracy.EqualLengths(region.Perimeter, perimeter));
		});
	}

	// Profile 0 is a simple line only region that contains a single inner rectangular region
	[Test]
    public void CONSTRUCTION_PROFILE_0()
    {
        var fi = base.CommonTestDataDirectory.EnumerateFiles().FirstOrDefault(fi => fi.Name == "profile0.dxf");
        if (fi == null) throw new Exception();
        var region = CreateFromFile(fi.FullName);

        // Inner/outer regions structure checks
        Assert.That(region.Boundary, Has.Count.EqualTo(118));
        Assert.That(region.InnerRegions, Has.Count.EqualTo(1));
        Assert.That(region.InnerRegions.Single().Boundary, Has.Count.EqualTo(4));

        Assert.That(region.IsOuter, Is.True);
        Assert.That(((PolyLineRegion)region.InnerRegions.Single()).IsOuter, Is.False);

        var area = 0.00023185765385;
        var perimeter = 0.25918847535;

        Assert.Multiple(() =>
        {
            Assert.That(Accuracy.AreEqual(region.Area, area, Math.Sqrt(Accuracy.LinearTolerance)));
            Assert.That(Accuracy.EqualLengths(region.Perimeter, perimeter));
        });

        // Point containment checks
        var outerPoint = new Point(0.018, 0.046, 0.0);
        var innerPoint = new Point(0.018, 0.053, 0.0);
        var boundaryPoint = new Point(0.01796234537, 0.04785211113, 0.0);

        Assert.Multiple(() =>
        {
            Assert.That(region.ContainsPoint(outerPoint), Is.False);
            Assert.That(region.ContainsPoint(innerPoint), Is.True);
            Assert.That(region.ContainsPoint(boundaryPoint), Is.True);
        });

        // Inner region checks
        var innerRegionInnerPoint = new Point(0.025, 0.064, 0.0);
        var innerRegion = (PolyLineRegion)region.InnerRegions.Single();

        Assert.Multiple(() =>
        {
            Assert.That(innerRegion.ContainsPoint(innerRegionInnerPoint), Is.True);
            Assert.That(innerRegion.ContainsPoint(outerPoint), Is.False);
        });

        var innerRegionArea = 0.00041560141514;
        var innerRegionPerimeter = 0.08197326331;

        Assert.Multiple(() =>
        {
            Assert.That(Accuracy.AreEqual(innerRegion.Area, innerRegionArea, Math.Sqrt(Accuracy.LinearTolerance)));
            Assert.That(Accuracy.EqualLengths(innerRegion.Perimeter, innerRegionPerimeter));
        });
    }

    // Profile 0 is a contains lines and arcs and a single inner rectangular region
    [Test]
    public void CONSTRUCTION_PROFILE_1()
    {
        var fi = base.CommonTestDataDirectory.EnumerateFiles().FirstOrDefault(fi => fi.Name == "profile1.dxf");
        if (fi == null) throw new Exception();
        var region = CreateFromFile(fi.FullName);

        // Inner/outer regions structure checks
        Assert.That(region.Boundary, Has.Count.EqualTo(56));
        Assert.That(region.InnerRegions, Has.Count.EqualTo(2));
        Assert.That(region.InnerRegions.First().Boundary, Has.Count.EqualTo(14));
        Assert.That(region.InnerRegions.ElementAt(1).Boundary, Has.Count.EqualTo(17));

        Assert.That(region.IsOuter, Is.True);
        Assert.That(((PolyLineRegion)region.InnerRegions.ElementAt(0)).IsOuter, Is.False);
        Assert.That(((PolyLineRegion)region.InnerRegions.ElementAt(1)).IsOuter, Is.False);

        var area = 0.00014093714741;
		var perimeter = 0.21485953917;

        Assert.Multiple(() =>
        {
            Assert.That(Accuracy.AreEqual(region.Area, area, Math.Sqrt(Accuracy.LinearTolerance)), Is.True);
            Assert.That(Accuracy.EqualLengths(region.Perimeter, perimeter), Is.True);
        });

        // Point containment checks
        var outerPoint = new Point(0.072, 0.019, 0.0);
        var innerPoint = new Point(0.07, 0.019, 0.0);
        var boundaryPoint = new Point(0.07095779555, 0.02, 0.0);

        Assert.Multiple(() =>
        {
            Assert.That(region.ContainsPoint(outerPoint), Is.False);
            Assert.That(region.ContainsPoint(innerPoint), Is.True);
            Assert.That(region.ContainsPoint(boundaryPoint), Is.True);
        });

        // Inner region checks
        var innerRegionInnerPoint1 = new Point(0.065, 0.027, 0.0);
        var innerRegionInnerPoint2 = new Point(0.065, 0.036, 0.0);

        var innerRegion1 = (PolyLineRegion)region.InnerRegions.ElementAt(1);
        var innerRegion2 = (PolyLineRegion)region.InnerRegions.ElementAt(0);

        Assert.Multiple(() =>
        {
            Assert.That(innerRegion1.ContainsPoint(innerRegionInnerPoint1), Is.True);
            Assert.That(innerRegion1.ContainsPoint(outerPoint), Is.False);
        });
        
        Assert.Multiple(() =>
        {
            Assert.That(innerRegion2.ContainsPoint(innerRegionInnerPoint2), Is.True);
            Assert.That(innerRegion2.ContainsPoint(outerPoint), Is.False);
        });

        var innerRegionArea1 = 0.00009052638905;
		var innerRegionPerimeter1 = 0.04091241106;

        Assert.Multiple(() =>
        {
            Assert.That(Accuracy.AreEqual(innerRegion1.Area, innerRegionArea1, Math.Sqrt(Accuracy.LinearTolerance)));
            Assert.That(Accuracy.EqualLengths(innerRegion1.Perimeter, innerRegionPerimeter1));
        });

        var innerRegionArea2 = 0.00011155617353;
		var innerRegionPerimeter2 = 0.04036854802;

        Assert.Multiple(() =>
        {
            Assert.That(Accuracy.AreEqual(innerRegion2.Area, innerRegionArea2, Math.Sqrt(Accuracy.LinearTolerance)));
            Assert.That(Accuracy.EqualLengths(innerRegion2.Perimeter, innerRegionPerimeter2));
        });
    }
        
    [Test]
    public void SPLIT_PROFILE_1()
    {
		var fi = base.CommonTestDataDirectory.EnumerateFiles().FirstOrDefault(fi => fi.Name == "profile1.dxf");
		if (fi == null) throw new Exception();
		var region = CreateFromFile(fi.FullName);

        var linePoint1 = new Point(0.05874258207, 0.0549982493, 0.0);
        var linePoint2 = new Point(0.06998276708, 0.01372391126, 0.0);

		var splitLine = new LineSegment(linePoint1, linePoint2);

        var splitted = region.Split(Iterate.Over((IBoundedCurve)splitLine).ToList());

        Assert.That(splitted, Has.Count.EqualTo(3));

        var first = splitted.ElementAt(0);
        var second = splitted.ElementAt(1);
        var third = splitted.ElementAt(2);

        var firstArea = 0.00004161335703;
        var firstPerimeter = 0.08212873217;
        
        var secondArea = 0.0000967035474;
		var secondPerimeter = 0.13906820223091787;
        
        var thirdArea = 0.00000262024298;
		var thirdPerimeter = 0.0072234055757239941;


		Assert.Multiple(() =>
		{
			Assert.That(Accuracy.AreEqual(first.Area, firstArea, Math.Sqrt(Accuracy.LinearTolerance)));
			Assert.That(Accuracy.EqualLengths(first.Perimeter, firstPerimeter));
		});
        
		Assert.Multiple(() =>
		{
			Assert.That(Accuracy.AreEqual(second.Area, secondArea, Math.Sqrt(Accuracy.LinearTolerance)));
			Assert.That(Accuracy.EqualLengths(second.Perimeter, secondPerimeter));
		});
        
		Assert.Multiple(() =>
		{
			Assert.That(Accuracy.AreEqual(third.Area, thirdArea, Math.Sqrt(Accuracy.LinearTolerance)));
			Assert.That(Accuracy.EqualLengths(third.Perimeter, thirdPerimeter));
		});
	}

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

        var area = 0.0001392699;
        var perimeter = 0.04570796;

		Assert.That(Accuracy.EqualLengths(region.Perimeter, perimeter), Is.True);
        Assert.That(Accuracy.CompareWithTolerance(region.Area, area, Math.Sqrt(Accuracy.LinearTolerance)) == 0, Is.True);
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

        var regions = PolyLineRegion.CreateRegions(curves, Plane.PlaneXY);
        
        Assert.That(regions, Has.Count.EqualTo(1));

        var region = regions.Single();

        var area = 0.00010726990816987235;
        var perimeter = 0.069707963267948952;

		Assert.That(Accuracy.EqualLengths(region.Perimeter, perimeter), Is.True);
        Assert.That(Accuracy.CompareWithTolerance(region.Area, area, Math.Sqrt(Accuracy.LinearTolerance)) == 0, Is.True);
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

        var regions = PolyLineRegion.CreateRegions(curves, Plane.PlaneXY);

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