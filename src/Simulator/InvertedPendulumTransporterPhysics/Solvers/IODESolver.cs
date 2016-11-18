using InvertedPendulumTransporterPhysics.Common;

namespace InvertedPendulumTransporterPhysics.Solvers
{
    public interface IODESolver
    {
        void UpdateSystemParameters(SolverParameters parameters);
        OneDimensionalSystemState SolveODESystem(double[] x, double[] y);
    }
}
