using InvertedPendulumTransporterPhysics.Common;

namespace InvertedPendulumTransporterPhysics.Solvers
{
    /// <summary>
    /// Strategy for dynamics system with interferences
    /// </summary>
    public class InterferedSystemODESolverFunctionStrategy : IODESolverFunctionStrategy
    {
        #region Private Members

        #endregion

        #region Public Members

        #endregion

        #region Private Methods

        #endregion

        #region Public Methods
        #region IODESolverFunctionStrategy Interface
        public void ODESolverFunction(double[] y, double x, double[] dy, object obj)
        {
            // y: x, theta, dx, dtheta
            // dy: dx, dtheta, d2x, d2theta 
            // A = [ 0       0       1    0 ]  B = [    0    ] Noise = [              0                ]
            //     [ 0       0       0    1 ]      [    0    ]         [              0                ]
            //     [ 0    -mG/M   -g_2/M  0 ]      [  g_1/M  ]         [    (F_x - F_y*theta) / M      ]
            //     [ 0 (M + m)g/Ml g_2/Ml 0 ]      [ -g_1/Ml ]         [ (F_y*theta - F_x)(M + m)/(Mm) ]
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
        #endregion
        #endregion
    }
}
