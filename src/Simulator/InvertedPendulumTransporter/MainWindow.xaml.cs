using System;
using System.ComponentModel;
using System.Windows;
using InvertedPendulumTransporterPhysics.Common;
using InvertedPendulumTransporterPhysics.Controllers;
using InvertedPendulumTransporterPhysics.Solvers;
using InvertedPendulumTransporter.Controls;
using System.Windows.Controls;

namespace InvertedPendulumTransporter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private System.Windows.Threading.DispatcherTimer dispatcherTimer;
        private ODESolver solver;
        private VoltageController xCoordVoltageController;
        private VoltageController yCoordVoltageController;
        private WindController windController;
        private TrajectoryController trajectoryController;
        private MovementController movementController;
        private SystemState systemState;


        public double MaxAngle { get { return systemState.MaxAngle; } }
        public double MinAngle { get { return systemState.MinAngle; } }

        public double MaxWindPower { get { return windController.MaxWindPower; } }
        public double MinWindPower { get { return windController.MinWindPower; } }


        private double windPower;
        private bool animationPlaying;

        /// <summary>
        /// Animation speed ratio [based on inverse exponential function (1 / 2^x)]
        /// </summary>
        public double AnimationSpeed
        {
            get { return systemState.TimeDelta; }
            set
            {
                value = Math.Round(value, 3);
                if (value != systemState.TimeDelta)
                {
                    systemState.TimeDelta = value;
                    OnPropertyChanged("AnimationSpeed");
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
                    OnPropertyChanged("PendulumSwingX");
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
                    OnPropertyChanged("PendulumSwingY");
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

        public double WindPower
        {
            get { return windPower; }
            set
            {
                value = Math.Round(value, 1);
                if (value != windPower)
                {
                    windPower = value;
                    windController.WindPower = value;
                    OnPropertyChanged("WindPower");
                }
            }
        }

        public double CartPositionX
        {
            get { return Math.Round(systemState.StateX.Position, 2); }
        }
        public double CartPositionY
        {
            get { return Math.Round(systemState.StateY.Position, 2); }
        }

        public double PendulumSwingX
        {
            get { return Math.Round(systemState.StateX.Angle, 2); }
        }
        public double PendulumSwingY
        {
            get { return Math.Round(systemState.StateY.Angle, 2); }
        }

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

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            InitializeObjects();
            InitializeControllers();
            SetupParameters();
            SetupMenu();
        }

        /// <summary>
        /// Initialization of variables and sets
        /// </summary>
        private void InitializeObjects()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            systemState = new SystemState();
            solver = new ODESolver(systemState.SolverParameters);

            SceneControl.ResetSimulation(systemState);
            SceneControl.UpdateState(systemState);
            SceneControl.UpdateCamera(systemState);

            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimerTick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 20);
        }

        private void InitializeControllers()
        {
            xCoordVoltageController = new VoltageController();
            yCoordVoltageController = new VoltageController();
            windController = new WindController();
            trajectoryController = new TrajectoryController();
            movementController = new MovementController();
        }

        private void SetupControllers()
        {
            xCoordVoltageController.Reset(systemState.TimeDelta);
            yCoordVoltageController.Reset(systemState.TimeDelta);
        }

        private void SetupMenu()
        {
            ShowTargetTrajectoryMenuItem.IsChecked = true;
            ShowCartTrajectoryMenuItem.IsChecked = true;
            ShowPendulumTrajectoryMenuItem.IsChecked = true;

            DoublePIDParallelVoltageMenuItem.IsChecked = true;
            RandomSmoothWindMenuItem.IsChecked = true;
        }

        private void SetupParameters()
        {
            XCoordAngle = 0.0;
            YCoordAngle = 0.0;
            systemState.Reset(XCoordAngle, YCoordAngle);
            RodLength = systemState.SolverParameters.PendulumLength;
            movementController.CorrectAngleY = 0.0;
            movementController.CorrectAngleX = 0.0;
            WindPower = windController.DefaultWindPower;
            AnimationSpeed = systemState.DefaultTimeDelta;
            xCoordVoltageController.ControlType = VoltageController.DefaultControlType;
            yCoordVoltageController.ControlType = VoltageController.DefaultControlType;
            windController.WindType = WindController.DefaultWindType;

            UpdateGUI();
        }

        /// <summary>
        /// Dispatcher timer cyclic function
        /// </summary>
        private void DispatcherTimerTick(object sender, EventArgs e)
        {
            bool nextCheckPoint;
            var targetPosition = trajectoryController.GetTargetAccuratePosition(systemState.StateX.Position, systemState.StateY.Position, out nextCheckPoint);
            if (nextCheckPoint)
            {
                xCoordVoltageController.Reset(systemState.TimeDelta);
                yCoordVoltageController.Reset(systemState.TimeDelta);
            }

            SceneControl.UpdateWindDirection(windController.UpdateWindForce(), windController.WindPower);

            xCoordVoltageController.SetTime(systemState.Time);
            xCoordVoltageController.SetControlError(movementController.CorrectAngleX - systemState.StateX.Angle, targetPosition.X - systemState.StateX.Position);
            var xCoordVoltage = xCoordVoltageController.GetVoltage();
            systemState.SolverParameters.Voltage = xCoordVoltage;
            systemState.SolverParameters.VerticalWindForce = windController.GetZCoordWindPower();
            systemState.SolverParameters.HorizontalWindForce = windController.GetXCoordWindPower();

            solver.UpdateSystemParameters(systemState.SolverParameters);
            var t = systemState.ToTimeArray();
            var x = systemState.StateX.ToStateArray();
            var xState = solver.SolveODESystem(t, x);
            if (Math.Abs(xState.Angle) > systemState.MaxAngle)
            {
                MessageBox.Show("Pendulum lost controllability");
                dispatcherTimer.Stop();
                return;
            }

            if (Math.Abs(xState.Position) > SceneControl.SimulationAreaSize / 2 - SceneControl.cart.PlatformSize / 2)
                xState.Position = Math.Sign(xState.Position) * (SceneControl.SimulationAreaSize / 2 - SceneControl.cart.PlatformSize / 2);
            systemState.UpdateSystemStateX(xState);

            yCoordVoltageController.SetTime(systemState.Time);
            yCoordVoltageController.SetControlError(movementController.CorrectAngleY - systemState.StateY.Angle, targetPosition.Y - systemState.StateY.Position);
            var yCoordVoltage = yCoordVoltageController.GetVoltage();
            systemState.SolverParameters.Voltage = yCoordVoltage;
            systemState.SolverParameters.HorizontalWindForce = windController.GetYCoordWindPower();

            var y = systemState.StateY.ToStateArray();
            var yState = solver.SolveODESystem(t, y);
            if (Math.Abs(yState.Angle) > systemState.MaxAngle)
            {
                MessageBox.Show("Pendulum lost controllability");
                dispatcherTimer.Stop();
                return;
            }
            if (Math.Abs(yState.Position) > SceneControl.SimulationAreaSize / 2 - SceneControl.cart.PlatformSize / 2)
                yState.Position = Math.Sign(yState.Position) * (SceneControl.SimulationAreaSize / 2 - SceneControl.cart.PlatformSize / 2);
            systemState.UpdateSystemStateY(yState);

            PlotsControl.UpdatePlots(systemState, xCoordVoltage, yCoordVoltage);
            systemState.UpdateTimer();
            UpdateGUI();
            SceneControl.UpdateFrame(systemState);
        }

        private void UpdateGUI()
        {
            OnPropertyChanged("CartPositionX");
            OnPropertyChanged("CartPositionY");
            OnPropertyChanged("PendulumSwingX");
            OnPropertyChanged("PendulumSwingY");
        }

        /// <summary>
        /// Start animation
        /// </summary>
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!animationPlaying)
            {
                animationPlaying = true;
                ParametersPanel.IsEnabled = false;
                SceneControl.ResetSimulation(systemState);
                SetupControllers();
            }
            PlayButton.IsEnabled = false;
            PauseButton.IsEnabled = true;
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
        }

        /// <summary>
        /// Reset animation
        /// </summary>
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            animationPlaying = false;
            PlayButton.IsEnabled = true;
            PauseButton.IsEnabled = false;
            ParametersPanel.IsEnabled = true;
            dispatcherTimer.Stop();

            PlotsControl.ResetPlots();

            if (trajectoryController.TrajectoryEnabled)
            {
                var startPosition = trajectoryController.GetTargetStartPosition();
                systemState.Reset(0.0, 0.0, startPosition.X, startPosition.Y);
            }
            else
                systemState.Reset(XCoordAngle, YCoordAngle);
            SceneControl.ResetSimulation(systemState);
            xCoordVoltageController.Reset(systemState.TimeDelta);
            yCoordVoltageController.Reset(systemState.TimeDelta);
            windController.Reset();
            trajectoryController.Reset();
            movementController.Reset();

            UpdateGUI();
        }

        private void SimulationScene_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            movementController.HandleKey(e.Key);
        }

        private void LoadTrajectoryItem_Click(object sender, RoutedEventArgs e)
        {
            ResetButton_Click(null, null);
            var trajectory = trajectoryController.LoadTrajectory();
            if (trajectory == null)
                return;
            SceneControl.UpdateTrajectory(trajectory);

            var startPosition = trajectoryController.GetTargetStartPosition();
            XCoordAngle = 0.0;
            YCoordAngle = 0.0;
            systemState.Reset(0.0, 0.0, startPosition.X, startPosition.Y);
            SceneControl.UpdateState(systemState);
            SceneControl.UpdateCamera(systemState);
            UpdateGUI();
        }

        private void ClearTrajectoryItem_Click(object sender, RoutedEventArgs e)
        {
            ResetButton_Click(null, null);
            SceneControl.ClearTrajectory();
            trajectoryController.Clear();
            systemState.Reset();
            XCoordAngle = 0.0;
            YCoordAngle = 0.0;
            systemState.Reset();
            SceneControl.UpdateState(systemState);
            SceneControl.UpdateCamera(systemState);
            UpdateGUI();
        }

        private void ShowTargetTrajectoryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            SceneControl.ShowTargetTrajectory(menuItem.IsChecked);
        }

        private void ShowCartTrajectoryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            SceneControl.ShowCartTrajectory(menuItem.IsChecked);
        }

        private void ShowPendulumTrajectoryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            SceneControl.ShowPendulumTrajectory(menuItem.IsChecked);
        }

        private void SetMenuVoltage(MenuItem sender, ControlType controlType)
        {
            foreach (var item in VoltageMenuItem.Items)
                (item as MenuItem).IsChecked = false;
            sender.IsChecked = true;
            xCoordVoltageController.ControlType = controlType;
            yCoordVoltageController.ControlType = controlType;
        }

        private void DoublePIDParallelVoltageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetMenuVoltage(sender as MenuItem, ControlType.DoublePIDParallel);
        }

        private void DoublePIDCascadeVoltageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetMenuVoltage(sender as MenuItem, ControlType.DoublePIDCascade);
        }

        private void PIDVoltageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetMenuVoltage(sender as MenuItem, ControlType.PID);
        }

        private void SinusoidalVoltageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetMenuVoltage(sender as MenuItem, ControlType.Sinusoidal);
        }

        private void RandomVoltageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetMenuVoltage(sender as MenuItem, ControlType.Random);
        }


        private void SetMenuWind(MenuItem sender, WindType windType)
        {
            foreach (var item in WindMenuItem.Items)
                (item as MenuItem).IsChecked = false;
            sender.IsChecked = true;
            windController.WindType = windType;
        }

        private void RandomPeakWindMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetMenuWind(sender as MenuItem, WindType.RandomPeak);
        }

        private void RandomSwitchWindMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetMenuWind(sender as MenuItem, WindType.RandomSwitch);
        }

        private void RandomSmoothWindMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetMenuWind(sender as MenuItem, WindType.RandomSmooth);
        }
    }
}
