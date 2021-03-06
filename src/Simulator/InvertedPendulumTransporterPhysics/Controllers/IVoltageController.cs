﻿namespace InvertedPendulumTransporterPhysics.Controllers
{
    /// <summary>
    /// Interface dedicated to voltage controller
    /// </summary>
    public interface IVoltageController : IController
    {
        /// <summary>
        /// Type of voltage controller
        /// </summary>
        ControlType ControlType { get; set; }

        /// <summary>
        /// Get result voltage from regulation process
        /// </summary>
        /// <returns>Motor Voltage</returns>
        double GetVoltage();

        /// <summary>
        /// Set current simulation time (for sinusoidal methods)
        /// </summary>
        /// <param name="time">Current simuation time</param>
        void SetTime(double time);

        /// <summary>
        /// Set control error
        /// </summary>
        /// <param name="angleError">Difference between desired angle and actual one</param>
        /// <param name="positionError">Difference between desired position and actual one</param>
        void SetControlError(double angleError, double positionError);

        /// <summary>
        /// Reset controller with given time delta
        /// </summary>
        /// <param name="timeDelta">Actual time delta</param>
        void Reset(double timeDelta);

        /// <summary>
        /// Setup angle generated by user (for none control)
        /// </summary>
        /// <param name="userAngle">User angle value</param>
        void SetUserAngle(double userAngle);
    }
}
