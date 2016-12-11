using InvertedPendulumTransporterPhysics.Common;

namespace InvertedPendulumTransporterPhysics.Solvers
{
    /// <summary>
    /// ODE solver class
    /// </summary>
    public class ODESolver : IODESolver
    {
        #region Private Members
        private SolverParameters ODESolverData;
        private IODESolverFunctionStrategy solverFunctionStrategy;
        #endregion

        #region Public Members

        #endregion

        #region Private Methods

        #endregion

        #region Public Methods
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="parameters">Solver parameters</param>
        public ODESolver(SolverParameters parameters)
        {
            SetupStrategy(new StandardSystemODESolverFunctionStrategy());
            UpdateSystemParameters(parameters);
        }

        #region IODESolver Interface
        public void SetupStrategy(IODESolverFunctionStrategy strategy)
        {
            solverFunctionStrategy = strategy;
        }


        public void UpdateSystemParameters(SolverParameters parameters)
        {
            ODESolverData = parameters;
        }

        public OneDimensionalSystemState SolveODESystem(double[] x, double[] y)
        {
            double eps = 0.00001;
            double h = 0;
            alglib.odesolverstate s;
            int m;
            double[] xtbl;
            double[,] ytbl;
            alglib.odesolverreport rep;

            alglib.odesolverrkck(y, x, eps, h, out s);
            alglib.odesolversolve(s, solverFunctionStrategy.ODESolverFunction, ODESolverData);
            alglib.odesolverresults(s, out m, out xtbl, out ytbl, out rep);

            return new OneDimensionalSystemState(ytbl[1, 0], ytbl[1, 1], ytbl[1, 2], ytbl[1, 3]);
        }
        #endregion
        #endregion
    }
}
