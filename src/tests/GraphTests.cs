using DataStructures.Graph;
using OpenGeometryEngine;

namespace OpenGeometryEngineTests;

[TestFixture]
public class GraphTests
{
    [Test]
    public void TRIMMED_CURVE_GRAPH_CONSTRUCTION()
    {
        var curves = new[]
        {
            new LineSegment(Point.Origin, new Point(1.0, 0.0, 0.0)),
            new LineSegment(new Point(1.0, 0.0, 0.0), new Point(1.0, 1.0, 0.0)),
            new LineSegment(new Point(1.0, 1.0, 0.0), new Point(0.0, 1.0, 0.0)),
            new LineSegment(new Point(0.0, 1.0, 0.0), Point.Origin),
        };

        var graph = new Graph<Point, IBoundedCurve>(false);

        foreach (var curve in curves)
        {
            graph.AddEdge(curve.StartPoint, curve.EndPoint, curve);
        }

        Assert.That(graph.Nodes.Count, Is.EqualTo(4));
        Assert.That(graph.EdgesCount, Is.EqualTo(4));
    }

    [Test]
    public void DEPTH_TRAVERSAL()
    {
        var curves = new[]
        {
            new LineSegment(Point.Origin, new Point(1.0, 0.0, 0.0)),
            new LineSegment(new Point(1.0, 0.0, 0.0), new Point(1.0, 1.0, 0.0)),
            new LineSegment(new Point(1.0, 1.0, 0.0), new Point(0.0, 1.0, 0.0)),
            new LineSegment(new Point(0.0, 1.0, 0.0), Point.Origin),
        };

        var graph = new Graph<Point, IBoundedCurve>(false);

        foreach (var curve in curves)
        {
            graph.AddEdge(curve.StartPoint, curve.EndPoint, curve);
        }

        var path = graph.DepthTraversal(Point.Origin);
    }
}