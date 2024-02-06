using DataStructures.Graph;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace OpenGeometryEngine.Classes.Regions;

public class PolyLineRegion : IFlatRegion
{
    private IBoundedCurve[] _curves { get; }

    private Point[] _points { get; }

    private PolygonRegion _polygon { get; }

    public PolyLineRegion(ICollection<IBoundedCurve> curves)
    {
        Argument.IsNotNull(nameof(curves), curves);
        var gr = new Graph<Point, IBoundedCurve>(false);
        foreach (var curve in curves)
        {
            gr.AddEdge(curve.StartPoint, curve.EndPoint, curve);
        }
        if (gr.Nodes.Any(node => gr.Degree(node) != 2)) throw new System.Exception();
        var chain = gr.DepthTraversal(gr.Nodes.First());
        _curves = chain.Select(tpl =>
        {
            var curve = tpl.Item3;
            switch (curve)
            {
                case LineSegment lineSegment:
                {
                    return new LineSegment(tpl.Item1, tpl.Item2);
                }
                case Arc arc:
                {
                    throw new NotImplementedException(); // TODO: create an arc such that p1
                                                         // is start point and
                                                         // p2 is the endpoint
                }
                default: 
                { 
                    throw new NotImplementedException(); 
                }
            }
        }).ToArray();

    }

    public IEnumerator<IBoundedCurve> GetEnumerator()
    {
        foreach (var curve in _curves) yield return curve;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public double Length { get; }
    public double Area { get; }
    public Plane Plane { get; }
    public ICollection<IFlatRegion> InnerRegions { get; }
    public int WindingNumber(Point point)
    {
        throw new System.NotImplementedException();
    }

    public IFlatRegion Offset(double offset)
    {
        throw new System.NotImplementedException();
    }

    public bool Contains(IFlatRegion other)
    {
        throw new System.NotImplementedException();
    }

    public bool Intersects(IFlatRegion other)
    {
        throw new System.NotImplementedException();
    }
}