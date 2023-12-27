using OpenGeometryEngine.Collections;
using System.Linq;

namespace OpenGeometryEngine.Fillet;

public sealed class LineSegmentJunction : CurveJunction<LineSegment, LineSegment>
{
    public LineSegmentJunction(LineSegment first, LineSegment second) : base(first, second)
    {
        var firstPoints = new Pair<Point>(first.StartPoint, first.EndPoint);
        var secondPoints = new Pair<Point>(second.StartPoint, second.EndPoint);

        var junction = Iterate.Over(Junction);

        FirstTangent = (firstPoints.Except(junction).Single() - Junction).Unit;
        SecondTangent = (firstPoints.Except(junction).Single() - Junction).Unit;
    }

    public override UnitVec FirstTangent { get; }
    public override UnitVec SecondTangent { get; }
}