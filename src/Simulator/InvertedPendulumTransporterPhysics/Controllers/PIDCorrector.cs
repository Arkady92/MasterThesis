using System;

namespace InvertedPendulumTransporterPhysics.Controllers
{
    public class PIDCorrector
    {
        private double angleError;
        private double previousAngleError;
        private double sumAngleErrors;
        private double positionError;
        private double previousPositionError;
        private const double MaxVoltage = 230;
        private const double KpA = -50.8;
        private const double TiA = 7.26;
        private const double TdA = 0.24;
        private const double KpPC = 0.05;
        private const double TdPC = 0.7;
        private const double KpPP = 6.0; 
        private const double TdPP = 1.5;
        private const double angleControlReductionFactor = 1;
        private double TimeDelta;
        private bool firstAngleIteration;
        private bool firstPositionIteration;

        public PIDCorrector()
        {
            firstAngleIteration = true;
            firstPositionIteration = true;
        }

        public void SetAngleError(double error)
        {
            if (firstAngleIteration)
            {
                previousAngleError = error;
                firstAngleIteration = false;
            }
            else
                previousAngleError = angleError;
            angleError = error;
            sumAngleErrors += error;
        }

        public void SetPositionError(double error)
        {
            if (firstPositionIteration)
            {
                previousPositionError = error;
                firstPositionIteration = false;
            }
            else
                previousPositionError = positionError;
            positionError = error;
        }

        public void Reset()
        {
            previousAngleError = 0.0;
            angleError = 0.0;
            sumAngleErrors = 0.0;
            previousPositionError = 0.0;
            positionError = 0.0;
            firstAngleIteration = true;
            firstPositionIteration = true;
        }

        public void Reset(double timeDelta)
        {
            TimeDelta = timeDelta;
            Reset();
        }

        public double CalculateAnglePIDCorrection()
        {
            var result = KpA * (angleError + sumAngleErrors / TiA + TdA * (angleError - previousAngleError) / TimeDelta);
            return (Math.Abs(result) > MaxVoltage) ? Math.Sign(result) * MaxVoltage : result;
        }

        public double CalculatePositionPIDCorrection(double angleError)
        {
            var result = KpPC * (positionError + TdPC * (positionError - previousPositionError) / TimeDelta);
            return result;
        }

        public double CalculateParallelPositionAnglePIDCorrection(bool integral)
        {
            var positionPIDCorrection = KpPP * (positionError + TdPP * (positionError - previousPositionError) / TimeDelta);
            double anglePIDCorrection = 0.0;
            if(integral)
                anglePIDCorrection = KpA * (angleError + sumAngleErrors / TiA + TdA * (angleError - previousAngleError) / TimeDelta);
            else
                anglePIDCorrection = KpA * (angleError + TdA * (angleError - previousAngleError) / TimeDelta) * angleControlReductionFactor;
            var result = (anglePIDCorrection - positionPIDCorrection);
            return (Math.Abs(result) > MaxVoltage) ? Math.Sign(result) * MaxVoltage : result;
        }
    }
}
