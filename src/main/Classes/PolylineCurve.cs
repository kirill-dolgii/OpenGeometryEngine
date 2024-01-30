using System;
using System.Collections;
using System.Collections.Generic;

namespace OpenGeometryEngine.Classes;

public class PolylineCurve : IEnumerable<ITrimmedCurve>
{
    private ITrimmedCurve[] _curves = { };

    private PolylineCurve() { }

    public PolylineCurve(IEnumerable<ITrimmedCurve> curves)
    {
        //check arguments
        //check for self-intersections
        //calculate fields
    }

    public IEnumerator<ITrimmedCurve> GetEnumerator() => Iterate().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Iterate().GetEnumerator();

    private IEnumerable<ITrimmedCurve> Iterate()
    {
        foreach (ITrimmedCurve curve in _curves) yield return curve;
    }


}
