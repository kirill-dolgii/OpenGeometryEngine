using System.Collections.Generic;

public interface IPair<T> : IEnumerable<T>
{
    T First { get; }
    T Second { get; }
}