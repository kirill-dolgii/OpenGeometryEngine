using System;

namespace OpenGeometryEngine;

public static class Accuracy
{
    static Accuracy()
    {
        LinearTolerance = DefaultLinearTolerance;
        AngularTolerance = DefaultAngularTolerance;
    }

    public static double LinearTolerance { get; set; }

    public const double DefaultLinearTolerance = 0.00000001;

    public static double AngularTolerance { get; set; }

    public const double DefaultAngularTolerance = 0.000001;

    public static bool LengthIsZero(double value)
        => Math.Abs(value) < LinearTolerance;

    /// <summary>
    /// 1 => a > b, -1 => a < b, 0 => a == b
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static int CompareLength(double a, double b)
    {
        var difference = a - b;
        if (Math.Abs(difference) < LinearTolerance) return 0;
        return Math.Sign(difference);
    }

    public static bool EqualLengths(double a, double b)
        => CompareLength(a, b) == 0;

    public static bool AngleIsZero(double value)
        => Math.Abs(value) < AngularTolerance;

    public static int CompareAngles(double a, double b)
    {
        var difference = a - b;
        if (Math.Abs(difference) < AngularTolerance) return 0;
        return Math.Sign(difference);
    }

    public static bool EqualAngles(double a, double b)
        => CompareLength(a, b) == 0;

    public static bool WithinLengthBounds(Bounds bounds, double param)
    {
        if (bounds.Equals(Bounds.Unbounded)) return true;
        var withinStart = bounds.Start.HasValue &&
            Accuracy.CompareLength(param, bounds.Start.Value) == 1;
        var withinEnd = bounds.End.HasValue &&
            Accuracy.CompareLength(bounds.End.Value, param) == 1;
        return withinStart && withinEnd;
    }

    public static bool WithinAngleBounds(Bounds bounds, double param)
    {
        if (bounds.Equals(Bounds.Unbounded)) return true;
        var withinStart = bounds.Start.HasValue &&
            Accuracy.CompareAngles(param, bounds.Start.Value) == 1;
        var withinEnd = bounds.End.HasValue &&
            Accuracy.CompareAngles(bounds.End.Value, param) == 1;
        return withinStart && withinEnd;
    }

    private static bool WithinRange(double start, double end, 
                                    double param, Func<double, double, int> compFunc)
    {
        if (compFunc == null) throw new ArgumentNullException(nameof(compFunc));
        var withinStart = compFunc.Invoke(param, start) == 1;
        var withinEnd = compFunc.Invoke(end, param) == 1;
        return withinStart && withinEnd;
    }

    public static bool WithinLengthInterval(Interval interval, double param)
        => WithinRange(interval.Start, interval.End, param, CompareLength);

    public static bool WithinAngleInterval(Interval interval, double param)
        => WithinRange(interval.Start, interval.End, param, CompareAngles);
}
