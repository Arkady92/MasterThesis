using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;
using MathNet.Symbolics;

namespace InvertedPendulumTransporter.Windows
{
    /// <summary>
    /// Interaction logic for CreateTrajectoryWindow.xaml
    /// </summary>
    public partial class CreateTrajectoryWindow : Window
    {
        #region Private Members
        private Dictionary<string, FloatingPoint> symbols;
        private MathNet.Symbolics.Expression xExpression;
        private MathNet.Symbolics.Expression yExpression;
        private MathNet.Symbolics.Expression symbol;
        private double minParameterValue;
        private double maxParameterValue;
        private double pointsCount;
        private const int HResultWrongParam = -2146232969;
        private const int HResultCannotCalculate = -2146233088;
        #endregion

        #region Public Members
        /// <summary>
        /// Result trajectory points
        /// </summary>
        public List<Point3D> TrajectoryPoints { get; private set; }

        /// <summary>
        /// Information about trajectory creation succes
        /// </summary>
        public bool TrajectoryLoaded { get; private set; }
        #endregion

        #region Private Methods
        /// <summary>
        /// Initialize objects for symbolics calculations
        /// </summary>
        private void Initialize()
        {
            symbol = MathNet.Symbolics.Expression.Symbol("t");
            symbols = new Dictionary<string, FloatingPoint> { { "t", 0 } };
        }

        /// <summary>
        /// Create trajectory from given parametrization
        /// </summary>
        private void CreateTrajectory()
        {
            // Try to parse parametrizations' strings
            try
            {
                xExpression = Infix.ParseOrThrow(XParametrizationTextBox.Text);
                yExpression = Infix.ParseOrThrow(YParametrizationTextBox.Text);
            }
            catch (Exception exception)
            {
                TrajectoryLoaded = false;
                MessageBox.Show("Wrong format of parametrization.\n" + exception.Message);
                return;
            }

            // Try to parse parameters
            try
            {
                minParameterValue = double.Parse(MinParameterValueTextBox.Text);
                maxParameterValue = double.Parse(MaxParameterValueTextBox.Text);
                pointsCount = double.Parse(PointsCountTextBox.Text);
                if (minParameterValue >= maxParameterValue || pointsCount < 2)
                    throw new Exception();
            }
            catch (Exception)
            {
                TrajectoryLoaded = false;
                MessageBox.Show("Parameters are invalid.");
                return;
            }

            TrajectoryPoints = new List<Point3D>();

            var flag = true;
            // Try to calculate curve's trajectory
            try
            {
                var parameterStep = (maxParameterValue - minParameterValue) / (pointsCount - 1);
                for (double t = minParameterValue; t < maxParameterValue + parameterStep / 2; t += parameterStep)
                {
                    symbols["t"] = t;
                    double xResult = Evaluate.Evaluate(symbols, xExpression).RealValue;
                    double yResult = Evaluate.Evaluate(symbols, yExpression).RealValue;
                    TrajectoryPoints.Add(new Point3D(xResult, yResult, 0.0));
                    if (flag)
                        flag = false;
                }
            }
            catch (Exception exception)
            {
                TrajectoryLoaded = false;
                TrajectoryPoints.Clear();
                if (exception.HResult == HResultWrongParam)
                    MessageBox.Show("Given parametrization is not based on [t] variable.\n");
                else if (exception.HResult == HResultCannotCalculate)
                    MessageBox.Show("Cannot calculate curve for given set.\n");
                else
                    MessageBox.Show("Cannot calculate curve for given set.\n" + exception.Message);
                return;
            }
            TrajectoryLoaded = true;
        }

        /// <summary>
        /// Handle create trajectory button click
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="e">Event arguments</param>
        private void CreateTrajectoryButton_Click(object sender, RoutedEventArgs e)
        {
            CreateTrajectory();
            if (TrajectoryLoaded)
            {
                DialogResult = true;
                Close();
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Class constructor
        /// </summary>
        public CreateTrajectoryWindow()
        {
            InitializeComponent();
            Initialize();
        }
        #endregion
    }
}
