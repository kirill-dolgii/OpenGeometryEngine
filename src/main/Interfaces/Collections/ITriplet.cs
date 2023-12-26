using System.Collections.Generic;

public interface ITriplet<T> : IEnumerable<T>
{
    T First { get; }

    T Second { get; }

    T Third { get; }
}