namespace OpenGeometryEngine;

public interface ICurveEvaluation
{
	public double Param { get; }

	public Point Point { get; }

	public UnitVec Tangent { get; }

	public double Curvature { get; }

    public Vector Derivative { get; }
}