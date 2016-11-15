using System;

namespace InvertedPendulumTransporter
{
    public class SystemState
    {
        public OneDimensionalSystemState StateX { get; private set; }
        public OneDimensionalSystemState StateY { get; private set; }
        public double Time { get; set; }
        public OneDimensionalSystemState LastStateX { get; private set; }
        public OneDimensionalSystemState LastStateY { get; private set; }

        public double TimeDelta { get; set; }
        public double DefaultTimeDelta = 0.01;

        public SystemState()
        {
            Reset(0.0,0.0);
        }

        public void Reset(double horizontalAngle, double verticalAngle)
        {
            StateX = new OneDimensionalSystemState();
            StateX.Angle = horizontalAngle;
            StateY = new OneDimensionalSystemState();
            StateY.Angle = verticalAngle;
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
    }
}
