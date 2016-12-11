using System.Windows.Media.Media3D;

namespace InvertedPendulumTransporterPhysics.Controllers
{
    /// <summary>
    /// Interface dedicated to wind controller
    /// </summary>
    public interface IWindController : IController
    {
        /// <summary>
        /// Wind type
        /// </summary>
        WindType WindType { get; set; }

        /// <summary>
        /// Wind power
        /// </summary>
        double WindPower { get; set; }

        /// <summary>
        /// Wind change speed
        /// </summary>
        double WindChangeSpeed { get; set; }

        /// <summary>
        /// Max wind power
        /// </summary>
        double MaxWindPower { get; }

        /// <summary>
        /// Min wind power
        /// </summary>
        double MinWindPower { get; }

        /// <summary>
        /// Default wind power
        /// </summary>
        double DefaultWindPower { get; }

        /// <summary>
        /// Update wind force
        /// </summary>
        /// <returns>Wind force direction</returns>
        Vector3D UpdateWindForce();

        /// <summary>
        /// Get final wind power in X-coordinate
        /// </summary>
        /// <returns>Wind power</returns>
        double GetXCoordWindPower();

        /// <summary>
        /// Get final wind power in X-coordinate
        /// </summary>
        /// <returns>Wind power</returns>
        double GetYCoordWindPower();

        /// <summary>
        /// Get final wind power in X-coordinate
        /// </summary>
        /// <returns>Wind power</returns>
        double GetZCoordWindPower();
    }
}
