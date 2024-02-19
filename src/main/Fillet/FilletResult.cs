using System.Collections;
using System.Collections.Generic;

namespace OpenGeometryEngine.Fillet;

public struct FilletResult
{
    internal FilletResult(IBoundedCurve? first, IBoundedCurve? second, 
                          FilletType type, Arc? fillet, 
                          ICollection<IBoundedCurve>? result)
    {
        First = first;
        Second = second;
        Type = type;
        Fillet = fillet;
        Result = result;
    }

    public IBoundedCurve? First { get; }

    public Arc? Fillet { get; }

    public IBoundedCurve? Second { get; }

    public FilletType Type { get; }

    public ICollection<IBoundedCurve>? Result { get; }
}