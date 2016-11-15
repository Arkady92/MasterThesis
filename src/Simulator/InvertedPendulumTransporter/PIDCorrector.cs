using System;

namespace InvertedPendulumTransporter
{
    public class PIDCorrector
    {
        private double angleError;
        private double previousAngleError;
        private double sumAngleErrors;
        private double positionError;
        private double previousPositionError;
        private double sumPositionErrors;
        private const double MaxVoltage = 230;
        private const double KpA = -50.8;
        private const double TiA = 7.26;
        private const double TdA = 0.24;
        private const double KpP = 6;// 0.05;
        private const double TiP = double.MaxValue;
        private const double TdP = 1.5; // 0.7;
        private double TimeDelta;

        public PIDCorrector()
        {
        }

        public void SetAngleError(double error)
        {
            previousAngleError = angleError;
            angleError = error;
            sumAngleErrors += error;
        }

        public void SetPositionError(double error)
        {
            previousPositionError = positionError;
            positionError = error;
            sumPositionErrors += error;
        }

        public void Reset(double timeDelta)
        {
            TimeDelta = timeDelta;
            previousAngleError = 0.0;
            angleError = 0.0;
            sumAngleErrors = 0.0;
        }

        public double CalculateAnglePIDCorrection()
        {
            var result = KpA * (angleError + sumAngleErrors / TiA + TdA * (angleError - previousAngleError) / TimeDelta);
            return (Math.Abs(result) > MaxVoltage) ? Math.Sign(result) * MaxVoltage : result;
        }

        public double CalculateParallelPositionAnglePIDCorrection()
        {
            var A = KpA * (angleError + sumAngleErrors / TiA + TdA * (angleError - previousAngleError) / TimeDelta);
            var B = KpP * (positionError + sumPositionErrors / TiP + TdP * (positionError - previousPositionError) / TimeDelta);

            //if (Math.Abs(A) < double.Epsilon)
            //{
            //    if (Math.Abs(B) < double.Epsilon)
            //        return 0.0;
            //    else
            //        return 0.0;    
            //    //A = B;
            //}
            //return (A * B) / (A + B);
            var result = A - B;
            return (Math.Abs(result) > MaxVoltage) ? Math.Sign(result) * MaxVoltage : result;
        }
    }
}
