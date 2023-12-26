using System;

namespace OpenGeometryEngine;

public class CircleEvaluation : ICurveEvaluation
{
    public CircleEvaluation(Circle circle, double param)
    {
        Circle = circle;
        Param = param;
    }

    public Circle Circle { get; }
    public double Param { get; }
    public Point Point => Circle.Origin + 
                          Circle.Frame.DirX * Circle.Radius * Math.Cos(Param) + 
                          Circle.Frame.DirY * Circle.Radius * Math.Sin(Param);
    public UnitVec Tangent => (Circle.Frame.DirY * Math.Cos(Param) - 
                               Circle.Frame.DirX * Math.Sin(Param)).Unit; // 90 deg rotation
    public double Curvature => 1 / Circle.Radius;
    public Vector Derivative => throw new NotImplementedException();
}