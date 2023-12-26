using System;
using OpenGeometryEngine.Exceptions;

namespace OpenGeometryEngine;

public static class Accuracy
{
    static Accuracy()
    {
        LinearTolerance = DefaultLinearTolerance;
        AngularTolerance = DefaultAngularTolerance;
    }

    public const double DefaultDoubleTolerance = 1E-13;

    public static double LinearTolerance { get; set; }

    public const double DefaultLinearTolerance = 0.00000001;

    public static double AngularTolerance { get; set; }

    public const double DefaultAngularTolerance = 0.000001;

    public static int CompareWithTolerance(double a, double b, double tolerance)
    {
        if (double.IsNaN(a)) throw new DoubleIsNanException(nameof(a));
        if (double.IsNaN(b)) throw new DoubleIsNanException(nameof(b));
        if (WithinTolerance(a, b, tolerance)) return 0;
        return a.CompareTo(b);
    }

    public static bool WithinTolerance(double a, double b, double tolerance)
    {
        if (double.IsNaN(a)) throw new DoubleIsNanException(nameof(a));
        if (double.IsNaN(b)) throw new DoubleIsNanException(nameof(b));
        if (double.IsPositiveInfinity(a) ^ double.IsPositiveInfinity(b) ||
            double.IsNegativeInfinity(a) ^ double.IsNegativeInfinity(b)) return false;
        var abs = Math.Abs(a - b);
        return abs < tolerance;
    }

    public static bool WithinRangeWithTolerance(double start, double end,
        double val, double tolerance)
    {
        if (double.IsNaN(start)) throw new DoubleIsNanException(nameof(start));
        if (double.IsNaN(end)) throw new DoubleIsNanException(nameof(end));
        if (double.IsNaN(val)) throw new DoubleIsNanException(nameof(val));
        if (double.IsNaN(tolerance)) throw new DoubleIsNanException(nameof(tolerance));
        var cmp1 = CompareWithTolerance(start, val, tolerance);
        var cmp2 = CompareWithTolerance(end, val, tolerance);
        if (cmp1 == 1 || cmp2 == -1) return false;
        return true;
    }

    public static bool EqualLengths(double a, double b, double? tolerance)
    {
        tolerance ??= LinearTolerance;
        return CompareWithTolerance(a, b, tolerance.Value) == 0;
    }

    public static bool EqualLengths(double a, double b) => EqualLengths(a, b, null);

    public static bool EqualAngles(double a, double b, double? tolerance)
    {
        tolerance ??= LinearTolerance;
        return CompareWithTolerance(a, b, tolerance.Value) == 0;
    }

    public static bool EqualAngles(double a, double b) => EqualAngles(a, b, null);

    public static bool WithinAngleInterval(Interval interval, double param)
        => WithinRangeWithTolerance(interval.Start, interval.End, param, AngularTolerance);

    public static bool WithinLengthInterval(Interval interval, double param)
        => WithinRangeWithTolerance(interval.Start, interval.End, param, LinearTolerance);

    public static bool LengthIsZero(double length) => Math.Abs(length) < LinearTolerance;

    public static bool AngleIsZero(double angle) => Math.Abs(angle) < AngularTolerance;
}