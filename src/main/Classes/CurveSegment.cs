using OpenGeometryEngine.Structures;
using System.Collections.Generic;

namespace OpenGeometryEngine;

public abstract class CurveSegment
{
    public Interval Interval { get; }
    
    public Curve Geometry { get; }

    public Point StartPoint { get; }
    
    public Point EndPoint { get; }

    public double Length { get; }

    protected CurveSegment(Interval interval, Curve geometry, 
                           Point startPoint, Point endPoint, 
                           double length)
    {
        Interval = interval;
        Geometry = geometry;
        StartPoint = startPoint;
        EndPoint = endPoint;
        Length = length;
    }

    public abstract ICollection<IntersectionPoint<CurveEvaluation, CurveEvaluation>> IntersectCurve(CurveSegment otherSegment);

    public abstract bool IsCoincident(CurveSegment otherSegment);
}