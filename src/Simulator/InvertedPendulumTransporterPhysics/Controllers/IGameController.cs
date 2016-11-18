using System.Windows.Input;

namespace InvertedPendulumTransporterPhysics.Controllers
{
    public interface IGameController : IController
    {
        double UserAngleX { get; }
        double UserAngleY { get; }
        bool GameEnabled { get; set; }
        void HandleKey(Key key);
    }
}
