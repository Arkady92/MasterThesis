using System;

namespace InvertedPendulumTransporterPhysics.Controllers
{
    public class VoltageController : IVoltageController
    {
        public ControlType ControlType { get; set; }
        public const ControlType DefaultControlType = ControlType.PID;

        private Random random;
        private double time;
        private PIDCorrector singlePIDCorrector;
        private PIDCorrector doublePIDCorrector;
        private const int SwitchStep = 10;
        private int counter = 0;
        private double randomControlValue;

        public VoltageController()
        {
            random = new Random(Guid.NewGuid().GetHashCode());
            singlePIDCorrector = new PIDCorrector();
            doublePIDCorrector = new PIDCorrector();
            ControlType = DefaultControlType;
        }

        public double GetVoltage()
        {
            switch (ControlType)
            {
                case ControlType.Random:
                    counter--;
                    if (counter <= 0)
                    {
                        counter = SwitchStep;
                        randomControlValue = (random.Next(300) - 150) / 5.0;
                    }
                    return randomControlValue;
                case ControlType.Sinusoidal:
                    return Math.Sin(time * 20) * 10;
                case ControlType.PID:
                    return CalculateSinglePIDCorrection();
                case ControlType.DoublePIDCascade:
                    return CalculateDoublePIDCascadeCorrection();
                case ControlType.DoublePIDParallel:
                    return CalculateDoublePIDParallelCorrection();
                case ControlType.DoublePDParallel:
                    return CalculateDoublePDParallelCorrection();
                case ControlType.None:
                    return 0.0;
            }
            return 0.0;
        }

        public void SetTime(double time)
        {
            this.time = time;
        }

        public void Reset()
        {
            singlePIDCorrector.Reset();
            doublePIDCorrector.Reset();
            counter = 0;
            randomControlValue = 0;
        }

        public void Reset(double timeDelta)
        {
            singlePIDCorrector.Reset(timeDelta);
            doublePIDCorrector.Reset(timeDelta);
            counter = 0;
            randomControlValue = 0;
        }

        public void SetControlError(double angleError, double positionError)
        {
            singlePIDCorrector.SetAngleError(angleError);
            doublePIDCorrector.SetPositionError(positionError);
            if (ControlType == ControlType.DoublePIDCascade)
                doublePIDCorrector.SetAngleError(angleError + doublePIDCorrector.CalculatePositionPIDCorrection(angleError));
            else
                doublePIDCorrector.SetAngleError(angleError);
        }

        private double CalculateDoublePDParallelCorrection()
        {
            return doublePIDCorrector.CalculateParallelPositionAnglePIDCorrection(false);
        }

        private double CalculateDoublePIDParallelCorrection()
        {
            return doublePIDCorrector.CalculateParallelPositionAnglePIDCorrection(true);
        }

        private double CalculateDoublePIDCascadeCorrection()
        {
            return doublePIDCorrector.CalculateAnglePIDCorrection();
        }

        private double CalculateSinglePIDCorrection()
        {
            return singlePIDCorrector.CalculateAnglePIDCorrection();
        }
    }

    public enum ControlType
    {
        Random,
        Sinusoidal,
        None,
        PID,
        DoublePIDCascade,
        DoublePIDParallel,
        DoublePDParallel
    }
}
