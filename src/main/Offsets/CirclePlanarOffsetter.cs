using OpenGeometryEngine.Exceptions;
using System;

namespace OpenGeometryEngine.Offsets; 

public sealed class CirclePlanarOffsetter : IPlanarOffsetter<Circle>
{
    public Circle Curve { get; }

    public Plane Plane { get; }

    public PointUV RelativePoint { get; }

    public bool IsPositive { get; }

    private CirclePlanarOffsetter(Circle circle, Plane plane, PointUV relativePoint)
    {
        Curve = circle ?? throw new ArgumentNullException($"Circle {nameof(circle)} was null.");
        Plane = plane ?? throw new ArgumentNullException($"Plane {nameof(Plane)} was null.");
        RelativePoint = relativePoint;
        var relativeMagnitude = (plane.Evaluate(relativePoint.U, relativePoint.V) - plane.Frame.Origin).Magnitude;
        IsPositive = Accuracy.CompareLength(relativeMagnitude, circle.Radius) == 1;
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="CirclePlanarOffsetter"/> class.
    /// </summary>
    /// <param name="circle">The circle for which the offset is being calculated.</param>
    /// <param name="plane">The plane in which the circle is positioned.</param>
    /// <param name="relativePoint">The relative point used for offset calculation.</param>
    /// <exception cref="ArgumentNullException">
    ///   Thrown when the provided <paramref name="circle"/> or <paramref name="plane"/> is null.
    /// </exception>
    public static CirclePlanarOffsetter Create(Circle circle, Plane plane, PointUV relativePoint) 
        => new(circle, plane, relativePoint);

    /// <summary>
    /// Creates a new offsetted circle based on the specified offset value.
    /// </summary>
    /// <param name="offset">The offset value by which to offset the circle.</param>
    /// <returns>
    ///   An offsetted circle with a radius adjusted by the specified offset value.
    /// </returns>
    /// <exception cref="OffsetFailedException">
    ///   Thrown when the offset operation fails due to constraints on the offset value.
    /// </exception>
    public Circle Offset(double offset)
    {
        var check = CirclePlanarOffsetCheck.Create(this);
        if (!check.CanOffset(offset)) throw new OffsetFailedException(offset);
        var offsettedCircle = Circle.Create(Curve.Frame, Curve.Radius + offset);
        return offsettedCircle;
    }
}

public sealed class CirclePlanarOffsetCheck : IOffsetCheck<Circle>
{
    public double MaxPositiveOffset { get; }

    public double MaxNegativeOffset { get; }

    public IPlanarOffsetter<Circle> Offsetter { get; }

    private CirclePlanarOffsetCheck(IPlanarOffsetter<Circle> offsetter)
    {
        Offsetter = offsetter;
        MaxNegativeOffset = offsetter.IsPositive ? offsetter.Curve.Radius : double.MaxValue;
        MaxPositiveOffset = offsetter.IsPositive ? double.MaxValue : offsetter.Curve.Radius;
    }

    /// <summary>
    /// Creates new CircleOffsetCheck object.
    /// </summary>
    /// <param name="offsetter">Offset description.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided circle is null.</exception>
    public static CirclePlanarOffsetCheck Create(IPlanarOffsetter<Circle> offsetter) => new(offsetter);

    /// <summary>
    /// Checks whether a given <paramref name="offset"/> value is within the allowable offset range.
    /// </summary>
    /// <param name="offset">The offset value to check.</param>
    /// <returns>
    ///   <c>true</c> if the offset is within the allowable range; otherwise, <c>false</c>.
    /// </returns>
    public bool CanOffset(double offset) =>
        Accuracy.CompareLength(offset, MaxNegativeOffset) == 1 &&
        Accuracy.CompareLength(MaxPositiveOffset, offset) == 1;
}