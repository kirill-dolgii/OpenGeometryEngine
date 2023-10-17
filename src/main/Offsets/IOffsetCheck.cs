namespace OpenGeometryEngine.Offsets;

/// <summary>
/// Encapsulates logic to check whether offset is possible or not.
/// </summary>
/// <typeparam name="T"></typeparam>
internal interface IOffsetCheck<T>
    where T : Curve
{
    /// <summary>
    /// Max positive offset value.
    /// </summary>
    public double MaxPositiveOffset { get; }

    /// <summary>
    /// Max negative offset value.
    /// </summary>
    public double MaxNegativeOffset { get; }

    public IPlanarOffsetter<T> Offsetter { get; }

    /// <summary>
    /// Checks whether a given offset value is within the allowable range for offsetting the circle.
    /// </summary>
    /// <param name="offset">The offset value to check.</param>
    /// <returns>
    ///   <c>true</c> if the offset is within the allowable range; otherwise, <c>false</c>.
    /// </returns>
    public bool CanOffset(double offset);
}
