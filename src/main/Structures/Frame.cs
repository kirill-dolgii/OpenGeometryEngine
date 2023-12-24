namespace OpenGeometryEngine;

/// <summary>
/// Represents a coordinate system in 3D space.
/// </summary>
public readonly struct Frame
{
    public static Frame World = new (Point.Origin, Vector.UnitX, Vector.UnitY, Vector.UnitZ);

    public Point Origin { get; }

    /// <summary>
    /// Gets the X-axis unit vector of the coordinate system.
    /// </summary>
    public UnitVec DirX { get; }

    /// <summary>
    /// Gets the Y-axis unit vector of the coordinate system.
    /// </summary>
    public UnitVec DirY { get; }

    /// <summary>
    /// Gets the Z-axis unit vector of the coordinate system.
    /// </summary>
    public UnitVec DirZ { get; }

    /// <summary>
    /// Initializes a new instance of the Frame struct.
    /// </summary>
    /// <param name="origin">The origin point of the coordinate system.</param>
    /// <param name="dirX">The X-axis vector of the coordinate system.</param>
    /// <param name="dirY">The Y-axis vector of the coordinate system.</param>
    /// <param name="dirZ">The Z-axis vector of the coordinate system.</param>
    public Frame(Point origin, Vector dirX, Vector dirY, Vector dirZ) => 
        (Origin, DirX, DirY, DirZ) = (origin, dirX.Unit, dirY.Unit, dirZ.Unit);

    public Frame(Point origin, UnitVec dirX, UnitVec dirY, UnitVec dirZ) =>
        (Origin, DirX, DirY, DirZ) = (origin, dirX, dirY, dirZ);


    public override string ToString() => $"Frame {Origin}, [{DirX}, {DirY}, {DirZ}]";
}