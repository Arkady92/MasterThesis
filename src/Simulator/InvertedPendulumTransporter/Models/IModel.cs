using InvertedPendulumTransporterPhysics.Common;
using System.Windows.Media.Media3D;

namespace InvertedPendulumTransporter.Models
{
    public interface IModel
    {
        ModelVisual3D Model { get; }
        void UpdateState(SystemState systemState);
    }
}
