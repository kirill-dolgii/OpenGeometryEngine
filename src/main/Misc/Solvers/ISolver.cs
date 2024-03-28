namespace OpenGeometryEngine.Misc.Solvers;

public interface ISolver<T>
{
	public abstract T Solve();

	public T? Result { get; }

	public bool Solved { get; }
}
