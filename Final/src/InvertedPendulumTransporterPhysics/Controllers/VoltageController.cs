using System;

namespace InvertedPendulumTransporterPhysics.Controllers
{
    /// <summary>
    /// Controller for motor voltage
    /// </summary>
    public class VoltageController : IVoltageController
    {
        #region Private Members
        private Random random;
        private double time;
        private PIDCorrector singlePIDCorrector;
        private PIDCorrector doublePIDCorrector;
        private const int SwitchStep = 10;
        private int counter = 0;
        private double randomControlValue;
        private double userAngle;
        #endregion

        #region Public Members
        #region IVoltageController Interface
        public ControlType ControlType { get; set; }
        public const ControlType DefaultControlType = ControlType.PID;
        #endregion
        #endregion

        #region Private Methods
        /// <summary>
        /// Calculate correction by modified double parallel controller 
        /// </summary>
        /// <returns>Motor voltage</returns>
        private double CalculateDoublePDParallelCorrection()
        {
            return doublePIDCorrector.CalculateParallelPositionAnglePIDCorrection(false);
        }

        /// <summary>
        /// Calculate correction by double parallel controller 
        /// </summary>
        /// <returns>Motor voltage</returns>
        private double CalculateDoublePIDParallelCorrection()
        {
            return doublePIDCorrector.CalculateParallelPositionAnglePIDCorrection(true);
        }

        /// <summary>
        /// Calculate correction by double cascade controller 
        /// </summary>
        /// <returns>Motor voltage</returns>
        private double CalculateDoublePIDCascadeCorrection()
        {
            return doublePIDCorrector.CalculateAnglePIDCorrection();
        }

        /// <summary>
        /// Calculate correction simple PID controller 
        /// </summary>
        /// <returns>Motor voltage</returns>
        private double CalculateSinglePIDCorrection()
        {
            return singlePIDCorrector.CalculateAnglePIDCorrection();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Class constructor
        /// </summary>
        public VoltageController()
        {
            random = new Random(Guid.NewGuid().GetHashCode());
            singlePIDCorrector = new PIDCorrector();
            doublePIDCorrector = new PIDCorrector();
            ControlType = DefaultControlType;
        }

        #region IVoltageController Interface
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
                    return userAngle;
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

        public void SetUserAngle(double userAngle)
        {
            this.userAngle = userAngle;
        }
        #endregion
        #endregion
    }

    /// <summary>
    /// Enumeration for control types
    /// </summary>
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
