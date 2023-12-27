using System;
using System.Linq;

namespace OpenGeometryEngine;

public abstract class CurveJunction<TCurve1, TCurve2> 
    where TCurve1 : ITrimmedCurve
    where TCurve2 : ITrimmedCurve
{
    protected CurveJunction(TCurve1 first, TCurve2 second)
    {
        First = first ?? throw new ArgumentNullException(nameof(first));
        Second = second ?? throw new ArgumentNullException(nameof(second));
        var firstPoints = new Pair<Point>(first.StartPoint, first.EndPoint);
        var secondPoints = new Pair<Point>(second.StartPoint, second.EndPoint);
        var junctions = firstPoints.Intersect(secondPoints).ToList();
        if (!junctions.Any()) throw new CommonPointException(nameof(first), nameof(second));
        if (junctions.Count > 1) throw new Exception("Found more than 1 common point");
        Junction = junctions.Single();
    }

    public TCurve1 First { get; }

    public TCurve2 Second { get; }

    public Point Junction { get; }

    public abstract UnitVec FirstTangent { get; }

    public abstract UnitVec SecondTangent { get; }
}