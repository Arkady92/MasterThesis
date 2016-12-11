namespace InvertedPendulumTransporterPhysics.Common
{
    /// <summary>
    /// Container for solver parameters
    /// </summary>
    public class SolverParameters
    {
        #region Private Members

        #endregion

        #region Public Members
        /// <summary>
        /// Pendulum mass
        /// </summary>
        public double PendulumMass { get; set; }

        /// <summary>
        /// Cart mass
        /// </summary>
        public double CartMass { get; set; }

        /// <summary>
        /// Pendulum Length
        /// </summary>
        public double PendulumLength { get; set; }

        /// <summary>
        /// Motor voltage
        /// </summary>
        public double Voltage { get; set; }

        /// <summary>
        /// Cart friction factor
        /// </summary>
        public double Gamma1 { get; set; }

        /// <summary>
        /// Voltage conversion factor
        /// </summary>
        public double Gamma2 { get; set; }

        /// <summary>
        /// Horizontal wind force
        /// </summary>
        public double HorizontalWindForce { get; set; }

        /// <summary>
        /// Vertical wind force
        /// </summary>
        public double VerticalWindForce { get; set; }

        /// <summary>
        /// Gravity acceleration
        /// </summary>
        public const double G = 9.83;
        #endregion

        #region Private Methods

        #endregion

        #region Public Methods
        /// <summary>
        /// Class constructor
        /// </summary>
        public SolverParameters()
        {
            PendulumMass = 0.231;
            CartMass = 0.792;
            PendulumLength = 0.610;
            Gamma1 = 1.72;
            Gamma2 = 7.68;
            Voltage = 0.0;
            HorizontalWindForce = 0.0;
            VerticalWindForce = 0.0;
        }
        #endregion
    }
}
