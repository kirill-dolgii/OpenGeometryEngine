using OpenGeometryEngine.Exceptions.Interval;
using System;
using System.ComponentModel;

namespace OpenGeometryEngine;

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
    public Interval(double start, double end, double tolerance = Accuracy.DefaultDoubleTolerance)
    {
        if (double.IsInfinity(start)) throw new ArgumentException($"{nameof(start)} is infinity");
        if (double.IsInfinity(end)) throw new ArgumentException($"{nameof(end)} is infinity");
        var cmp = Accuracy.CompareWithTolerance(start, end, tolerance);
        if (cmp == 1) throw new ReversedIntervalException();
        if (cmp == 0) throw new IntervalEqualBoundsException(nameof(start), nameof(end));
        Start = start;
        End = end;
        Span = end - start;
    }

    public double Project(double val, double tolerance)
    {
        if (double.IsNaN(val)) throw new DoubleIsNanException(nameof(val));
        if (Accuracy.CompareWithTolerance(val, Start, tolerance) == 1) return Start;
        if (Accuracy.CompareWithTolerance(val, End, tolerance) == -1) return End;
        return val;
    }

    public bool Contains(double param, double tolerance)
    {
        if (double.IsNaN(param)) throw new DoubleIsNanException(nameof(param));
        return Accuracy.WithinRangeWithTolerance(Start, End, param, tolerance);
    }
}
