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
    /// Interaction logic for MainWindow.xaml [Control Panel management]
    /// </summary>
    public partial class MainWindow
    {
        #region Private Members
        private double xCoordAngle;
        private double yCoordAngle;
        #endregion

        #region Public Members
        /// <summary>
        /// Max pendulum angle
        /// </summary>
        public double MaxAngle { get { return systemState.MaxAngle; } }

        /// <summary>
        /// Min pendulum angle
        /// </summary>
        public double MinAngle { get { return systemState.MinAngle; } }

        /// <summary>
        /// Max wind power
        /// </summary>
        public double MaxWindPower { get { return windController.MaxWindPower; } }

        /// <summary>
        /// Min wind power
        /// </summary>
        public double MinWindPower { get { return windController.MinWindPower; } }

        /// <summary>
        /// Actual cart position in X-coordinate
        /// </summary>
        public double CartPositionX
        {
            get { return Math.Round(systemState.StateX.Position, 2); }
        }

        /// <summary>
        /// Actual cart position in Y-coordinate
        /// </summary>
        public double CartPositionY
        {
            get { return Math.Round(systemState.StateY.Position, 2); }
        }

        /// <summary>
        /// Actual cart velocity in X-coordinate
        /// </summary>
        public double CartVelocityX
        {
            get { return Math.Round(systemState.StateX.Velocity, 2); }
        }

        /// <summary>
        /// Actual cart velocity in Y-coordinate
        /// </summary>
        public double CartVelocityY
        {
            get { return Math.Round(systemState.StateY.Velocity, 2); }
        }

        /// <summary>
        /// Actual pendulum angle in X-coordinate
        /// </summary>
        public double PendulumAngleX
        {
            get { return Math.Round(systemState.StateX.Angle, 2); }
        }

        /// <summary>
        /// Actual pendulum angle in Y-coordinate
        /// </summary>
        public double PendulumAngleY
        {
            get { return Math.Round(systemState.StateY.Angle, 2); }
        }

        /// <summary>
        /// Actual pendulum angual velocity in X-coordinate
        /// </summary>
        public double PendulumAngularVelocityX
        {
            get { return Math.Round(systemState.StateX.AngularVelocity, 2); }
        }

        /// <summary>
        /// Actual pendulum angual velocity in Y-coordinate
        /// </summary>
        public double PendulumAngularVelocityY
        {
            get { return Math.Round(systemState.StateY.AngularVelocity, 2); }
        }

        /// <summary>
        /// Actual time distance between two frames
        /// </summary>
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

        /// <summary>
        /// Initial pendulum angle in X-coordinate
        /// </summary>
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

        /// <summary>
        /// Initial pendulum angle in X-coordinate
        /// </summary>
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

        /// <summary>
        /// Actual pendulum rod length
        /// </summary>
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

        /// <summary>
        /// Actual pendulum mass
        /// </summary>
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

        /// <summary>
        /// Actual cart mass
        /// </summary>
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

        /// <summary>
        /// Actual  wind power
        /// </summary>
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

        /// <summary>
        /// Actual wind change speed
        /// </summary>
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

        /// <summary>
        /// Event handler for raising property change
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Private Methods

        /// <summary>
        /// Update GUI elements
        /// </summary>
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
        /// Handle play animation button click
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="e">Event arguments</param>
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
        /// Handle reset animation button click
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="e">Event arguments</param>
        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            PlayButton.IsEnabled = true;
            PauseButton.IsEnabled = false;
            gameController.GamePlaying = false;
        }

        /// <summary>
        /// Handle reset animation button click
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="e">Event arguments</param>
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
            foreach (var controller in controllers)
                controller.Reset();
            xCoordVoltageController.Reset(systemState.TimeDelta);
            yCoordVoltageController.Reset(systemState.TimeDelta);

            UpdateGUI();
        }

        /// <summary>
        /// Change system parameters modification enablement
        /// </summary>
        /// <param name="value">Are parameters enabled</param>
        private void EnableSystemParameters(bool value)
        {
            foreach (var item in SystemParametersPanel.Children.OfType<Slider>())
                item.IsEnabled = value;
            ResetParametersButton.IsEnabled = value;
        }

        /// <summary>
        /// Handle keyboard input over simulation area
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event args</param>
        private void SimulationScene_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            gameController.HandleKey(e.Key);
        }

        /// <summary>
        /// Handle reset parameters button click
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
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

        /// <summary>
        /// Handle virtual keyboard up button click
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void UpKeyboardButton_Click(object sender, RoutedEventArgs e)
        {
            gameController.HandleKey(Key.T);
        }

        /// <summary>
        /// Handle virtual keyboard down button click
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void DownKeyboardButton_Click(object sender, RoutedEventArgs e)
        {
            gameController.HandleKey(Key.G);
        }

        /// <summary>
        /// Handle virtual keyboard left button click
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void LeftKeyboardButton_Click(object sender, RoutedEventArgs e)
        {
            gameController.HandleKey(Key.F);
        }

        /// <summary>
        /// Handle virtual keyboard right button click
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void RightKeyboardButton_Click(object sender, RoutedEventArgs e)
        {
            gameController.HandleKey(Key.H);
        }

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

        #region Public Methods
        #endregion

        
    }
}
