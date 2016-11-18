using System;
using System.Windows.Input;

namespace InvertedPendulumTransporterPhysics.Controllers
{
    public class GameController : IGameController
    {
        private const double WindControlPowerDelta = Math.PI / 120;
        public double UserAngleX { get; private set; }
        public double UserAngleY { get; private set; }

        public bool GameEnabled { get; set; }

        public GameController()
        {

        }

        public void Reset()
        {
            UserAngleY = 0.0;
            UserAngleX = 0.0;
        }

        public void HandleKey(Key key)
        {
            if (!GameEnabled) return;
            switch (key)
            {
                case Key.T:
                    UserAngleY += WindControlPowerDelta;
                    break;
                case Key.G:
                    UserAngleY -= WindControlPowerDelta;
                    break;
                case Key.F:
                    UserAngleX -= WindControlPowerDelta;
                    break;
                case Key.H:
                    UserAngleX += WindControlPowerDelta;
                    break;
            }
        }
    }
}
