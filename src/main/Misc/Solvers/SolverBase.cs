namespace OpenGeometryEngine.Misc.Solvers;

public abstract class SolverBase<T>
{
	public abstract T Solve();

	public T? Result { get; protected set; }

	public bool Solved { get; protected set; }
}
