using System;
using System.Collections.Generic;

namespace InvertedPendulumTransporter
{
    public class SystemController
    {
        private Random random;
        public ControlType ControlType { get; set; }
        public double TimeDelta { get; set; }
        private double time;
        private PIDCorrector singlePIDCorrector;
        private PIDCorrector doublePIDCorrector;

        public SystemController()
        {
            random = new Random(Guid.NewGuid().GetHashCode());
            singlePIDCorrector = new PIDCorrector();
            doublePIDCorrector = new PIDCorrector();
        }

        public double GetVoltage()
        {
            switch (ControlType)
            {
                case ControlType.Random:
                    return (random.Next(300) - 150) / 1.0;
                case ControlType.Sinusoidal:
                    return Math.Sin(time * 10) * 2;
                case ControlType.PID:
                    return CalculateSinglePIDCorrection();
                case ControlType.DoublePID:
                    return CalculateDoublePIDCorrection();
                case ControlType.None:
                    return 0.0;
            }
            return 0.0;
        }

        private double CalculateDoublePIDCorrection()
        {
            return doublePIDCorrector.CalculateParallelPositionAnglePIDCorrection();
        }

        private double CalculateSinglePIDCorrection()
        {
            return singlePIDCorrector.CalculateAnglePIDCorrection();
        }

        public void SetTime(double time)
        {
            this.time = time;
        }

        public void Reset(double timeDelta)
        {
            singlePIDCorrector.Reset(timeDelta);
            doublePIDCorrector.Reset(timeDelta);
        }

        public void SetControlError(double angleError, double positionError)
        {
            singlePIDCorrector.SetAngleError(angleError);
            doublePIDCorrector.SetAngleError(angleError);
            doublePIDCorrector.SetPositionError(positionError);
        }
    }

    public enum ControlType
    {
        Random,
        Sinusoidal,
        None,
        PID,
        DoublePID
    }
}
