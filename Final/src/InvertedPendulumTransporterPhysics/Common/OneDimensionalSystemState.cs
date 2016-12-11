namespace InvertedPendulumTransporterPhysics.Common
{
    /// <summary>
    /// Container for subsystem state
    /// </summary>
    public class OneDimensionalSystemState
    {
        #region Private Members

        #endregion

        #region Public Members
        /// <summary>
        /// Cart position on axis
        /// </summary>
        public double Position { get; set; }

        /// <summary>
        /// Cart velocity
        /// </summary>
        public double Velocity { get; set; }
        
        /// <summary>
        /// Pendulum angle
        /// </summary>
        public double Angle { get; set; }

        /// <summary>
        /// Pendulum angular velocity
        /// </summary>
        public double AngularVelocity { get; set; }
        #endregion

        #region Private Methods

        #endregion

        #region Public Methods

        /// <summary>
        /// Class constructor
        /// </summary>
        public OneDimensionalSystemState()
        {
            InitializeDefault();
        }

        /// <summary>
        /// Class constructor with initialization
        /// </summary>
        /// <param name="position">Initial cart position</param>
        /// <param name="angle">Initial pendulum angle</param>
        /// <param name="velocity">Intial cart velocity</param>
        /// <param name="angularVelocity">Initial pendulum angular velocity</param>
        public OneDimensionalSystemState(double position, double angle, double velocity, double angularVelocity)
        {
            Position = position;
            Angle = angle;
            Velocity = velocity;
            AngularVelocity = angularVelocity;
        }

        /// <summary>
        /// Initialization with default parameters (except angle)
        /// </summary>
        public void InitializeDefault()
        {
            Position = 0.0;
            Velocity = 0.0;
            AngularVelocity = 0.0;
        }

        /// <summary>
        /// Convert system state to array
        /// </summary>
        /// <returns>Array of system parameters</returns>
        public double[] ToStateArray()
        {
            return new double[] { Position, Angle, Velocity, AngularVelocity };
        }
        #endregion
    }
}
