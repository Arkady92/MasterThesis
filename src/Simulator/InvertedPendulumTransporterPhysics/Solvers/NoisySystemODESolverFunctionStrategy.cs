using InvertedPendulumTransporterPhysics.Common;

namespace InvertedPendulumTransporterPhysics.Solvers
{
    public class NoisySystemODESolverFunctionStrategy : IODESolverFunctionStrategy
    {
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
    }
}
