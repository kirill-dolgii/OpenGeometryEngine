using System;

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

    //public static bool LengthIsZero(double value)
    //    => Math.Abs(value) < LinearTolerance;
    
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


    ///// <summary>
    ///// 1 => a > b, -1 => a < b, 0 => a == b
    ///// </summary>
    ///// <param name="a"></param>
    ///// <param name="b"></param>
    ///// <returns></returns>
    //public static int CompareLength(double a, double b)
    //{
    //    var difference = a - b;
    //    if (Math.Abs(difference) < LinearTolerance) return 0;
    //    return Math.Sign(difference);
    //}

    //public static bool EqualLengths(double a, double b)
    //    => CompareLength(a, b) == 0;

    //public static bool AngleIsZero(double value)
    //    => Math.Abs(value) < AngularTolerance;

    //public static int CompareAngles(double a, double b)
    //{
    //    var difference = a - b;
    //    if (Math.Abs(difference) < AngularTolerance) return 0;
    //    return Math.Sign(difference);
    //}

    //public static bool EqualAngles(double a, double b)
    //    => CompareAngles(a, b) == 0;

    //public static bool WithinLengthBounds(Bounds bounds, double param)
    //{
    //    if (bounds.Equals(Bounds.Unbounded)) return true;
    //    var withinStart = bounds.Start.HasValue &&
    //        Accuracy.CompareLength(param, bounds.Start.Value) == 1;
    //    var withinEnd = bounds.End.HasValue &&
    //        Accuracy.CompareLength(bounds.End.Value, param) == 1;
    //    return withinStart && withinEnd;
    //}

    //public static bool WithinAngleBounds(Bounds bounds, double param)
    //{
    //    if (bounds.Equals(Bounds.Unbounded)) return true;
    //    var withinStart = bounds.Start.HasValue &&
    //        Accuracy.CompareAngles(param, bounds.Start.Value) == 1;
    //    var withinEnd = bounds.End.HasValue &&
    //        Accuracy.CompareAngles(bounds.End.Value, param) == 1;
    //    return withinStart && withinEnd;
    //}

    //private static bool WithinRange(double start, double end, 
    //                                double param, Func<double, double, int> compFunc)
    //{
    //    if (compFunc == null) throw new ArgumentNullException(nameof(compFunc));
    //    var withinStart = compFunc.Invoke(param, start) >= 0;
    //    var withinEnd = compFunc.Invoke(end, param) >= 0;
    //    return withinStart && withinEnd;
    //}

    //public static bool WithinLengthInterval(Interval interval, double param)
    //    => WithinRange(interval.Start, interval.End, param, CompareLength);

    //public static bool WithinAngleInterval(Interval interval, double param)
    //    => WithinRange(interval.Start, interval.End, param, CompareAngles);
}
