using System;
using System.Windows.Media.Media3D;

namespace InvertedPendulumTransporterPhysics.Common
{
    /// <summary>
    /// Container for the whole system state
    /// </summary>
    public class SystemState
    {
        #region Private Members

        #endregion

        #region Public Members
        /// <summary>
        /// X-coordinate subsystem state
        /// </summary>
        public OneDimensionalSystemState StateX { get; private set; }

        /// <summary>
        /// Y-coordinate subsystem state
        /// </summary>
        public OneDimensionalSystemState StateY { get; private set; }

        /// <summary>
        /// Last X-coordinate subsystem state
        /// </summary>
        public OneDimensionalSystemState LastStateX { get; private set; }

        /// <summary>
        /// Last Y-coordinate subsystem state
        /// </summary>
        public OneDimensionalSystemState LastStateY { get; private set; }

        /// <summary>
        /// Max pendulum angle
        /// </summary>
        public double MaxAngle { get { return Math.PI / 2.0 * 0.75; } }

        /// <summary>
        /// Min pendulum angle
        /// </summary>
        public double MinAngle { get { return -Math.PI / 2.0 * 0.75; } }

        /// <summary>
        /// Default simulation time delta
        /// </summary>
        public double DefaultTimeDelta = 0.01;

        /// <summary>
        /// Current simulation time
        /// </summary>
        public double Time { get; set; }

        /// <summary>
        /// Current simulation time delta
        /// </summary>
        public double TimeDelta { get; set; }

        /// <summary>
        /// Current solver parameters
        /// </summary>
        public SolverParameters SolverParameters { get; set; }
        #endregion

        #region Private Methods

        #endregion

        #region Public Methods
        /// <summary>
        /// Class constructor
        /// </summary>
        public SystemState()
        {
            Reset();
            SolverParameters = new SolverParameters();
        }

        /// <summary>
        /// Reset system parameters to default
        /// </summary>
        public void ResetSystemParameters()
        {
            SolverParameters = new SolverParameters();
        }

        /// <summary>
        /// Reset system state
        /// </summary>
        /// <param name="xCoordAngle">New pendulum angle in X-coordinate</param>
        /// <param name="yCoordAngle">New pendulum angle in Y-coordinate</param>
        /// <param name="xCoordPosition">New cart position in X-coordinate</param>
        /// <param name="yCoordPosition">New cart position in Y-coordinate</param>
        public void Reset(double xCoordAngle = 0.0, double yCoordAngle = 0.0, double xCoordPosition = 0.0, double yCoordPosition = 0.0)
        {
            StateX = new OneDimensionalSystemState();
            StateX.Angle = xCoordAngle;
            StateX.Position = xCoordPosition;
            StateY = new OneDimensionalSystemState();
            StateY.Angle = yCoordAngle;
            StateY.Position = yCoordPosition;
            ResetTimer();
        }

        /// <summary>
        /// Convert time to array
        /// </summary>
        /// <returns>Array of time stamps</returns>
        public double[] ToTimeArray()
        {
            return new double[] { Time, Time + TimeDelta };
        }

        /// <summary>
        /// Update simulation time
        /// </summary>
        public void UpdateTimer()
        {
            Time += TimeDelta;
        }

        /// <summary>
        /// Reste simuation time
        /// </summary>
        public void ResetTimer()
        {
            Time = 0.0;
        }

        /// <summary>
        /// Update X-coordinate subsystem state
        /// </summary>
        /// <param name="xState">New subsystem state</param>
        public void UpdateSystemStateX(OneDimensionalSystemState xState)
        {
            LastStateX = StateX;
            StateX = xState;
        }

        /// <summary>
        /// Update Y-coordinate subsystem state
        /// </summary>
        /// <param name="xState">New subsystem state</param>
        public void UpdateSystemStateY(OneDimensionalSystemState yState)
        {
            LastStateY = StateY;
            StateY = yState;
        }

        /// <summary>
        /// Get actual cart position
        /// </summary>
        /// <returns>Point with 2-dim position</returns>
        public Point3D GetSystemPosition()
        {
            return new Point3D(StateX.Position, StateY.Position, 0.0);
        }
        #endregion
    }
}
