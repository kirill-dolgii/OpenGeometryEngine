namespace OpenGeometryEngine.Fillet;

public struct FilletCheck
{
    internal FilletCheck(IBoundedCurve first, 
                         IBoundedCurve second, 
                         double maxFilletRadius)
    {
        First = first;
        Second = second;
        MaxFilletRadius = maxFilletRadius;
    }

    public IBoundedCurve First { get; }

    public IBoundedCurve Second { get; }

    public double MaxFilletRadius { get; }
}