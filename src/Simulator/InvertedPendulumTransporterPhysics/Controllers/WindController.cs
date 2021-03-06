﻿using System;
using System.Windows.Media.Media3D;

namespace InvertedPendulumTransporterPhysics.Controllers
{
    /// <summary>
    /// Controller for wind power
    /// </summary>
    public class WindController : IWindController
    {
        #region Private Members
        private Random random;
        private Vector3D startWindDirection;
        private Vector3D endWindDirection;
        private Vector3D windDirection;
        private int tickCounter = 0;
        private double actualWindChangeSpeed;
        private double omega;
        private const int peakResetValue = 500;
        private const int switchResetValue = 500;
        private const int smoothResetValue = 500;
        #endregion

        #region Public Members
        #region IWindController Interface
        public WindType WindType { get; set; }
        public double WindPower { get; set; }
        public double WindChangeSpeed { get; set; }
        public double MaxWindPower { get { return 1.0; } }
        public double MinWindPower { get { return 0.0; } }
        public double DefaultWindPower { get { return 0.0; } }
        #endregion

        /// <summary>
        /// Default wind change speed
        /// </summary>
        public const double DefaultWindChangeSpeed = 0.5;

        /// <summary>
        /// Default wind type
        /// </summary>
        public const WindType DefaultWindType = WindType.RandomSmooth;
        #endregion

        #region Private Methods
        /// <summary>
        /// Generate random peak wind direction
        /// </summary>
        /// <returns>Wind direction</returns>
        private Vector3D GeneratePeakWindDirection()
        {
            var result = new Vector3D(random.NextDouble() * 2 - 1, random.NextDouble() * 2 - 1, random.NextDouble() - 0.5);
            if (result.Length < double.Epsilon)
                return GeneratePeakWindDirection();
            return Normalize(result);
        }

        /// <summary>
        /// Generate random switch wind direction
        /// </summary>
        /// <returns>Wind direction</returns>
        private Vector3D GenerateSwitchWindDirection()
        {
            var result = new Vector3D(-Math.Sign(windDirection.X) * random.NextDouble(),
                -Math.Sign(windDirection.X) * random.NextDouble(), -Math.Sign(windDirection.X) * random.NextDouble() * 0.5);
            if (result.Length < double.Epsilon)
                return GenerateSwitchWindDirection();
            return Normalize(result);
        }

        /// <summary>
        /// Normalize wind direction vector
        /// </summary>
        /// <param name="vector">Input vector</param>
        /// <returns>Normalized vector</returns>
        private Vector3D Normalize(Vector3D vector)
        {
            return vector * (1 / Math.Sqrt(vector.LengthSquared));
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Class constructor
        /// </summary>
        public WindController()
        {
            random = new Random(Guid.NewGuid().GetHashCode());
            WindType = DefaultWindType;
            WindPower = DefaultWindPower;
            WindChangeSpeed = DefaultWindChangeSpeed;
            actualWindChangeSpeed = DefaultWindChangeSpeed;
            windDirection = GeneratePeakWindDirection();
            startWindDirection = windDirection;
            endWindDirection = GenerateSwitchWindDirection();
            omega = Math.Acos(Vector3D.DotProduct(startWindDirection, endWindDirection));
        }

        #region IWindController Interface
        public double GetXCoordWindPower()
        {
            return windDirection.X * WindPower;
        }

        public double GetYCoordWindPower()
        {
            return windDirection.Y * WindPower;
        }

        public double GetZCoordWindPower()
        {
            return windDirection.Z * WindPower;
        }

        public Vector3D UpdateWindForce()
        {
            switch (WindType)
            {
                case WindType.RandomPeak:
                    tickCounter++;
                    if (tickCounter >= peakResetValue * (1.1 - WindChangeSpeed))
                    {
                        tickCounter = 0;
                        windDirection = GeneratePeakWindDirection();
                    }
                    break;
                case WindType.RandomSwitch:
                    tickCounter++;
                    if (tickCounter >= switchResetValue * (1.1 - WindChangeSpeed))
                    {
                        tickCounter = 0;
                        windDirection = GenerateSwitchWindDirection();
                    }
                    break;
                case WindType.RandomSmooth:
                    tickCounter++;
                    if (Math.Abs(WindChangeSpeed) < double.Epsilon)
                    {
                        tickCounter = 0;
                        return windDirection;
                    }
                    if (Math.Abs(actualWindChangeSpeed - WindChangeSpeed) > double.Epsilon)
                    {
                        tickCounter = (int)(tickCounter * (1.1 - WindChangeSpeed) / (1.1 - actualWindChangeSpeed));
                        actualWindChangeSpeed = WindChangeSpeed;
                    }
                    double resetValue = (smoothResetValue * (1.1 - actualWindChangeSpeed));
                    if (tickCounter >= resetValue)
                    {
                        tickCounter = 0;
                        omega = Math.Acos(Vector3D.DotProduct(startWindDirection, endWindDirection));
                        startWindDirection = endWindDirection;
                        endWindDirection = GenerateSwitchWindDirection();
                    }
                    // Calculate wind direction using SLERP
                    windDirection = startWindDirection * Math.Sin(((resetValue - tickCounter) / resetValue) * omega) / Math.Sin(omega)
                        + endWindDirection * Math.Sin(((tickCounter) / resetValue) * omega) / Math.Sin(omega);
                    windDirection = Normalize(windDirection);
                    break;
                default:
                    break;
            }
            return windDirection;
        }

        public void Reset()
        {
            tickCounter = 0;
            windDirection = GeneratePeakWindDirection();
            startWindDirection = windDirection;
            endWindDirection = GenerateSwitchWindDirection();
            omega = Math.Acos(Vector3D.DotProduct(startWindDirection, endWindDirection));
        }
        #endregion
        #endregion
    }

    /// <summary>
    /// Enumeration for wind generation types
    /// </summary>
    public enum WindType
    {
        RandomPeak,
        RandomSwitch,
        RandomSmooth
    }
}
