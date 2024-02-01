using System;
using System.Numerics;

namespace OpenGeometryEngine;

public static class QuadraticEquationSolver
{
    public static ValueTuple<Complex, Complex> Solve(double a, double b, double c)
    {
        if (b == 0d)
        {
            var t = Complex.Sqrt(new Complex(-c / a, 0d));
            return (t, -t);
        }

        var q = -0.5 * (b + Math.Sign(b) * Complex.Sqrt(new Complex(b * b - 4 * a * c, 0d)));

        return (q / a, c / q);
    }

    public static bool TryGetRealRoots(double a, double b, double c,
                                       out double root1, out double root2)
    {
        var roots = Solve(a, b, c);
        if (roots.Item1.Imaginary == 0 && roots.Item2.Imaginary == 0)
        {
            root1 = roots.Item1.Real;
            root2 = roots.Item2.Real;
            return true;
        }
        else
        {
            root1 = double.NaN;
            root2 = double.NaN;
            return false;
        }
    }
}