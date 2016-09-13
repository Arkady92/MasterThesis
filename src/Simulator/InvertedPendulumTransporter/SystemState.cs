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

        public SystemState()
        {
            InitializeDefault();
        }

        public void InitializeDefault()
        {
            StateX = new OneDimensionalSystemState();
            StateY = new OneDimensionalSystemState();
            TimeDelta = 0.01;
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
