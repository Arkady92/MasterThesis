using System.Windows.Media.Media3D;

namespace InvertedPendulumTransporter.Models
{
    public interface IPendulum : IModel
    {
        Point3D MassLinkPoint { get; }
        Point3D CartLinkPoint { get; }
        double RodLength { get; }

        void SetupHighGradeTextures();
        void SetupLowGradeTextures();
    }
}
