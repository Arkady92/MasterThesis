using System;
using System.Windows.Media.Media3D;

namespace InvertedPendulumTransporterPhysics.Common
{
    public class SystemState
    {
        public OneDimensionalSystemState StateX { get; private set; }
        public OneDimensionalSystemState StateY { get; private set; }
        public OneDimensionalSystemState LastStateX { get; private set; }
        public OneDimensionalSystemState LastStateY { get; private set; }

        public double MaxAngle { get { return Math.PI / 2.0 * 0.75; } }
        public double MinAngle { get { return -Math.PI / 2.0 * 0.75; } }

        public double DefaultTimeDelta = 0.01;
        public double Time { get; set; }
        public double TimeDelta { get; set; }
        public SolverParameters SolverParameters { get; set; }

        public SystemState()
        {
            Reset();
            SolverParameters = new SolverParameters();
        }

        public void ResetSystemParameters()
        {
            SolverParameters = new SolverParameters();
        }

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

        public double[] ToTimeArray()
        {
            return new double[] { Time, Time + TimeDelta };
        }

        public void UpdateTimer()
        {
            Time += TimeDelta;
        }

        public void ResetTimer()
        {
            Time = 0.0;
        }

        public void UpdateSystemStateX(OneDimensionalSystemState xState)
        {
            LastStateX = StateX;
            StateX = xState;
        }

        public void UpdateSystemStateY(OneDimensionalSystemState yState)
        {
            LastStateY = StateY;
            StateY = yState;
        }

        public Point3D GetSystemPosition()
        {
            return new Point3D(StateX.Position, StateY.Position, 0.0);
        }
    }
}
