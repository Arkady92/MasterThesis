using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

namespace InvertedPendulumTransporterPhysics.Controllers
{
    public class GameController : IGameController
    {
        private const double WindControlPowerDelta = Math.PI / 120;
        public double UserAngleX { get; private set; }
        public double UserAngleY { get; private set; }

        public bool GameEnabled { get; set; }
        public bool GamePlaying { get; set; }
        private Dictionary<Key, Button> keyButtons;

        public GameController(Button up, Button down, Button left, Button right)
        {
            keyButtons = new Dictionary<Key, Button>()
            {
                { Key.T, up },
                { Key.G, down },
                { Key.F, left },
                { Key.H, right }
            };
        }

        public void Reset()
        {
            UserAngleY = 0.0;
            UserAngleX = 0.0;
            GamePlaying = false;
        }

        public void HandleKey(Key key)
        {
            if (!GameEnabled || !GamePlaying) return;
            switch (key)
            {
                case Key.T:
                    UserAngleY += WindControlPowerDelta;
                    keyButtons[key].Focus();
                    break;
                case Key.G:
                    UserAngleY -= WindControlPowerDelta;
                    keyButtons[key].Focus();
                    break;
                case Key.F:
                    UserAngleX -= WindControlPowerDelta;
                    keyButtons[key].Focus();
                    break;
                case Key.H:
                    UserAngleX += WindControlPowerDelta;
                    keyButtons[key].Focus();
                    break;
            }
        }
    }
}
