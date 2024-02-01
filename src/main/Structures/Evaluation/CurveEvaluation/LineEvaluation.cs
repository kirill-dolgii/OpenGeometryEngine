using System;

namespace OpenGeometryEngine;

public readonly struct LineEvaluation : ICurveEvaluation
{
    public LineEvaluation(Line line, double param)
    {
        Line = line;
        Param = param;
        Point = Line.Origin + Line.Direction * param;
    }

    public Line Line { get; }
    public double Param { get; }
    public Point Point { get; }
    public UnitVec Tangent => throw new NotImplementedException();
    public double Curvature => 0.0;
    public Vector Derivative => throw new NotImplementedException();
}
