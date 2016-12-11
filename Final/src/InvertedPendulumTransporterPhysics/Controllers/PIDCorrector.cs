using System;

namespace InvertedPendulumTransporterPhysics.Controllers
{
    /// <summary>
    /// Helping class for voltage controlling dedicated to PID corrector
    /// </summary>
    public class PIDCorrector
    {
        #region Private Members
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
        private const double angleControlReductionFactor = 0.5;
        private double TimeDelta;
        private bool firstAngleIteration;
        private bool firstPositionIteration;
        private bool firstDoublePIDIteration;
        #endregion

        #region Public Members

        #endregion

        #region Private Methods

        #endregion

        #region Public Methods
        /// <summary>
        /// Class constructor
        /// </summary>
        public PIDCorrector()
        {
            firstAngleIteration = true;
            firstPositionIteration = true;
            firstDoublePIDIteration = true;
        }

        /// <summary>
        /// Setup pendulum angle error
        /// </summary>
        /// <param name="error">Angle error</param>
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

        /// <summary>
        /// Setup cart position error
        /// </summary>
        /// <param name="error">Position error</param>
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

        /// <summary>
        /// Reset object to default state
        /// </summary>
        public void Reset()
        {
            previousAngleError = 0.0;
            angleError = 0.0;
            sumAngleErrors = 0.0;
            previousPositionError = 0.0;
            positionError = 0.0;
            firstAngleIteration = true;
            firstPositionIteration = true;
            firstDoublePIDIteration = true;
        }

        /// <summary>
        /// Reset object to default state with setting new time delta
        /// </summary>
        /// <param name="timeDelta">Time delta</param>
        public void Reset(double timeDelta)
        {
            TimeDelta = timeDelta;
            Reset();
        }

        /// <summary>
        /// Calculate PID correction for pendulum angle
        /// </summary>
        /// <returns>Motor voltage</returns>
        public double CalculateAnglePIDCorrection()
        {
            var result = KpA * (angleError + sumAngleErrors / TiA + TdA * (angleError - previousAngleError) / TimeDelta);
            return (Math.Abs(result) > MaxVoltage) ? Math.Sign(result) * MaxVoltage : result;
        }

        /// <summary>
        /// Calculate PID correction for cart position
        /// </summary>
        /// <param name="angleError">Pendulum angle error</param>
        /// <returns>Motor voltage</returns>
        public double CalculatePositionPIDCorrection(double angleError)
        {
            var result = KpPC * (positionError + TdPC * (positionError - previousPositionError) / TimeDelta);
            return result;
        }

        /// <summary>
        /// Calculate PID correction parallely for cart position and pendulum angle
        /// </summary>
        /// <param name="integral">Use integral part in pendulum angle correction</param>
        /// <returns></returns>
        public double CalculateParallelPositionAnglePIDCorrection(bool integral)
        {
            var positionPIDCorrection = KpPP * (positionError + TdPP * (positionError - previousPositionError) / TimeDelta);
            double anglePIDCorrection = 0.0;
            if (integral)
                anglePIDCorrection = KpA * (angleError + sumAngleErrors / TiA + TdA * (angleError - previousAngleError) / TimeDelta);
            else
                anglePIDCorrection = KpA * (angleError + TdA * (angleError - previousAngleError) / TimeDelta) * angleControlReductionFactor;
            var result = (anglePIDCorrection - positionPIDCorrection);
            if (firstDoublePIDIteration)
            {
                firstDoublePIDIteration = false;
                return 0;
            }
            return (Math.Abs(result) > MaxVoltage) ? Math.Sign(result) * MaxVoltage : result;
        }
        #endregion
    }
}
