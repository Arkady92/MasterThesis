using System;
using System.Collections.Generic;

namespace InvertedPendulumTransporter
{
    public class SystemController
    {
        private Random random;
        public ControlType controlType { get; set; }
        public double TimeDelta { get; set; }
        private double time;
        private double controlError;
        private double previousControlError;
        private double sumControlErrors;
        private const double Kp = -50.8;
        private const double Ti = 7.26;
        private const double Td = 0.24;
        private const double MaxVoltage = 230;

        public SystemController()
        {
            random = new Random(Guid.NewGuid().GetHashCode());
        }

        public void SetControlError(double error)
        {
            previousControlError = controlError;
            controlError = error;
            sumControlErrors += error;
        }

        public double GetVoltage()
        {
            switch (controlType)
            {
                case ControlType.Random:
                    return (random.Next(300) - 150) / 1.0;
                case ControlType.Sinusoidal:
                    return Math.Sin(time * 10) * 2;
                case ControlType.PID:
                    return CalculatePIDCorrection();
                case ControlType.None:
                    return 0.0;
            }
            return 0.0;
        }

        private double CalculatePIDCorrection()
        {
            var result = Kp * ( controlError + sumControlErrors / Ti + Td * (controlError - previousControlError)/ TimeDelta);
            return (Math.Abs(result) > MaxVoltage) ? Math.Sign(result) * MaxVoltage : result;
        }

        internal void SetTime(double time)
        {
            this.time = time;
        }

        internal void InitializeDefault()
        {
            previousControlError = 0.0;
            controlError = 0.0;
            sumControlErrors = 0.0;
        }

        internal void SetControlError(object p)
        {
            throw new NotImplementedException();
        }
    }

    public enum ControlType
    {
        Random,
        Sinusoidal,
        None,
        PID
    }
}
