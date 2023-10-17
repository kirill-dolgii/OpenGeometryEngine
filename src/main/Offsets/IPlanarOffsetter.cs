namespace OpenGeometryEngine.Offsets;

/// <summary>
/// Encapsulates information needed to check and perform planar offset.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IPlanarOffsetter<T>
{
    /// <summary>
    /// Curve to perform offset at.
    /// </summary>
    public T Curve { get; }

    /// <summary>
    /// Plane in what offsetting will be performed.
    /// </summary>
    public Plane Plane { get; }

    /// <summary>
    /// Point to check whether offset is positive or negative.
    /// </summary>
    public PointUV RelativePoint { get; }

    /// <summary>
    /// Shows whether offset is positive relative to the RelativePoint specified e.g. for
    /// Circle it is positive if offset applied increases the radius of the Circle.
    /// </summary>
    public bool IsPositive { get; }

    public T Offset(double offset);
}

