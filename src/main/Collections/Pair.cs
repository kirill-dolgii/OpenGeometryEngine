using System.Collections;
using System.Collections.Generic;

public class Pair<T> : IEnumerable<T>
{
    public T First { get; }

    public T Second { get; }
    
    public Pair(T first, T second)
    {
        First = first;
        Second = second;
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
    }

    public void UnPack(out T first, out T second) => (first, second) = (First, Second);
}