using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

namespace InvertedPendulumTransporterPhysics.Controllers
{
    /// <summary>
    /// Controller for game mode
    /// </summary>
    public class GameController : IGameController
    {
        #region Private Members
        private Dictionary<Key, Button> keyButtons;
        private const double UserControlDelta = Math.PI / 120;
        #endregion

        #region Public Members
        #region IGameController Interface
        public double UserAngleX { get; private set; }
        public double UserAngleY { get; private set; }
        public bool GameEnabled { get; set; }
        public bool GamePlaying { get; set; }
        #endregion
        #endregion

        #region Private Methods

        #endregion

        #region Public Methods
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="up">Up key binding</param>
        /// <param name="down">Down key binding</param>
        /// <param name="left">Left key binding</param>
        /// <param name="right">Right key binding</param>
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

        #region IGameController Interface
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
                    UserAngleY += UserControlDelta;
                    keyButtons[key].Focus();
                    break;
                case Key.G:
                    UserAngleY -= UserControlDelta;
                    keyButtons[key].Focus();
                    break;
                case Key.F:
                    UserAngleX -= UserControlDelta;
                    keyButtons[key].Focus();
                    break;
                case Key.H:
                    UserAngleX += UserControlDelta;
                    keyButtons[key].Focus();
                    break;
            }
        }
        #endregion
        #endregion
    }
}
