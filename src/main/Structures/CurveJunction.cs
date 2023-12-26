using System;
using System.Linq;

namespace OpenGeometryEngine;

public readonly struct CurveJunction
{
    public CurveJunction(ITrimmedCurve first, ITrimmedCurve second) : this()
    {
        First = first ?? throw new ArgumentNullException(nameof(first));
        Second = second ?? throw new ArgumentNullException(nameof(second));
        var firstPoints = new Pair<Point>(first.StartPoint, first.EndPoint);
        var secondPoints = new Pair<Point>(second.StartPoint, second.EndPoint);
        var junctions = firstPoints.Intersect(secondPoints).ToList();
        if (!junctions.Any()) throw new CommonPointException(nameof(first), nameof(second));
        if (junctions.Count > 1) throw new Exception("Found more than 1 common point");
        Junction = junctions.Single();
        FirstTangent = (firstPoints.Except(junctions).Single() - Junction).Unit;
        SecondTangent = (secondPoints.Except(junctions).Single() - Junction).Unit;
    }

    public ITrimmedCurve First { get; }

    public ITrimmedCurve Second { get; }

    public Point Junction { get; }

    public UnitVec FirstTangent { get; }

    public UnitVec SecondTangent { get; }
}