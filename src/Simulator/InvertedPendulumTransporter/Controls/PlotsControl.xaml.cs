using OxyPlot;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using OxyPlot.Wpf;
using Microsoft.Win32;

namespace InvertedPendulumTransporter.Controls
{
    /// <summary>
    /// Interaction logic for PlotsControl.xaml
    /// </summary>
    public partial class PlotsControl : UserControl
    {
        #region Private Members
        private ObservableCollection<DataPoint> angleErrorPointsX;
        private ObservableCollection<DataPoint> positionErrorPointsX;
        private ObservableCollection<DataPoint> voltagePointsX;
        private ObservableCollection<DataPoint> angleErrorPointsY;
        private ObservableCollection<DataPoint> positionErrorPointsY;
        private ObservableCollection<DataPoint> voltagePointsY;

        private double timeDelta;
        private double xCoordAngle;
        private double yCoordAngle;
        private double rodLength;
        private double cartMass;
        private double pendulumMass;
        private double windPower;
        #endregion

        #region Public Members
        /// <summary>
        /// Collection for pendulum angle errors in X-coordinate
        /// </summary>
        public ObservableCollection<DataPoint> AngleErrorPointsX
        {
            get { return angleErrorPointsX; }
        }

        /// <summary>
        /// Collection for cart position errors in X-coordinate
        /// </summary>
        public ObservableCollection<DataPoint> PositionErrorPointsX
        {
            get { return positionErrorPointsX; }
        }

        /// <summary>
        /// Collection for motor voltage values in X-coordinate
        /// </summary>
        public ObservableCollection<DataPoint> VoltagePointsX
        {
            get { return voltagePointsX; }
        }

        /// <summary>
        /// Collection for cart pendulum angle errors in Y-coordinate
        /// </summary>
        public ObservableCollection<DataPoint> AngleErrorPointsY
        {
            get { return angleErrorPointsY; }
        }

        /// <summary>
        /// Collection for cart position errors in Y-coordinate
        /// </summary>
        public ObservableCollection<DataPoint> PositionErrorPointsY
        {
            get { return positionErrorPointsY; }
        }

        /// <summary>
        /// Collection for motor voltage values in Y-coordinate
        /// </summary>
        public ObservableCollection<DataPoint> VoltagePointsY
        {
            get { return voltagePointsY; }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Initialize plots data collections
        /// </summary>
        private void InitializeObjects()
        {
            angleErrorPointsX = new ObservableCollection<DataPoint>();
            positionErrorPointsX = new ObservableCollection<DataPoint>();
            angleErrorPointsY = new ObservableCollection<DataPoint>();
            positionErrorPointsY = new ObservableCollection<DataPoint>();
            voltagePointsX = new ObservableCollection<DataPoint>();
            voltagePointsY = new ObservableCollection<DataPoint>();
        }

        /// <summary>
        /// Handle right mouse button click on engine voltage plot
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="eventArgs">Event arguments</param>
        private void EngineVoltagePlot_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs eventArgs)
        {
            var fileName = "EngineVoltage[" + "TD=" + timeDelta + "XC=" + xCoordAngle + "YC=" + yCoordAngle + "RL=" + rodLength + "CM=" + cartMass + "PM=" + pendulumMass + "WP=" + windPower + "].png";
            SavePlotAsImage(EngineVoltagePlot.ActualModel, fileName);
        }

        /// <summary>
        /// Handle right mouse button click on control error plot
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="eventArgs">Event arguments</param>
        private void ControlErrorPlot_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs eventArgs)
        {
            var angleFileName = "ControlError[" + "TD=" + timeDelta + "XC=" + xCoordAngle + "YC=" + yCoordAngle + "RL=" + rodLength + "CM=" + cartMass + "PM=" + pendulumMass + "WP=" + windPower + "].png";
            SavePlotAsImage(ControlErrorPlot.ActualModel, angleFileName);

            var positionFileName = "PositionError[" + "TD=" + timeDelta + "XC=" + xCoordAngle + "YC=" + yCoordAngle + "RL=" + rodLength + "CM=" + cartMass + "PM=" + pendulumMass + "WP=" + windPower + "].png";
            SavePlotAsImage(PositionErrorPlot.ActualModel, positionFileName);
        }

        /// <summary>
        /// Save plot as image
        /// </summary>
        /// <param name="model">Plot model</param>
        /// <param name="fileName">Target file name</param>
        private void SavePlotAsImage(IPlotModel model, string fileName)
        {
            var fileDialog = new SaveFileDialog
            {
                FileName = fileName,
                Filter = @"Image files (*.png)|*.png",
            };

            bool? userClickedOK = fileDialog.ShowDialog();

            if (userClickedOK == true)
            {
                var pngExporter = new PngExporter { Width = 800, Height = 400, Background = OxyColors.White };
                pngExporter.ExportToFile(model, fileDialog.FileName);
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Class constructor
        /// </summary>
        public PlotsControl()
        {
            InitializeComponent();
            DataContext = this;
            InitializeObjects();
        }

        /// <summary>
        /// Update voltage plot data
        /// </summary>
        /// <param name="time">Animation time</param>
        /// <param name="xCoordVoltage">Voltage value in X-coordinate</param>
        /// <param name="yCoordVoltage">Voltage value in Y-coordinate</param>
        public void UpdateVoltagePlots(double time, double xCoordVoltage, double yCoordVoltage)
        {
            voltagePointsX.Add(new DataPoint(time, xCoordVoltage));
            voltagePointsY.Add(new DataPoint(time, yCoordVoltage));
        }

        /// <summary>
        /// Update angle error plot data
        /// </summary>
        /// <param name="time">Animation time</param>
        /// <param name="xCoordError">Angle error in X-coordinate</param>
        /// <param name="yCoordError">Angle error in Y-coordinate</param>
        public void UpdateAngleErrorPlots(double time, double xCoordError, double yCoordError)
        {
            angleErrorPointsX.Add(new DataPoint(time, xCoordError));
            angleErrorPointsY.Add(new DataPoint(time, yCoordError));
        }

        /// <summary>
        /// Update position error plot data
        /// </summary>
        /// <param name="time">Animation time</param>
        /// <param name="xCoordError">Position error in X-coordinate</param>
        /// <param name="yCoordError">Position error in Y-coordinate</param>
        public void UpdatePositionErrorPlots(double time, double xCoordError, double yCoordError)
        {
            positionErrorPointsX.Add(new DataPoint(time, xCoordError));
            positionErrorPointsY.Add(new DataPoint(time, yCoordError));
        }

        /// <summary>
        /// Reset plots state to default
        /// </summary>
        public void ResetPlots()
        {
            angleErrorPointsX.Clear();
            angleErrorPointsY.Clear();
            positionErrorPointsX.Clear();
            positionErrorPointsY.Clear();
            voltagePointsX.Clear();
            voltagePointsY.Clear();
        }

        /// <summary>
        /// Pass actual parameters for file save info
        /// </summary>
        /// <param name="timeDelta">Time delta</param>
        /// <param name="xCoordAngle">Initial angle in X-coordinate</param>
        /// <param name="yCoordAngle">Initial angle in Y-coordinate</param>
        /// <param name="rodLength">Rod length</param>
        /// <param name="cartMass">Cart mass</param>
        /// <param name="pendulumMass">Pendulum mass</param>
        /// <param name="windPower">Wind power</param>
        public void PassParameters(double timeDelta, double xCoordAngle, double yCoordAngle, double rodLength, double cartMass, double pendulumMass, double windPower)
        {
            this.timeDelta = timeDelta;
            this.xCoordAngle = xCoordAngle;
            this.yCoordAngle = yCoordAngle;
            this.rodLength = rodLength;
            this.cartMass = cartMass;
            this.pendulumMass = pendulumMass;
            this.windPower = windPower;
        }
        #endregion
    }
}
