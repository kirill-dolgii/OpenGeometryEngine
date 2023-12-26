using System.Collections;
using System.Collections.Generic;

public class Triplet<T> : IEnumerable<T>
{
    public T First { get; }

    public T Second { get; }

    public T Third { get; }

    public Triplet(T first, T second, T third)
    {
        First = first;
        Second = second;
        Third = third;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return Walk().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private IEnumerable<T> Walk()
    {
        yield return First;
        yield return Second;
        yield return Third;
    }
}