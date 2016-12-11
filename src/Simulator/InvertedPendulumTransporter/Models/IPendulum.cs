using System.Windows.Media.Media3D;

namespace InvertedPendulumTransporter.Models
{
    /// <summary>
    /// Interface dedicated to pendulum model
    /// </summary>
    public interface IPendulum : IModel
    {
        /// <summary>
        /// Three dimensional point of pendulum mass link position
        /// </summary>
        Point3D MassLinkPoint { get; }

        /// <summary>
        /// Three dimensional point of pendulum cart link position
        /// </summary>
        Point3D CartLinkPoint { get; }

        /// <summary>
        /// Pendulum rod length
        /// </summary>
        double RodLength { get; }
    }
}
