using System.Windows.Media.Media3D;

namespace InvertedPendulumTransporterPhysics.Controllers
{
    public interface IWindController : IController
    {
        WindType WindType { get; set; }
        double WindPower { get; set; }
        double WindChangeSpeed { get; set; }
        double MaxWindPower { get; }
        double MinWindPower { get; }
        double DefaultWindPower { get; }

        Vector3D UpdateWindForce();
        double GetXCoordWindPower();
        double GetYCoordWindPower();
        double GetZCoordWindPower();
    }
}
