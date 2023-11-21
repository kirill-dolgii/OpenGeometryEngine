using OpenGeometryEngine.Interfaces;
using OpenGeometryEngine.Structures;
using System.Collections.Generic;

namespace OpenGeometryEngine;

public abstract class CurveSegment : ITrimmedCurve
{
    public Interval Interval { get; }
    
    public Curve Geometry { get; }

    public Point StartPoint { get; }
    
    public Point EndPoint { get; }

    public double Length { get; }
    public abstract TCurve GetGeometry<TCurve>() where TCurve : Curve;

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

    public abstract Box GetBoundingBox();

    public abstract ICollection<IntersectionPoint<CurveEvaluation, CurveEvaluation>> IntersectCurve(CurveSegment otherSegment);

    public abstract bool IsCoincident(CurveSegment otherSegment);

    public abstract CurveSegment CreateTransformedCopy(Matrix matrix);


}