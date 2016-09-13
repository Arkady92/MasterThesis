using HelixToolkit.Wpf;
using OxyPlot;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace InvertedPendulumTransporter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private System.Windows.Threading.DispatcherTimer dispatcherTimer;
        private Pendulum pendulum;
        private Cart cart;
        private Solver solver;
        private SystemController systemControllerX;
        private SystemController systemControllerY;
        private SystemState systemState;
        private SystemParameters systemParameters;
        public double WindControlPowerY;
        public double WindControlPowerX;
        public const double WindControlPowerDelta = Math.PI / 60;


        ObservableCollection<DataPoint> errorPointsX;
        public ObservableCollection<DataPoint> ErrorPointsX
        {
            get { return errorPointsX; }
        }

        ObservableCollection<DataPoint> voltagePointsX;
        public ObservableCollection<DataPoint> VoltagePointsX
        {
            get { return voltagePointsX; }
        }

        ObservableCollection<DataPoint> errorPointsY;
        public ObservableCollection<DataPoint> ErrorPointsY
        {
            get { return errorPointsY; }
        }

        ObservableCollection<DataPoint> voltagePointsY;
        public ObservableCollection<DataPoint> VoltagePointsY
        {
            get { return voltagePointsY; }
        }

        /// <summary>
        /// Animation speed ration [based on inverse exponential function (1 / 2^x)]
        /// </summary>
        public double AnimationSpeed
        {
            get { return systemState.TimeDelta; }
            set
            {
                if (value != systemState.TimeDelta)
                {
                    systemState.TimeDelta = value;
                    OnPropertyChanged("AnimationSpeed");
                }
            }
        }

        public double HorizontalAngle
        {
            get { return systemState.StateX.Angle; }
            set
            {
                if (value != systemState.StateX.Angle)
                {
                    systemState.StateX.Angle = value;
                    cart.UpdateState(systemState);
                    pendulum.UpdateState(systemState);
                    OnPropertyChanged("HorizontalAngle");
                }
            }
        }

        public double VerticalAngle
        {
            get { return systemState.StateY.Angle; }
            set
            {
                if (value != systemState.StateY.Angle)
                {
                    systemState.StateY.Angle = value;
                    cart.UpdateState(systemState);
                    pendulum.UpdateState(systemState);
                    OnPropertyChanged("VerticalAngle");
                }
            }
        }

        public double RodLength
        {
            get { return pendulum.rodLength / pendulum.rodLengthFactor; }
            set
            {
                if (value != pendulum.rodLength)
                {
                    pendulum.UpdateState(systemState, value);
                    OnPropertyChanged("RodLength");
                }
            }
        }

        private bool windEnabled;
        public bool WindEnabled
        {
            get { return windEnabled; }
            set
            {
                if (value != windEnabled)
                {
                    windEnabled = value;
                    OnPropertyChanged("WindEnabled");
                }
            }
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
            Initialize();
            InitializeScene();
            SetupParameters();
            SetupController();
        }

        private void SetupController()
        {
            systemControllerX = new SystemController();
            systemControllerX.controlType = ControlType.PID;
            systemControllerX.TimeDelta = systemState.TimeDelta;

            systemControllerY = new SystemController();
            systemControllerY.controlType = ControlType.PID;
            systemControllerY.TimeDelta = systemState.TimeDelta;
        }

        private void SetupParameters()
        {
            //systemState.StateX.Position = 0.1;
            HorizontalAngle = Math.PI / 3;
            VerticalAngle = Math.PI / 3;
            RodLength = systemParameters.PendulumLength;
            WindControlPowerX = 0.0;
            WindControlPowerY = 0.0;
            cart.UpdateState(systemState);
            pendulum.UpdateState(systemState);
        }

        /// <summary>
        /// Initialization of variables and sets
        /// </summary>
        private void Initialize()
        {
            systemParameters = new SystemParameters();
            solver = new Solver(systemParameters);
            systemState = new SystemState();

            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimerTick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 20);// (int)systemState.TimeDelta);

            errorPointsX = new ObservableCollection<DataPoint>();
            voltagePointsX = new ObservableCollection<DataPoint>();
            errorPointsY = new ObservableCollection<DataPoint>();
            voltagePointsY = new ObservableCollection<DataPoint>();

            WindEnabled = true;
        }

        private void InitializeScene()
        {
            var floor = new ModelVisual3D();
            floor.Children.Add(new GridLinesVisual3D() { Center = new Point3D(0.0, 0.0, 0.0), Width = 100, Length = 100, MinorDistance = 1, MajorDistance = 10, Thickness = 0.03, Fill = Brushes.Green });
            SimulationScene.Children.Add(floor);

            cart = new Cart();
            cart.UpdateState(systemState);
            SimulationScene.Children.Add(cart.cartModel);

            pendulum = new Pendulum();
            pendulum.UpdateState(systemState);
            SimulationScene.Children.Add(pendulum.pendulumModel);
        }

        /// <summary>
        /// Dispatcher timer cyclic function
        /// </summary>
        private void DispatcherTimerTick(object sender, EventArgs e)
        {
            systemControllerX.SetTime(systemState.Time);
            systemControllerX.SetControlError(WindControlPowerX - systemState.StateX.Angle);
            systemParameters.Voltage = systemControllerX.GetVoltage();
            voltagePointsX.Add(new DataPoint(systemState.Time, systemParameters.Voltage));
            errorPointsX.Add(new DataPoint(systemState.Time, -systemState.StateX.Angle));

            solver.UpdateSystemParameters(systemParameters);
            var t = systemState.ToTimeArray();
            var x = systemState.StateX.ToStateArray();
            var xState = solver.SolveODESystem(t, x);
            if (Math.Abs(xState.Angle) > Math.PI / 2)
            {
                xState.Angle = Math.Sign(xState.Angle) * Math.PI / 2;
                dispatcherTimer.Stop();
            }
            systemState.UpdateSystemStateX(xState);

            systemControllerY.SetTime(systemState.Time);
            systemControllerY.SetControlError(WindControlPowerY - systemState.StateY.Angle);
            systemParameters.Voltage = systemControllerY.GetVoltage();
            voltagePointsY.Add(new DataPoint(systemState.Time, systemParameters.Voltage));
            errorPointsY.Add(new DataPoint(systemState.Time, -systemState.StateY.Angle));

            var y = systemState.StateY.ToStateArray();
            var yState = solver.SolveODESystem(t, y);
            if (Math.Abs(yState.Angle) > Math.PI / 2)
            {
                yState.Angle = Math.Sign(yState.Angle) * Math.PI / 2;
                dispatcherTimer.Stop();
            }
            systemState.UpdateSystemStateY(yState);

            systemState.UpdateTimer();
            cart.UpdateState(systemState);
            pendulum.UpdateState(systemState);
        }

        /// <summary>
        /// Start animation
        /// </summary>
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Start();
        }

        /// <summary>
        /// Pause animation
        /// </summary>
        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
        }

        /// <summary>
        /// Reset animation
        /// </summary>
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            errorPointsX.Clear();
            voltagePointsX.Clear();
            errorPointsY.Clear();
            voltagePointsY.Clear();
            systemState.InitializeDefault();
            systemControllerX.InitializeDefault();
            systemControllerY.InitializeDefault();
            SetupParameters();
        }

        private void SimulationScene_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.T:
                    WindControlPowerY -= WindControlPowerDelta;
                    break;
                case Key.G:
                    WindControlPowerY += WindControlPowerDelta;
                    break;
                case Key.F:
                    WindControlPowerX += WindControlPowerDelta;
                    break;
                case Key.H:
                    WindControlPowerX -= WindControlPowerDelta;
                    break;
            }
        }
    }
}
