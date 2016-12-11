using InvertedPendulumTransporterPhysics.Common;

namespace InvertedPendulumTransporterPhysics.Solvers
{
    /// <summary>
    /// Interface dedicated to ODE solver
    /// </summary>
    public interface IODESolver
    {
        /// <summary>
        /// Update solver parameters
        /// </summary>
        /// <param name="parameters">New solver parameters</param>
        void UpdateSystemParameters(SolverParameters parameters);

        /// <summary>
        /// Solve ordinary differential equation (ODE)
        /// </summary>
        /// <param name="x">System state array</param>
        /// <param name="y">Time stamps array</param>
        /// <returns>New system state</returns>
        OneDimensionalSystemState SolveODESystem(double[] x, double[] y);

        /// <summary>
        /// Setup solving strategy
        /// </summary>
        /// <param name="strategy">Strategy of solving equations</param>
        void SetupStrategy(IODESolverFunctionStrategy strategy);
    }
}
