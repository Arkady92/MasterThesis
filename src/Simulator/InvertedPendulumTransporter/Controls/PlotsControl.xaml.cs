using OxyPlot;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System;
using InvertedPendulumTransporterPhysics.Common;
using OxyPlot.Wpf;
using Microsoft.Win32;

namespace InvertedPendulumTransporter.Controls
{
    /// <summary>
    /// Interaction logic for PlotsControl.xaml
    /// </summary>
    public partial class PlotsControl : UserControl
    {
        private ObservableCollection<DataPoint> angleErrorPointsX;
        public ObservableCollection<DataPoint> AngleErrorPointsX
        {
            get { return angleErrorPointsX; }
        }

        private ObservableCollection<DataPoint> positionErrorPointsX;
        public ObservableCollection<DataPoint> PositionErrorPointsX
        {
            get { return positionErrorPointsX; }
        }

        private ObservableCollection<DataPoint> voltagePointsX;
        public ObservableCollection<DataPoint> VoltagePointsX
        {
            get { return voltagePointsX; }
        }

        private ObservableCollection<DataPoint> angleErrorPointsY;
        public ObservableCollection<DataPoint> AngleErrorPointsY
        {
            get { return angleErrorPointsY; }
        }

        private ObservableCollection<DataPoint> positionErrorPointsY;
        public ObservableCollection<DataPoint> PositionErrorPointsY
        {
            get { return positionErrorPointsY; }
        }

        private ObservableCollection<DataPoint> voltagePointsY;
        public ObservableCollection<DataPoint> VoltagePointsY
        {
            get { return voltagePointsY; }
        }

        private double timeDelta;
        private double xCoordAngle;
        private double yCoordAngle;
        private double rodLength;
        private double cartMass;
        private double pendulumMass;
        private double windPower;

        public PlotsControl()
        {
            InitializeComponent();
            DataContext = this;
            InitializeObjects();
        }

        private void InitializeObjects()
        {
            angleErrorPointsX = new ObservableCollection<DataPoint>();
            positionErrorPointsX = new ObservableCollection<DataPoint>();
            angleErrorPointsY = new ObservableCollection<DataPoint>();
            positionErrorPointsY = new ObservableCollection<DataPoint>();
            voltagePointsX = new ObservableCollection<DataPoint>();
            voltagePointsY = new ObservableCollection<DataPoint>();
        }

        public void UpdateVoltagePlots(double time, double xCoordVoltage, double yCoordVoltage)
        {
            voltagePointsX.Add(new DataPoint(time, xCoordVoltage));
            voltagePointsY.Add(new DataPoint(time, yCoordVoltage));
        }

        public void UpdateAngleErrorPlots(double time, double xCoordError, double yCoordError)
        {
            angleErrorPointsX.Add(new DataPoint(time, xCoordError));
            angleErrorPointsY.Add(new DataPoint(time, yCoordError));
        }

        public void UpdatePositionErrorPlots(double time, double xCoordError, double yCoordError)
        {
            positionErrorPointsX.Add(new DataPoint(time, xCoordError));
            positionErrorPointsY.Add(new DataPoint(time, yCoordError));
        }

        public void ResetPlots()
        {
            angleErrorPointsX.Clear();
            angleErrorPointsY.Clear();
            positionErrorPointsX.Clear();
            positionErrorPointsY.Clear();
            voltagePointsX.Clear();
            voltagePointsY.Clear();
        }

        private void EngineVoltagePlot_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var fileName = "EngineVoltage[" + "TD=" + timeDelta + "XC=" + xCoordAngle + "YC=" + yCoordAngle + "RL=" + rodLength + "CM=" + cartMass + "PM=" + pendulumMass + "WP=" + windPower + "].png";
            SavePlotAsImage(EngineVoltagePlot.ActualModel, fileName);
        }

        private void ControlErrorPlot_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var angleFileName = "ControlError[" + "TD=" + timeDelta + "XC=" + xCoordAngle + "YC=" + yCoordAngle + "RL=" + rodLength + "CM=" + cartMass + "PM=" + pendulumMass + "WP=" + windPower + "].png";
            SavePlotAsImage(ControlErrorPlot.ActualModel, angleFileName);

            var positionFileName = "PositionError[" + "TD=" + timeDelta + "XC=" + xCoordAngle + "YC=" + yCoordAngle + "RL=" + rodLength + "CM=" + cartMass + "PM=" + pendulumMass + "WP=" + windPower + "].png";
            SavePlotAsImage(PositionErrorPlot.ActualModel, positionFileName);
        }

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
    }
}
