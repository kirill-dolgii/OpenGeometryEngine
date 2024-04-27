using OpenGeometryEngine;
using OpenGeometryEngine.Misc.Solvers;

namespace OpenGeometryEngineTests.Solvers;

[TestFixture]
public class LoopWire2dSolverTests
{
	[Test]
	public void GET_FIRST_EDGE()
	{
		var points = new Point[]
		{
			Point.Origin,
			new Point(0.0, 1.0, 0.0),
			new Point(1.0, 1.0, 0.0),
			new Point(3.0, 1.0, 0.0),
			new Point(3.0, -1.0, 0.0),
		};

		IBoundedCurve[] curves = new[]
		{
			new LineSegment(points[0], points[1]),
			new LineSegment(points[0], points[2]),
			new LineSegment(points[0], points[3]),
			new LineSegment(points[0], points[4]),
		};

		var solver = new LoopWire2dSolver(curves, Array.Empty<IBoundedCurve>(), Plane.PlaneXY);
		var firstEdge = solver.GetFirstEdge(solver._map[points[0]]);

		Assert.That(solver._edges[firstEdge], Is.EqualTo(curves[3]));
	}

	[Test]
	public void GET_NEXT_EDGE()
	{
		var points = new Point[]
		{
			Point.Origin,
			new Point(1.0, 0.0, 0.0),
			new Point(2.0, 1.0, 0.0),
			new Point(3.0, 1.0, 0.0),
			new Point(3.0, -1.0, 0.0),
		};

		IBoundedCurve[] curves = new[]
		{
			new LineSegment(points[0], points[1]),
			new LineSegment(points[1], points[2]),
			new LineSegment(points[1], points[3]),
			new LineSegment(points[1], points[4]),
		};

		var solver = new LoopWire2dSolver(curves, Array.Empty<IBoundedCurve>(), Plane.PlaneXY);
		solver._visitedEdges[curves[0]] = true;
		
		var next = solver.GetNextEdge(solver._map[points[1]], solver._map[points[0]]);
		Assert.That(solver._edges[(next.x, next.y)] == curves[1]);
	}
}
