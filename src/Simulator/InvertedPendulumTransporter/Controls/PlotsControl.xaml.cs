using OxyPlot;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System;
using InvertedPendulumTransporterPhysics.Common;

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
    }
}
