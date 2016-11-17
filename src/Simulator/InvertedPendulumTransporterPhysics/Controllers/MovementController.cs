using System;
using System.Windows.Input;

namespace InvertedPendulumTransporterPhysics.Controllers
{
    public class MovementController
    {
        private const double WindControlPowerDelta = Math.PI / 120;
        public double CorrectAngleX;
        public double CorrectAngleY;

        public MovementController()
        {

        }

        public void Reset()
        {
            CorrectAngleY = 0.0;
            CorrectAngleX = 0.0;
        }

        public void HandleKey(Key key)
        {
            switch (key)
            {
                case Key.T:
                    CorrectAngleY += WindControlPowerDelta;
                    break;
                case Key.G:
                    CorrectAngleY -= WindControlPowerDelta;
                    break;
                case Key.F:
                    CorrectAngleX -= WindControlPowerDelta;
                    break;
                case Key.H:
                    CorrectAngleX += WindControlPowerDelta;
                    break;
            }
        }
    }
}
