namespace InvertedPendulumTransporterPhysics.Solvers
{
    /// <summary>
    /// Interface dedicated to ODE solving function strategies
    /// </summary>
    public interface IODESolverFunctionStrategy
    {
        /// <summary>
        /// Function of solving the state-space equations in differential form
        /// </summary>
        /// <param name="y">System state array</param>
        /// <param name="x">Time stamps array</param>
        /// <param name="dy">System state derivatives array</param>
        /// <param name="obj">Solver parameters</param>
        void ODESolverFunction(double[] y, double x, double[] dy, object obj);
    }
}
