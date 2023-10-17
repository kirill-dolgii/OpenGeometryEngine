using OpenGeometryEngine.Exceptions;
using System;

namespace OpenGeometryEngine.Structures;

/// <summary>
/// Represents a numerical interval defined by a start and end point.
/// </summary>
public struct Interval
{
    /// <summary>
    /// Gets the start point of the interval.
    /// </summary>
    public double Start { get; }

    /// <summary>
    /// Gets the end point of the interval.
    /// </summary>
    public double End { get; }

    /// <summary>
    /// Gets the span of the interval (End - Start).
    /// </summary>
    public double Span { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Interval"/> struct with the specified start and end points.
    /// </summary>
    /// <param name="start">The start point of the interval.</param>
    /// <param name="end">The end point of the interval.</param>
    /// <exception cref="ArgumentException">Thrown when either start or end is infinity.</exception>
    /// <exception cref="ReversedIntervalException">Thrown when the end point is less than the start point.</exception>
    public Interval(double start, double end)
    {
        if (double.IsInfinity(start)) throw new ArgumentException($"{nameof(start)} is infinity");
        if (double.IsInfinity(end)) throw new ArgumentException($"{nameof(end)} is infinity");
        if (Accuracy.CompareLength(start, end) == 1) throw new ReversedIntervalException();
        Start = start;
        End = end;
        Span = end - start;
    }
}
