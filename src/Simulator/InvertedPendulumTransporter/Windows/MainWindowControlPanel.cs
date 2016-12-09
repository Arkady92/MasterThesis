using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace InvertedPendulumTransporter.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Properties

        public double MaxAngle { get { return systemState.MaxAngle; } }
        public double MinAngle { get { return systemState.MinAngle; } }

        public double MaxWindPower { get { return windController.MaxWindPower; } }
        public double MinWindPower { get { return windController.MinWindPower; } }

        public double CartPositionX
        {
            get { return Math.Round(systemState.StateX.Position, 2); }
        }
        public double CartPositionY
        {
            get { return Math.Round(systemState.StateY.Position, 2); }
        }

        public double CartVelocityX
        {
            get { return Math.Round(systemState.StateX.Velocity, 2); }
        }
        public double CartVelocityY
        {
            get { return Math.Round(systemState.StateY.Velocity, 2); }
        }

        public double PendulumAngleX
        {
            get { return Math.Round(systemState.StateX.Angle, 2); }
        }
        public double PendulumAngleY
        {
            get { return Math.Round(systemState.StateY.Angle, 2); }
        }

        public double PendulumAngularVelocityX
        {
            get { return Math.Round(systemState.StateX.AngularVelocity, 2); }
        }
        public double PendulumAngularVelocityY
        {
            get { return Math.Round(systemState.StateY.AngularVelocity, 2); }
        }

        public double TimeDelta
        {
            get { return systemState.TimeDelta; }
            set
            {
                value = Math.Round(value, 3);
                if (value != systemState.TimeDelta)
                {
                    systemState.TimeDelta = value;
                    OnPropertyChanged("TimeDelta");
                }
            }
        }

        private double xCoordAngle;
        public double XCoordAngle
        {
            get { return xCoordAngle; }
            set
            {
                value = Math.Round(value, 2);
                if (value != xCoordAngle)
                {
                    xCoordAngle = value;
                    systemState.StateX.Angle = value;
                    SceneControl.UpdateState(systemState);
                    OnPropertyChanged("XCoordAngle");
                    OnPropertyChanged("PendulumAngleX");
                }
            }
        }

        private double yCoordAngle;
        public double YCoordAngle
        {
            get { return yCoordAngle; }
            set
            {
                value = Math.Round(value, 2);
                if (value != yCoordAngle)
                {
                    yCoordAngle = value;
                    systemState.StateY.Angle = value;
                    SceneControl.UpdateState(systemState);
                    OnPropertyChanged("YCoordAngle");
                    OnPropertyChanged("PendulumAngleY");
                }
            }
        }

        public double RodLength
        {
            get { return Math.Round(systemState.SolverParameters.PendulumLength, 2); }
            set
            {
                value = Math.Round(value, 2);
                if (value != systemState.SolverParameters.PendulumLength)
                {
                    systemState.SolverParameters.PendulumLength = value;
                    SceneControl.UpdateState(systemState);
                    SceneControl.UpdateCamera(systemState);
                    OnPropertyChanged("RodLength");
                }
            }
        }

        public double PendulumMass
        {
            get { return Math.Round(systemState.SolverParameters.PendulumMass, 2); }
            set
            {
                value = Math.Round(value, 2);
                if (value != systemState.SolverParameters.PendulumMass)
                {
                    systemState.SolverParameters.PendulumMass = value;
                    SceneControl.UpdateState(systemState);
                    SceneControl.UpdateCamera(systemState);
                    OnPropertyChanged("PendulumMass");
                }
            }
        }

        public double CartMass
        {
            get { return Math.Round(systemState.SolverParameters.CartMass, 2); }
            set
            {
                value = Math.Round(value, 2);
                if (value != systemState.SolverParameters.CartMass)
                {
                    systemState.SolverParameters.CartMass = value;
                    SceneControl.UpdateState(systemState);
                    SceneControl.UpdateCamera(systemState);
                    OnPropertyChanged("CartMass");
                }
            }
        }

        public double WindPower
        {
            get { return windController.WindPower; }
            set
            {
                value = Math.Round(value, 2);
                if (value != windController.WindPower)
                {
                    windController.WindPower = value;
                    OnPropertyChanged("WindPower");
                }
            }
        }

        public double WindChangeSpeed
        {
            get { return windController.WindChangeSpeed; }
            set
            {
                value = Math.Round(value, 1);
                if (value != windController.WindChangeSpeed)
                {
                    windController.WindChangeSpeed = value;
                    OnPropertyChanged("WindChangeSpeed");
                }
            }
        }
        #endregion

        #region Properties Event Handlers
        /// <summary>
        /// Event handler for raising property change
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Property change handling method
        /// </summary>
        /// <param name="propertyName"></param>
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Functions
        private void UpdateGUI()
        {
            OnPropertyChanged("CartPositionX");
            OnPropertyChanged("CartPositionY");
            OnPropertyChanged("CartVelocityX");
            OnPropertyChanged("CartVelocityY");
            OnPropertyChanged("PendulumAngleX");
            OnPropertyChanged("PendulumAngleY");
            OnPropertyChanged("PendulumAngularVelocityX");
            OnPropertyChanged("PendulumAngularVelocityY");
            OnPropertyChanged("RodLength");
            OnPropertyChanged("CartMass");
            OnPropertyChanged("PendulumMass");
            OnPropertyChanged("TimeDelta");
            OnPropertyChanged("XCoordAngle");
            OnPropertyChanged("YCoordAngle");
        }

        /// <summary>
        /// Start animation
        /// </summary>
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!animationPlaying)
            {
                animationPlaying = true;
                EnableSystemParameters(false);
                SceneControl.ResetSimulation(systemState);
                SetupControllers();
            }
            PlayButton.IsEnabled = false;
            PauseButton.IsEnabled = true;
            gameController.GamePlaying = true;
            dispatcherTimer.Start();
        }

        /// <summary>
        /// Pause animation
        /// </summary>
        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            PlayButton.IsEnabled = true;
            PauseButton.IsEnabled = false;
            gameController.GamePlaying = false;
        }

        /// <summary>
        /// Reset animation
        /// </summary>
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            animationPlaying = false;
            PlayButton.IsEnabled = true;
            PauseButton.IsEnabled = false;
            EnableSystemParameters(true);
            dispatcherTimer.Stop();

            PlotsControl.ResetPlots();

            if (trajectoryController.TrajectoryEnabled)
            {
                var startPosition = trajectoryController.GetTargetStartPosition();
                systemState.Reset(XCoordAngle, YCoordAngle, startPosition.X, startPosition.Y);
            }
            else
                systemState.Reset(XCoordAngle, YCoordAngle);
            SceneControl.ResetSimulation(systemState);
            xCoordVoltageController.Reset(systemState.TimeDelta);
            yCoordVoltageController.Reset(systemState.TimeDelta);
            windController.Reset();
            trajectoryController.Reset();
            gameController.Reset();

            UpdateGUI();
        }

        private void EnableSystemParameters(bool value)
        {
            foreach (var item in SystemParametersPanel.Children.OfType<Slider>())
                item.IsEnabled = value;
            ResetParametersButton.IsEnabled = value;
        }

        private void SimulationScene_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            gameController.HandleKey(e.Key);
        }

        private void ResetParametersButton_Click(object sender, RoutedEventArgs e)
        {
            systemState.ResetSystemParameters();
            TimeDelta = systemState.DefaultTimeDelta;
            XCoordAngle = 0.0;
            YCoordAngle = 0.0;
            RodLength = systemState.SolverParameters.PendulumLength;
            CartMass = systemState.SolverParameters.CartMass;
            PendulumMass = systemState.SolverParameters.PendulumMass;
            UpdateGUI();
        }

        private void UpKeyboardButton_Click(object sender, RoutedEventArgs e)
        {
            gameController.HandleKey(Key.T);
        }

        private void DownKeyboardButton_Click(object sender, RoutedEventArgs e)
        {
            gameController.HandleKey(Key.G);
        }

        private void LeftKeyboardButton_Click(object sender, RoutedEventArgs e)
        {
            gameController.HandleKey(Key.F);
        }

        private void RightKeyboardButton_Click(object sender, RoutedEventArgs e)
        {
            gameController.HandleKey(Key.H);
        }
        #endregion
    }
}
