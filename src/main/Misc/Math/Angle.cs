namespace OpenGeometryEngine.Misc;

public static class Angle
{
	public static double GetDegreesFromRadians(double radians) => (radians / System.Math.PI) * 180;

	public static double GetRadiansFromDegrees(double degrees) => (degrees / 180) * System.Math.PI;
}
