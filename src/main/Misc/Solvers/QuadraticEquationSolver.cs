using OpenGeometryEngine.Misc.Solvers;
using System;
using System.Numerics;

namespace OpenGeometryEngine;

public class QuadraticEquationSolver : ISolver<Pair<Complex>>
{
    public double a { get; }

    public double b { get; }

    public double c { get; }

    private bool _solved = false;
    private Pair<Complex>? _result;

	public Pair<Complex>? Result => throw new NotImplementedException();

	public bool Solved => throw new NotImplementedException();

	public QuadraticEquationSolver(double a, double b, double c) 
        => (this.a, this.b, this.c, _result) = (a, b, c, null);

	public Pair<Complex> Solve()
	{
        if (Solved && Result != null) return Result;

		if (b == 0d)
		{
			var t = Complex.Sqrt(new Complex(-c / a, 0d));
			_result = new (t, -t);
		}

		var q = -0.5 * (b + Math.Sign(b) * Complex.Sqrt(new Complex(b * b - 4 * a * c, 0d)));

		_result = new(q / a, c / q);

		return Result;
	}

	public bool TryGetRealRoots(out double root1, out double root2)
    {
        var roots = Solve();
        if (roots.First.Imaginary == 0 && roots.Second.Imaginary == 0)
        {
            root1 = roots.First.Real;
            root2 = roots.Second.Real;
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