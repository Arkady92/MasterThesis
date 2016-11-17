using InvertedPendulumTransporterPhysics.Common;

namespace InvertedPendulumTransporterPhysics.Solvers
{
    public class ODESolver
    {
        private SolverParameters ODESolverData;

        public ODESolver(SolverParameters parameters)
        {
            UpdateSystemParameters(parameters);
        }

        public void UpdateSystemParameters(SolverParameters parameters)
        {
            ODESolverData = parameters;
        }

        public void ODESolverFunction(double[] y, double x, double[] dy, object obj)
        {
            // y: x, theta, dx, dtheta
            // dy: dx, dtheta, d2x, d2theta
            var data = obj as SolverParameters;
            double Fw_X = data.HorizontalWindForce;
            double Fw_Z = data.VerticalWindForce;
            dy[0] = y[2];
            dy[1] = y[3];
            dy[2] = (-data.PendulumMass * SolverParameters.G / data.CartMass) * y[1] + (-data.Gamma2 / data.CartMass) * y[2]
                + (data.Gamma1 / data.CartMass * data.Voltage)
                + ((Fw_X - Fw_Z * y[1]) / data.CartMass);
            dy[3] = ((data.CartMass + data.PendulumMass) * SolverParameters.G / data.CartMass / data.PendulumLength) * y[1]
                + (data.Gamma2 / data.CartMass / data.PendulumLength) * y[2]
                + (-data.Gamma1 / data.CartMass / data.PendulumLength * data.Voltage)
                + ((Fw_Z * y[1] - Fw_X) * (data.CartMass + data.PendulumMass) / (data.CartMass * data.PendulumMass));
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
            alglib.odesolversolve(s, ODESolverFunction, ODESolverData);
            alglib.odesolverresults(s, out m, out xtbl, out ytbl, out rep);

            return new OneDimensionalSystemState(ytbl[1, 0], ytbl[1, 1], ytbl[1, 2], ytbl[1, 3]);
        }
    }
}
