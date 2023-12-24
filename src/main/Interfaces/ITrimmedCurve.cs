namespace OpenGeometryEngine.Interfaces;

public interface ITrimmedCurve : IBounded, IShape, ISpatial
{
    public Interval Interval { get; }

    public Point StartPoint { get; }

    public Point EndPoint { get; }
    
    public double Length { get; }


}
