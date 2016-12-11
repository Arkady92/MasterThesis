using InvertedPendulumTransporterPhysics.Common;
using System.Windows.Media.Media3D;

namespace InvertedPendulumTransporter.Models
{
    /// <summary>
    /// Interface dedicated to all visualization models
    /// </summary>
    public interface IModel
    {
        /// <summary>
        /// Reference to model
        /// </summary>
        ModelVisual3D Model { get; }

        /// <summary>
        /// Initialization method
        /// </summary>
        void Initialize();

        /// <summary>
        /// Uptate model visual
        /// </summary>
        /// <param name="systemState">Actual system state</param>
        void UpdateState(SystemState systemState);

        /// <summary>
        /// Seup high level graphics for model
        /// </summary>
        void SetupHighLevelGraphics();

        /// <summary>
        /// Setup low level graphics for model
        /// </summary>
        void SetupLowLevelGraphics();
    }
}
