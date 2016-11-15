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
        private WindController windController;
        private SystemState systemState;
        private SystemParameters systemParameters;
        public double CorrectAngleX;
        public double CorrectAngleY;
        public const double WindControlPowerDelta = Math.PI / 60;

        public double MaxAngle { get { return Math.PI / 2.0 * 0.75; } }
        public double MinAngle { get { return -Math.PI / 2.0 * 0.75; } }


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

        private double horizontalAngle;
        public double HorizontalAngle
        {
            get { return horizontalAngle; }
            set
            {
                value = Math.Round(value, 2);
                if (value != horizontalAngle)
                {
                    horizontalAngle = value;
                    systemState.StateX.Angle = value;
                    cart.UpdateState(systemState);
                    pendulum.UpdateState(systemState);
                    OnPropertyChanged("HorizontalAngle");
                }
            }
        }

        private double varticalAngle;
        public double VerticalAngle
        {
            get { return varticalAngle; }
            set
            {
                value = Math.Round(value, 2);
                if (value != varticalAngle)
                {
                    varticalAngle = value;
                    systemState.StateY.Angle = value;
                    cart.UpdateState(systemState);
                    pendulum.UpdateState(systemState);
                    OnPropertyChanged("VerticalAngle");
                }
            }
        }

        public double RodLength
        {
            get { return Math.Round(pendulum.rodLength / pendulum.rodLengthFactor, 2); }
            set
            {
                value = Math.Round(value, 2);
                if (value != pendulum.rodLength)
                {
                    pendulum.UpdateState(systemState, value);
                    OnPropertyChanged("RodLength");
                }
            }
        }

        private double windPower;
        private Point3D sceneCenter;
        private const double SimulationAreaSize = 100.0;
        private bool animationPlaying;
        private Point3D targetPostion;

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
            SetupControllers();
            SetupParameters();
        }

        private void SetupControllers()
        {
            if (systemControllerX == null)
                systemControllerX = new SystemController();
            systemControllerX.ControlType = ControlType.DoublePID;
            systemControllerX.Reset(systemState.TimeDelta);

            if (systemControllerY == null)
                systemControllerY = new SystemController();
            systemControllerY.ControlType = ControlType.DoublePID;
            systemControllerY.Reset(systemState.TimeDelta);

            if (windController == null)
                windController = new WindController();
            windController.WindPower = WindPower;
            windController.WindType = WindType.RandomSwitch;
        }

        private void SetupParameters()
        {
            HorizontalAngle = 0.0; // Math.PI / 6;
            VerticalAngle = 0.0; // Math.PI / 6;
            RodLength = systemParameters.PendulumLength;
            CorrectAngleY = 0.0;
            CorrectAngleX = 0.0;
            WindPower = 0.0;
            AnimationSpeed = systemState.DefaultTimeDelta;
            targetPostion = new Point3D(5, 0, 0);
            UpdateSimuation();
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
        }

        private void InitializeScene()
        {
            WindDirectionScene.Camera = SimulationScene.Camera;

            var floor = new ModelVisual3D();
            floor.Children.Add(new GridLinesVisual3D() { Center = new Point3D(0.0, 0.0, 0.0), Width = SimulationAreaSize, Length = SimulationAreaSize, MinorDistance = 2, MajorDistance = 10, Thickness = 0.03, Fill = Brushes.Green });
            floor.Children.Add(new RectangleVisual3D() { Origin = new Point3D(0.0, 0.0, -0.01), Normal = new Vector3D(0, 0, 1), Width = SimulationAreaSize, Length = SimulationAreaSize, Fill = Brushes.Tomato });
            SimulationScene.Children.Add(floor);

            var walls = new ModelVisual3D();
            var wallHeight = SimulationAreaSize / 20.0;
            walls.Children.Add(new GridLinesVisual3D() { Center = new Point3D(0.0, SimulationAreaSize / 2, wallHeight / 2.0), Normal = new Vector3D(0.0, 1.0, 0), Width = wallHeight, Length = SimulationAreaSize, Fill = Brushes.Green });
            walls.Children.Add(new GridLinesVisual3D() { Center = new Point3D(0.0, -SimulationAreaSize / 2, wallHeight / 2.0), Normal = new Vector3D(0.0, 1.0, 0), Width = wallHeight, Length = SimulationAreaSize, Fill = Brushes.Green });
            walls.Children.Add(new GridLinesVisual3D() { Center = new Point3D(SimulationAreaSize / 2, 0.0, wallHeight / 2.0), Normal = new Vector3D(1.0, 0.0, 0), Width = SimulationAreaSize, Length = wallHeight, Fill = Brushes.Green });
            walls.Children.Add(new GridLinesVisual3D() { Center = new Point3D(-SimulationAreaSize / 2, 0.0, wallHeight / 2.0), Normal = new Vector3D(1.0, 0.0, 0), Width = SimulationAreaSize, Length = wallHeight, Fill = Brushes.Green });
            SimulationScene.Children.Add(walls);

            cart = new Cart();
            cart.UpdateState(systemState);
            SimulationScene.Children.Add(cart.cartModel);

            pendulum = new Pendulum();
            pendulum.UpdateState(systemState);
            SimulationScene.Children.Add(pendulum.pendulumModel);

            //windArrow = new ArrowVisual3D();
            //windArrow.Fill = Brushes.Red;
            //windArrow.Diameter = 1.0;
            //windArrow.Direction = new Vector3D(0, 0, 1);
            ////windArrow.Point2 = new Point3D(0, 0, 2);
            //WindDirectionScene.Children.Add(windArrow);
        }

        /// <summary>
        /// Dispatcher timer cyclic function
        /// </summary>
        private void DispatcherTimerTick(object sender, EventArgs e)
        {
            windController.UpdateWindForce();
            WindArrow.Point1 = (sceneCenter.ToVector3D() + windController.WindDirection * -5.0).ToPoint3D();
            WindArrow.Point2 = (sceneCenter.ToVector3D() + windController.WindDirection * 5.0).ToPoint3D();
            systemControllerX.SetTime(systemState.Time);
            systemControllerX.SetControlError(CorrectAngleX - systemState.StateX.Angle, targetPostion.X - systemState.StateX.Position);
            systemParameters.Voltage = systemControllerX.GetVoltage();
            systemParameters.VerticalWindForce = windController.GetVerticalWindPower();
            systemParameters.HorizontalWindForce = windController.GetHorizontalXWindPower();

            voltagePointsX.Add(new DataPoint(systemState.Time, systemParameters.Voltage));
            errorPointsX.Add(new DataPoint(systemState.Time, -systemState.StateX.Angle));

            solver.UpdateSystemParameters(systemParameters);
            var t = systemState.ToTimeArray();
            var x = systemState.StateX.ToStateArray();
            var xState = solver.SolveODESystem(t, x);
            if (Math.Abs(xState.Angle) > MaxAngle)
            {
                //HorizontalAngle = Math.Sign(xState.Angle) * MaxAngle;
                MessageBox.Show("Pendulum lost controllability");
                dispatcherTimer.Stop();
                return;
            }
            if (Math.Abs(xState.Position) > SimulationAreaSize / 2 - cart.platformSize / 2)
                xState.Position = Math.Sign(xState.Position) * (SimulationAreaSize / 2 - cart.platformSize / 2);
            systemState.UpdateSystemStateX(xState);

            systemControllerY.SetTime(systemState.Time);
            systemControllerY.SetControlError(CorrectAngleY - systemState.StateY.Angle, targetPostion.Y - systemState.StateY.Position);
            systemParameters.Voltage = systemControllerY.GetVoltage();
            systemParameters.HorizontalWindForce = windController.GetHorizontalYWindPower();

            voltagePointsY.Add(new DataPoint(systemState.Time, systemParameters.Voltage));
            errorPointsY.Add(new DataPoint(systemState.Time, -systemState.StateY.Angle));

            var y = systemState.StateY.ToStateArray();
            var yState = solver.SolveODESystem(t, y);
            if (Math.Abs(yState.Angle) > MaxAngle)
            {
                //HorizontalAngle = Math.Sign(yState.Angle) * MaxAngle;
                MessageBox.Show("Pendulum lost controllability");
                dispatcherTimer.Stop();
                return;
            }
            if (Math.Abs(yState.Position) > SimulationAreaSize / 2 - cart.platformSize / 2)
                yState.Position = Math.Sign(yState.Position) * (SimulationAreaSize / 2 - cart.platformSize / 2);
            systemState.UpdateSystemStateY(yState);
            systemState.UpdateTimer();
            UpdateSimuation();

        }

        private void UpdateSimuation()
        {
            cart.UpdateState(systemState);
            pendulum.UpdateState(systemState);
            sceneCenter = new Point3D(systemState.StateX.Position, systemState.StateY.Position, pendulum.rodLength / 2);
            SimulationScene.Camera.LookAt(sceneCenter, 0.0);
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
                if (Math.Abs(WindPower) > double.Epsilon)
                    WindArrow.Visible = true;
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
            dispatcherTimer.Stop();
            errorPointsX.Clear();
            voltagePointsX.Clear();
            errorPointsY.Clear();
            voltagePointsY.Clear();
            systemState.Reset(HorizontalAngle, VerticalAngle);
            UpdateSimuation();
            systemControllerX.Reset(systemState.TimeDelta);
            systemControllerY.Reset(systemState.TimeDelta);
            windController.Reset();
            CorrectAngleY = 0.0;
            CorrectAngleX = 0.0;
            ParametersPanel.IsEnabled = true;
            WindArrow.Visible = false;
        }

        private void SimulationScene_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.T:
                    CorrectAngleY -= WindControlPowerDelta;
                    break;
                case Key.G:
                    CorrectAngleY += WindControlPowerDelta;
                    break;
                case Key.F:
                    CorrectAngleX += WindControlPowerDelta;
                    break;
                case Key.H:
                    CorrectAngleX -= WindControlPowerDelta;
                    break;
            }
        }
    }
}
