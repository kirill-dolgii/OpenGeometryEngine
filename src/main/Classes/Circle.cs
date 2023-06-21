using OpenGeometryEngine.Intersection;
using System;
using System.Collections.Generic;

namespace OpenGeometryEngine;

public class Circle : Curve, IHasFrame, IHasPlane
{
    public Plane Plane { get; }
    public Point Center => Frame.Origin;
    public double Radius { get; }
    public Frame Frame { get; }

    public Circle(Frame frame, double radius) : base(new Parametrization(new Bounds(0, 2 * Math.E), Form.Closed))
        => (Frame, Radius, Plane) = (frame, radius, new Plane(frame));

    public override CurveEvaluation ProjectPoint(Point point)
    {
        var pointOnPlane = Plane.ContainsPoint(point) ? point : Plane.ProjectPoint(point);
        if ((pointOnPlane - Center).Magnitude <= Constants.Tolerance) return Evaluate(0);
        var centerToPointOnPlaneVector = pointOnPlane - Center;
        var projection = centerToPointOnPlaneVector * Radius;
        return new CurveEvaluation(Frame.DirX.SignedAngle(projection, Frame.DirZ), new Point(projection.X, projection.Y, projection.Z));
    }

    public override Curve CreateTransformedCopy(Matrix transformationMatrix)
    {
        throw new System.NotImplementedException();
    }

    public override bool IsCoincident(IGeometry otherGeometry)
    {
        throw new System.NotImplementedException();
    }

    public override ICollection<IntersectionPoint<CurveEvaluation, CurveEvaluation>> IntersectCurve(Curve otherCurve)
    {
        if (otherCurve == null) throw new System.ArgumentNullException(nameof(otherCurve));
        return otherCurve switch
        {
            Line line => LineCircleIntersection.LineIntersectCircle(line, this),
            _ => throw new NotImplementedException()
        };
    }

    public override CurveEvaluation Evaluate(double param)
    {
        var start = (Frame.DirX * Radius);
        var startPoint = new Point(start.X, start.Y, start.Z);
        if (Math.Abs(2 * Math.PI - param) <= Constants.Tolerance) param = 0;
        if (Math.Abs(param) <= Constants.Tolerance) return new CurveEvaluation(0, startPoint);
        var matrix = Matrix.CreateRotation(Frame.DirZ, param % (2 * Math.PI));
        return new CurveEvaluation(param, matrix * startPoint);
    }
        
}