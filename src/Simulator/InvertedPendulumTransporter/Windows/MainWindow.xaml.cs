using System;
using System.ComponentModel;
using System.Windows;
using InvertedPendulumTransporterPhysics.Common;
using InvertedPendulumTransporterPhysics.Controllers;
using InvertedPendulumTransporterPhysics.Solvers;
using InvertedPendulumTransporter.Controls;
using System.Collections.Generic;

namespace InvertedPendulumTransporter.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml [application management]
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Private Members
        private System.Windows.Threading.DispatcherTimer dispatcherTimer;
        private IODESolver solver;
        private IVoltageController xCoordVoltageController;
        private IVoltageController yCoordVoltageController;
        private IWindController windController;
        private ITrajectoryController trajectoryController;
        private IGameController gameController;
        private List<IController> controllers;
        private SystemState systemState;
        private bool animationPlaying;
        #endregion

        #region Public Members

        #endregion

        #region Private Methods
        /// <summary>
        /// Initialization of variables and sets
        /// </summary>
        private void InitializeObjects()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            systemState = new SystemState();
            solver = new ODESolver(systemState.SolverParameters);
            solver.SetupStrategy(new InterferedSystemODESolverFunctionStrategy());

            SceneControl.ResetSimulation(systemState);
            SceneControl.UpdateState(systemState);
            SceneControl.UpdateCamera(systemState);

            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimerTick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 16);
        }

        /// <summary>
        /// Initialization of physics controllers
        /// </summary>
        private void InitializeControllers()
        {
            xCoordVoltageController = new VoltageController();
            yCoordVoltageController = new VoltageController();
            windController = new WindController();
            trajectoryController = new TrajectoryController();
            gameController = new GameController(UpKeyboardButton, DownKeyboardButton, LeftKeyboardButton, RightKeyboardButton);

            controllers = new List<IController>();
            controllers.Add(xCoordVoltageController);
            controllers.Add(yCoordVoltageController);
            controllers.Add(windController);
            controllers.Add(trajectoryController);
            controllers.Add(gameController);
        }

        /// <summary>
        /// Controllers setup
        /// </summary>
        private void SetupControllers()
        {
            xCoordVoltageController.Reset(systemState.TimeDelta);
            yCoordVoltageController.Reset(systemState.TimeDelta);
        }

        /// <summary>
        /// System parameters setup
        /// </summary>
        private void SetupParameters()
        {
            XCoordAngle = 0.0;
            YCoordAngle = 0.0;
            systemState.Reset(XCoordAngle, YCoordAngle);
            RodLength = systemState.SolverParameters.PendulumLength;
            WindPower = windController.DefaultWindPower;
            TimeDelta = systemState.DefaultTimeDelta;
            xCoordVoltageController.ControlType = VoltageController.DefaultControlType;
            yCoordVoltageController.ControlType = VoltageController.DefaultControlType;
            windController.WindType = WindController.DefaultWindType;
            UpdateGUI();
        }

        /// <summary>
        /// Dispatcher timer cyclic function
        /// </summary>
        /// <remarks>
        /// Method responsible for the whole simulation process. It manages calculations loop.
        /// </remarks>
        private void DispatcherTimerTick(object sender, EventArgs e)
        {
            // Get info about trajectory
            bool nextCheckPoint;
            var targetPosition = trajectoryController.GetTargetPosition(systemState.StateX.Position, systemState.StateY.Position, out nextCheckPoint);
            if (nextCheckPoint)
            {
                xCoordVoltageController.Reset(systemState.TimeDelta);
                yCoordVoltageController.Reset(systemState.TimeDelta);
            }

            // Get info about interferences
            SceneControl.UpdateWindDirection(windController.UpdateWindForce(), windController.WindPower);

            // Calculate X-coordinate system
            double xCoordVoltage = 0.0;
            var resultXState = ExecuteSystemCalculations(xCoordVoltageController, systemState.StateX, gameController.UserAngleX,
                targetPosition.X, -windController.GetZCoordWindPower(), -windController.GetXCoordWindPower(), out xCoordVoltage);
            if (resultXState == null) return;
            systemState.UpdateSystemStateX(resultXState);

            // Calculate Y-coordinate system
            double yCoordVoltage = 0.0;
            var resultYState = ExecuteSystemCalculations(yCoordVoltageController, systemState.StateY, gameController.UserAngleY,
                targetPosition.Y, -windController.GetZCoordWindPower(), -windController.GetYCoordWindPower(), out yCoordVoltage);
            if (resultYState == null) return;
            systemState.UpdateSystemStateY(resultYState);

            // Update plots
            PlotsControl.PassParameters(TimeDelta, XCoordAngle, YCoordAngle, RodLength, CartMass, PendulumMass, WindPower);
            PlotsControl.UpdateVoltagePlots(systemState.Time, xCoordVoltage, yCoordVoltage);
            PlotsControl.UpdateAngleErrorPlots(systemState.Time, systemState.StateX.Angle, systemState.StateY.Angle);
            PlotsControl.UpdatePositionErrorPlots(systemState.Time, targetPosition.X - systemState.StateX.Position,
                targetPosition.Y - systemState.StateY.Position);

            // Update visualization
            systemState.UpdateTimer();
            UpdateGUI();
            SceneControl.UpdateFrame(systemState);
        }

        /// <summary>
        /// Calculate system dynamics
        /// </summary>
        /// <param name="voltageController">Reference to voltage controller</param>
        /// <param name="state">Actual system state</param>
        /// <param name="userAngle">Pendulum angle generated by user</param>
        /// <param name="targetPosition">Trajectory target position</param>
        /// <param name="verticalWindForce">Vertical wind force value</param>
        /// <param name="horizontalWindForce">Horizontal wind formce value</param>
        /// <param name="voltage">Result regulation voltage</param>
        /// <returns>New system state</returns>
        private OneDimensionalSystemState ExecuteSystemCalculations(IVoltageController voltageController, OneDimensionalSystemState state,
            double userAngle, double targetPosition, double verticalWindForce, double horizontalWindForce, out double voltage)
        {
            voltageController.SetTime(systemState.Time);
            voltageController.SetUserAngle(userAngle);
            voltageController.SetControlError(userAngle - state.Angle, targetPosition - state.Position);
            voltage = voltageController.GetVoltage();
            systemState.SolverParameters.Voltage = voltage;
            systemState.SolverParameters.VerticalWindForce = verticalWindForce;
            systemState.SolverParameters.HorizontalWindForce = horizontalWindForce;

            solver.UpdateSystemParameters(systemState.SolverParameters);
            var t = systemState.ToTimeArray();
            var x = state.ToStateArray();
            var resultState = solver.SolveODESystem(t, x);
            if (Math.Abs(resultState.Angle) > systemState.MaxAngle)
            {
                MessageBox.Show("Pendulum lost controllability");
                dispatcherTimer.Stop();
                return null;
            }
            if (Math.Abs(resultState.Position) > SceneControl.simulationArea.Size / 2 - SceneControl.cart.PlatformSize / 2)
                resultState.Position = Math.Sign(resultState.Position) * (SceneControl.simulationArea.Size / 2
                    - SceneControl.cart.PlatformSize / 2);
            return resultState;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Class constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            InitializeObjects();
            InitializeControllers();
            SetupParameters();
            SetupMenu();
        }
        #endregion
    }
}
