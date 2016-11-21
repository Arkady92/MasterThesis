using System;
using System.ComponentModel;
using System.Windows;
using InvertedPendulumTransporterPhysics.Common;
using InvertedPendulumTransporterPhysics.Controllers;
using InvertedPendulumTransporterPhysics.Solvers;
using InvertedPendulumTransporter.Controls;

namespace InvertedPendulumTransporter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private System.Windows.Threading.DispatcherTimer dispatcherTimer;
        private IODESolver solver;
        private IVoltageController xCoordVoltageController;
        private IVoltageController yCoordVoltageController;
        private IWindController windController;
        private ITrajectoryController trajectoryController;
        private IGameController gameController;
        private SystemState systemState;
        private bool animationPlaying;

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
            solver.SetupStrategy(new NoisySystemODESolverFunctionStrategy());

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
            gameController = new GameController(UpKeyboardButton, DownKeyboardButton, LeftKeyboardButton, RightKeyboardButton);
        }

        private void SetupControllers()
        {
            xCoordVoltageController.Reset(systemState.TimeDelta);
            yCoordVoltageController.Reset(systemState.TimeDelta);
        }

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

            double xCoordVoltage = 0.0;
            var resultXState = ExecuteSystemCalculations(xCoordVoltageController, systemState.StateX, gameController.UserAngleX, 
                targetPosition.X, -windController.GetZCoordWindPower(), -windController.GetXCoordWindPower(), out xCoordVoltage);
            if (resultXState == null) return;
            systemState.UpdateSystemStateX(resultXState);

            double yCoordVoltage = 0.0;
            var resultYState = ExecuteSystemCalculations(yCoordVoltageController, systemState.StateY, gameController.UserAngleY, 
                targetPosition.Y, -windController.GetZCoordWindPower(), -windController.GetYCoordWindPower(), out yCoordVoltage);
            if (resultYState == null) return;
            systemState.UpdateSystemStateY(resultYState);

            PlotsControl.UpdateVoltagePlots(systemState.Time, xCoordVoltage, yCoordVoltage);
            PlotsControl.UpdateErrorPlots(systemState.Time, systemState.StateX.Angle, systemState.StateY.Angle);
            systemState.UpdateTimer();
            UpdateGUI();
            SceneControl.UpdateFrame(systemState);
        }

        private OneDimensionalSystemState ExecuteSystemCalculations(IVoltageController voltageController, OneDimensionalSystemState state, 
            double userAngle, double targetPosition, double verticalWindForce, double horizontalWindForce, out double voltage)
        {
            voltageController.SetTime(systemState.Time);
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
            if (Math.Abs(resultState.Position) > SceneControl.SimulationAreaSize / 2 - SceneControl.cart.PlatformSize / 2)
                resultState.Position = Math.Sign(resultState.Position) * (SceneControl.SimulationAreaSize / 2 
                    - SceneControl.cart.PlatformSize / 2);
            return resultState;
        }
    }
}
