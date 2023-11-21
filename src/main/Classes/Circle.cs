using OpenGeometryEngine.Intersection.Unbounded;
using System;
using System.Collections.Generic;

namespace OpenGeometryEngine;

public sealed class Circle : Curve, IHasFrame, IHasPlane
{
    public Plane Plane { get; }
    public Point Center => Frame.Origin;
    public double Radius { get; }
    public Frame Frame { get; }

    private Circle(Frame frame, double radius) : base(new Parametrization(new Bounds(0, 2 * Math.PI), 
                                                                         Form.Periodic))
        => (Frame, Radius, Plane) = (frame, radius, new Plane(frame));

    public static Circle Create(Frame frame, double radius)
    {
        if (radius < 0 || Accuracy.LengthIsZero(radius)) 
            throw new ArgumentException("circle must be a positive non-zero value");
        return new Circle(frame, radius);
    }

    public override CurveEvaluation ProjectPoint(Point point)
    {
        var pointOnPlane = Plane.ContainsPoint(point) ? point : Plane.ProjectPoint(point);         
        if (pointOnPlane == Center) return Evaluate(0);
        var centerToPointOnPlaneVector = (pointOnPlane - Center).Normalize();
        var projection = centerToPointOnPlaneVector * Radius + Center.Vector;
        return new CurveEvaluation(Frame.DirX.SignedAngle(projection - Center.Vector, Frame.DirZ), 
                                   new Point(projection.X, projection.Y, projection.Z));
    }

    public override bool IsCoincident(Curve otherCurve)
    {
        var otherCircle = otherCurve as Circle;
        if (otherCircle == null) return false;
        return Center == otherCircle.Center &&
               Accuracy.EqualLengths(Radius, otherCircle.Radius);

    }

    public override ICollection<IntersectionPoint<CurveEvaluation, CurveEvaluation>> 
        IntersectCurve(Curve otherCurve)
    {
        if (otherCurve == null) throw new ArgumentNullException(nameof(otherCurve));
        return otherCurve switch
        {
            Line line => LineCircleIntersection.LineIntersectCircle(line, this),
            _ => throw new NotImplementedException()
        };
    }

    public override CurveEvaluation Evaluate(double param)
    {
        var startPoint = Frame.Origin + Frame.DirX * Radius;
        if (Accuracy.EqualAngles(2 * Math.PI, param)) param = 0;
        if (Accuracy.AngleIsZero(param)) return new CurveEvaluation(0, startPoint);
        var matrix = Matrix.CreateRotation(Frame.DirZ, param % (2 * Math.PI));
        return new CurveEvaluation(param, matrix * startPoint);
    }

    public override bool ContainsParam(double param)
        => Accuracy.WithinAngleBounds(Parametrization.Bounds, param);

    public Circle CreateTransformedCopy(Matrix matrix)
    {
        var newFrame = matrix * Frame;
        var ret = Circle.Create(newFrame, Radius);
        return ret;
    }
}