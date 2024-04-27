using netDxf;
using OpenGeometryEngine;
using System;

using static OpenGeometryEngine.Misc.Math.Angle;

namespace Translators.Utils;

internal static class BackWardsConverters
{
	public static Vector3 ToVector3(this Vector vector)
		=> new Vector3(vector.X * 1000, vector.Y * 1000, vector.Z * 1000);

	public static Vector2 ToVector2(this Vector vector)
		=> new Vector2(vector.X * 1000, vector.Y * 1000);

	public static Vector3 ToVector3(this Point point) 
		=> new Vector3(point.X * 1000, point.Y * 1000, point.Z * 1000);

	public static Vector2 ToVector2(this Point point)
		=> new Vector2(point.X * 1000, point.Y * 1000);

	public static netDxf.Entities.Arc ToArc(this Arc arc)
		=> new netDxf.Entities.Arc(arc.Circle.Frame.Origin.ToVector2(), 
								   arc.Circle.Radius * 1000,
								   GetDegreesFromRadians(arc.Interval.Start), 
								   GetDegreesFromRadians(arc.Interval.End));

	public static netDxf.Entities.Line ToLine(this LineSegment ls)
		=> new netDxf.Entities.Line(ls.StartPoint.ToVector3(), ls.EndPoint.ToVector3());

	public static netDxf.Entities.EntityObject ToDxfEntity(IBoundedCurve curve)
	{
		return curve switch
		{
			LineSegment ls => ToLine(ls),
			Arc arc => ToArc(arc),
			_ => throw new NotImplementedException()
		};
	}
}
