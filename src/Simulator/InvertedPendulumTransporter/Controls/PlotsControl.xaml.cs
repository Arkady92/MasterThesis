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
        private ObservableCollection<DataPoint> errorPointsX;
        public ObservableCollection<DataPoint> ErrorPointsX
        {
            get { return errorPointsX; }
        }

        private ObservableCollection<DataPoint> voltagePointsX;
        public ObservableCollection<DataPoint> VoltagePointsX
        {
            get { return voltagePointsX; }
        }

        private ObservableCollection<DataPoint> errorPointsY;
        public ObservableCollection<DataPoint> ErrorPointsY
        {
            get { return errorPointsY; }
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
            errorPointsX = new ObservableCollection<DataPoint>();
            voltagePointsX = new ObservableCollection<DataPoint>();
            errorPointsY = new ObservableCollection<DataPoint>();
            voltagePointsY = new ObservableCollection<DataPoint>();
        }

        public void UpdateVoltagePlots(double time, double xCoordVoltage, double yCoordVoltage)
        {
            voltagePointsX.Add(new DataPoint(time, xCoordVoltage));
            voltagePointsY.Add(new DataPoint(time, yCoordVoltage));
        }

        public void UpdateErrorPlots(double time, double xCoordError, double yCoordError)
        {
            errorPointsX.Add(new DataPoint(time, xCoordError));
            errorPointsY.Add(new DataPoint(time, yCoordError));
        }

        public void ResetPlots()
        {
            errorPointsX.Clear();
            voltagePointsX.Clear();
            errorPointsY.Clear();
            voltagePointsY.Clear();
        }

        private void EngineVoltagePlot_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var fileName = "EngineVoltage[" + "TD=" + timeDelta + "XC=" + xCoordAngle + "YC=" + yCoordAngle + "RL=" + rodLength + "CM=" + cartMass + "PM=" + pendulumMass + "WP=" + windPower + "].png";
            SavePlotAsImage(EngineVoltagePlot.ActualModel, fileName);
        }

        private void ControlErrorPlot_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var fileName = "ControlError[" + "TD=" + timeDelta + "XC=" + xCoordAngle + "YC=" + yCoordAngle + "RL=" + rodLength + "CM=" + cartMass + "PM=" + pendulumMass + "WP=" + windPower + "].png";
            SavePlotAsImage(ControlErrorPlot.ActualModel, fileName);
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
