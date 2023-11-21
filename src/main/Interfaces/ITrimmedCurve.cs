namespace OpenGeometryEngine.Interfaces;

public interface ITrimmedCurve
{
    public Interval Interval { get; }

    public Point StartPoint { get; }

    public Point EndPoint { get; }

    public Curve Geometry { get; }

    public double Length { get; }

    public TCurve GetGeometry<TCurve>() where TCurve : Curve;


}
