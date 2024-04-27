using netDxf;
using OpenGeometryEngine;
using System;
using System.Collections;
using static OpenGeometryEngine.Misc.Math.Angle;

namespace Translators.Utils;

public static class Converters
{
	public static Vector ToVector(this Vector3 vector3) 
		=> new Vector(vector3.X / 1000, vector3.Y / 1000, vector3.Z / 1000);

	public static Vector ToVector(this Vector2 vector2)
		=> new Vector(vector2.X / 1000, vector2.Y / 1000, 0d);

	public static Point ToPoint(this Vector3 vector3) 
		=> new Point(vector3.X / 1000, vector3.Y / 1000, vector3.Z / 1000);

	public static Arc ToArc(this netDxf.Entities.Arc arc)
	{
		var center = arc.Center.ToVector();
		var transform = Matrix.CreateTranslation(center);

		var start = Accuracy.EqualAngles(GetRadiansFromDegrees(arc.StartAngle), 2 * System.Math.PI) ? 0.0 : GetRadiansFromDegrees(arc.StartAngle);
		var end = GetRadiansFromDegrees(arc.EndAngle);

		if (start > end) start -= 2 * Math.PI;

		var interval = new Interval(start,
									end,
									Accuracy.AngularTolerance);
		return new Arc(transform * Frame.World, arc.Radius / 1000, interval);
	}

	public static LineSegment ToLineSegment(this netDxf.Entities.Line line) 
		=> new(line.StartPoint.ToPoint(), line.EndPoint.ToPoint());

}
