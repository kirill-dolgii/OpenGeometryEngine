using OpenGeometryEngine;
using OpenGeometryEngine.Misc.Solvers;

namespace OpenGeometryEngineTests;

[TestFixture]
public class PolygonRegionTests : PolygonRegionTestBase
{
    private PolygonRegion CreateRegionFromDxfPath(string path)
	{
        var result = Translators.DxfTranslator.Read(path, out var curves);
		var solver = new LoopWire2dSolver(curves, Array.Empty<IBoundedCurve>(), Plane.PlaneXY);
		var wires = solver.Solve();
		var polygon = new PolygonRegion(solver.Loops.Single(), Plane.PlaneXY, null);
        return polygon;
	}

	[Test]
    public void Convex1_CONSTRUCTION()
    {
        var fi = base.TestDataDirectory.EnumerateFiles().FirstOrDefault(fi => fi.Name == "convex1.dxf");
		if (fi == null) throw new Exception("failed to read convex1.dxf");

        var polygon = CreateRegionFromDxfPath(fi.FullName);

        var perimeter = 0.081318437;
        var area = 0.0004771001315;

		Assert.Multiple(() =>
        {
            Assert.That(Accuracy.EqualLengths(polygon.Perimeter, perimeter), Is.True);
            Assert.That(Accuracy.EqualAreas(polygon.Area, area), Is.True);
            Assert.That(polygon.IsConvex, Is.True);
        });

        var outerPoint = new Point(-0.014, -0.009, 0.0);
        var innerPoint = new Point(-0.01, -0.009, 0.0);
        var boundaryPoint = new Point(-0.00921687836, -0.01221687836, 0.0);

        Assert.Multiple(() =>
        {
            Assert.That(polygon.ContainsPoint(outerPoint), Is.False);
            Assert.That(polygon.ContainsPoint(innerPoint), Is.True);
            Assert.That(polygon.ContainsPoint(boundaryPoint), Is.True);
        });
    }
    
    [Test]
    public void Concave1_CONSTRUCTION()
    {
        var fi = base.TestDataDirectory.EnumerateFiles().FirstOrDefault(fi => fi.Name == "concave1.dxf");
        if (fi == null) throw new Exception("failed to read concave1.dxf");

        var polygon = CreateRegionFromDxfPath(fi.FullName);

		var perimeter = 0.1228737604;
        var area = 0.0004673504501;

		Assert.Multiple(() =>
        {
            Assert.That(Accuracy.EqualLengths(polygon.Perimeter, perimeter), Is.True);
            Assert.That(Accuracy.EqualAreas(polygon.Area, area), Is.True);
            Assert.That(polygon.IsConvex, Is.False);
        });

        var outerPoint = new Point(-0.009, -0.006, 0.0);
        var innerPoint = new Point(-0.003, -0.005, 0.0);
        var boundaryPoint = new Point(-0.004, -0.01, 0.0);

        Assert.Multiple(() =>
        {
            Assert.That(polygon.ContainsPoint(outerPoint), Is.False);
            Assert.That(polygon.ContainsPoint(innerPoint), Is.True);
            Assert.That(polygon.ContainsPoint(boundaryPoint), Is.True);
        });
    }

	[Test]
	public void re_6064_CONSTRUCTION()
	{
		var fi = base.CommonTestDataDirectory.EnumerateFiles().FirstOrDefault(fi => fi.Name == "re_6064.dxf");
		if (fi == null) throw new Exception("failed to read re_6064.dxf");

		var polygon = CreateRegionFromDxfPath(fi.FullName);

		var perimeter = 0.4859019772;
		var area = 0.0006458875167;

		Assert.Multiple(() =>
		{
			Assert.That(Accuracy.EqualLengths(polygon.Perimeter, perimeter), Is.True);
			Assert.That(Accuracy.EqualAreas(polygon.Area, area), Is.True);
			Assert.That(polygon.IsConvex, Is.False);
		});

        var outerPoint = new Point(0.454, 0.152, 0.0);
        var innerPoint = new Point(0.491, 0.189, 0.0);
        var boundaryPoint = new Point(0.441, 0.1904880, 0.0);

        Assert.Multiple(() =>
        {
            Assert.That(polygon.ContainsPoint(outerPoint), Is.False);
            Assert.That(polygon.ContainsPoint(innerPoint), Is.True);
            Assert.That(polygon.ContainsPoint(boundaryPoint), Is.True);
        });
    }
}
