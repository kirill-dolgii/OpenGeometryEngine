namespace OpenGeometryEngine;

/// <summary>
/// Represents the evaluation of a curve at a specific parameter value.
/// </summary>
public readonly struct CurveEvaluation
{
    /// <summary>
    /// Gets the parameter value at which the curve was evaluated.
    /// </summary>
    public readonly double Param { get; }

    /// <summary>
    /// Gets the point resulting from the evaluation of the curve.
    /// </summary>
    public readonly Point Point { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CurveEvaluation"/> struct.
    /// </summary>
    /// <param name="param">The parameter value at which the curve was evaluated.</param>
    /// <param name="point">The resulting point from the evaluation of the curve.</param>
    internal CurveEvaluation(double param, Point point) =>
        (Param, Point) = (param, point);

}
