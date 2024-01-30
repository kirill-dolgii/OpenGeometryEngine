using OpenGeometryEngine.Classes;
using OpenGeometryEngine.Collections;
using OpenGeometryEngine.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OpenGeometryEngine.Fillet;

public static class LinesegmentFillet
{

    public static IList<ITrimmedCurve> Fillet(LineSegment firstSegment, 
                                              LineSegment secondSegment,
                                              double radius)
    {
        if (firstSegment == null) throw new ArgumentNullException(nameof(firstSegment));
        if (secondSegment == null) throw new ArgumentNullException(nameof(secondSegment));

        var commonPoints = firstSegment.StartEndPoints.Intersect(secondSegment.StartEndPoints).ToList();
        
        if (!commonPoints.Any()) throw new CommonPointException(nameof(firstSegment), nameof(secondSegment));
        if (commonPoints.Count() > 1) throw new Exception("Lines are equal");

        var commonPoint = commonPoints.Single();

        var firstTangent = (firstSegment.StartEndPoints.Except(commonPoints).ToList().Single() - commonPoint).Unit;
        var secondTangent = (secondSegment.StartEndPoints.Except(commonPoints).ToList().Single() - commonPoint).Unit;

        var circleCenterVec = (firstTangent + secondTangent) / 2;
        var circleCenter = (circleCenterVec * radius).ToPoint();

        var cross = Vector.Cross(firstTangent, secondTangent).Unit;

        var arc = new Arc(circleCenter, 
            firstSegment.ProjectPoint(circleCenter).Point, 
            secondSegment.ProjectPoint(circleCenter).Point, cross);

    }

}