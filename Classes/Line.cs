namespace OpenGeometryEngine;
    
/// <summary>
/// Represents an infinite 3D line defined by an origin point and a direction vector.
/// </summary>
public class Line : ISpatial
{
    /// <summary>
    /// The origin point of the line.
    /// </summary>
    public Point Origin { get; }
    /// <summary>
    /// The direction unit vector of the line.
    /// </summary>
    public Vector Direction { get; }

    /// <summary>
    /// Initializes a new Line instance with the specified origin and direction.
    /// </summary>
    /// <param name="origin">The origin point of the line.</param>
    /// <param name="direction">The direction vector of the line.</param>
    public Line(Point origin, Vector direction)
    {
        Origin = origin;
        Direction = direction.Normalize();
    }

    /// <summary>
    /// Checks if the line contains the specified point.
    /// </summary>
    /// <param name="point">The point to check for containment.</param>
    /// <returns><c>true</c> if the line contains the point, otherwise <c>false</c>.</returns>
    public bool ContainsPoint(Point point)
    {
        Vector vectorToPoint = point - Origin;
        double scalarProjection = Vector.Dot(vectorToPoint, Direction);
        return scalarProjection == 0; // The point is on the line if the scalar projection is zero.
    }

    /// <summary>
    /// Projects a point onto the line, returning the closest point on the line to the given point.
    /// </summary>
    /// <param name="point">The point to project onto the line.</param>
    /// <returns>The projected point on the line.</returns>
    public Point ProjectPoint(Point point)
    {
        Vector originToPoint = point - Origin;
        double t = Vector.Dot(originToPoint, Direction); // Direction is a unit vector.
        return Evaluated(t);
    }

    /// <summary>
    /// Evaluates the line at the given parameter.
    /// </summary>
    /// <param name="param">Local coordinate.</param>
    /// <returns>Evaluated point at the specified parameter.</returns>

    public Point Evaluated(double param) => Origin + Direction * param;
}