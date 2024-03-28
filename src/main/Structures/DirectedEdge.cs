namespace OpenGeometryEngine;

public struct DirectedEdge
{
	public Point X { get; }

	public Point Y { get; }

	public IBoundedCurve Curve { get; }

	internal DirectedEdge(Point x, Point y, IBoundedCurve curve) : this()
	{
		X = x;
		Y = y;
		Curve = curve;
	}
}
