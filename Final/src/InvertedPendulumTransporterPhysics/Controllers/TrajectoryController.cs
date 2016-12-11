using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Media3D;

namespace InvertedPendulumTransporterPhysics.Controllers
{
    /// <summary>
    /// Controller for trajectory tracking
    /// </summary>
    public class TrajectoryController : ITrajectoryController
    {
        #region Private Members
        private List<Point3D> trajectoryPoints;
        private int actualPointIndex = 0;
        private const double ZValue = 0.05;
        private const AccuracyType DefaultAccuracyType = AccuracyType.Medium;
        private double distanceEps;
        private const double ApproximateDistanceFactor = 0.75;
        #endregion

        #region Public Members
        #region ITrajectoryController Interface
        public double AverageDistance { get; private set; }
        public bool TrajectoryAchieved { get; private set; }
        public bool TrajectoryEnabled { get; private set; }
        #endregion
        #endregion

        #region Private Methods
        /// <summary>
        /// Calculate average distace between control points
        /// </summary>
        /// <returns>Average distance</returns>
        private double CalculateAverageDistance()
        {
            var sum = 0.0;
            for (int i = 1; i < trajectoryPoints.Count; i++)
            {
                sum += trajectoryPoints[i - 1].DistanceTo(trajectoryPoints[i]);
            }
            return sum / (trajectoryPoints.Count - 1);
        }

        /// <summary>
        /// Read trajectory from file
        /// </summary>
        /// <param name="fileName">File full path</param>
        /// <returns>Info about reading succes</returns>
        private bool ReadTrajectoryFromFile(string fileName)
        {
            trajectoryPoints.Clear();
            try
            {
                using (StreamReader reader = new StreamReader(fileName))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var numbers = Regex.Split(line, " ");
                        var x = double.Parse(numbers[0], CultureInfo.InvariantCulture);
                        var y = double.Parse(numbers[1], CultureInfo.InvariantCulture);
                        trajectoryPoints.Add(new Point3D(x, y, ZValue));
                    }
                }
            }
            catch (Exception)
            {
                trajectoryPoints.Clear();
                TrajectoryEnabled = false;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Write trajectory to file
        /// </summary>
        /// <param name="fileName">File full path</param>
        /// <returns>Info about writing succes</returns>
        private bool WriteTrajectoryToFile(string fileName)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(fileName, false))
                {
                    foreach (var point in trajectoryPoints)
                    {
                        writer.WriteLine(point.X + " " + point.Y);
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Class constructor
        /// </summary>
        public TrajectoryController()
        {
            trajectoryPoints = new List<Point3D>();
            actualPointIndex = 1;
        }

        #region ITrajectoryController Interface
        public void Reset()
        {
            actualPointIndex = 1;
            TrajectoryAchieved = false;
        }

        public void Clear()
        {
            trajectoryPoints.Clear();
            actualPointIndex = 1;
            TrajectoryAchieved = false;
            TrajectoryEnabled = false;
        }

        public void SetAccuracy(AccuracyType accuracy)
        {
            switch (accuracy)
            {
                case AccuracyType.Ultra:
                    distanceEps = AverageDistance * 0.001;
                    break;
                case AccuracyType.High:
                    distanceEps = AverageDistance * 0.01;
                    break;
                case AccuracyType.Medium:
                    distanceEps = AverageDistance * 0.05;
                    break;
                case AccuracyType.Low:
                    distanceEps = AverageDistance * 0.25;
                    break;
                default:
                    break;
            }
        }

        public Point3DCollection LoadTrajectory(string fileName = null)
        {
            if (fileName == null)
            {
                var fileDialog = new OpenFileDialog
                {
                    Filter = @"Trajectory files (*.trj)|*.trj",
                    Multiselect = false
                };

                bool? userClickedOK = fileDialog.ShowDialog();

                if (userClickedOK == false) return null;

                fileName = fileDialog.FileName;
            }

            if (ReadTrajectoryFromFile(fileName) && trajectoryPoints.Count >= 2)
            {
                TrajectoryEnabled = true;
                AverageDistance = CalculateAverageDistance();
                SetAccuracy(DefaultAccuracyType);
                var result = new Point3DCollection();
                result.Add(trajectoryPoints[0]);
                for (int i = 1; i < trajectoryPoints.Count - 1; i++)
                {
                    result.Add(trajectoryPoints[i]);
                    result.Add(trajectoryPoints[i]);
                }
                result.Add(trajectoryPoints[trajectoryPoints.Count - 1]);
                return result;
            }
            MessageBox.Show("Trajectory cannot be loaded.");
            return null;
        }



        public string SaveTrajectory(List<Point3D> trajectory)
        {
            trajectoryPoints = trajectory;
            var fileDialog = new SaveFileDialog
            {
                FileName = "Trajectory.trj",
                Filter = @"Trajectory files (*.trj)|*.trj",
            };

            bool? userClickedOK = fileDialog.ShowDialog();

            if (userClickedOK == true)
            {
                if (!WriteTrajectoryToFile(fileDialog.FileName))
                {
                    MessageBox.Show("Trajectory cannot be saved.");
                    return null;
                }
                return fileDialog.FileName;
            }
            return null;
        }




        public Point3D GetTargetStartPosition()
        {
            return trajectoryPoints[0];
        }

        public Point3D GetTargetPosition(double x, double y, out bool nextCheckPoint)
        {
            nextCheckPoint = false;
            if (!TrajectoryEnabled)
                return new Point3D();
            if (TrajectoryAchieved)
                return trajectoryPoints[trajectoryPoints.Count - 1];

            if (trajectoryPoints[actualPointIndex].DistanceTo(new Point3D(x, y, ZValue)) < distanceEps)
            {
                actualPointIndex++;
                nextCheckPoint = true;
            }

            if (actualPointIndex >= trajectoryPoints.Count)
            {
                TrajectoryAchieved = true;
                nextCheckPoint = false;
                return trajectoryPoints[trajectoryPoints.Count - 1];
            }
            return trajectoryPoints[actualPointIndex];
        }

        #endregion

        /// <summary>
        /// Get target position as a smooth combination of control points [the function is deprecated]
        /// </summary>
        /// <param name="x">Cart position in X-Coordinate</param>
        /// <param name="y">Cart position in Y-Coordinate</param>
        /// <returns>Target position</returns>
        public Point3D GetTargetSmoothPosition(double x, double y)
        {
            if (!TrajectoryEnabled)
                return new Point3D();
            if (TrajectoryAchieved)
                return trajectoryPoints[trajectoryPoints.Count - 1];

            var distanceFrom = trajectoryPoints[actualPointIndex - 1].DistanceTo(new Point3D(x, y, ZValue));
            var distanceTo = trajectoryPoints[actualPointIndex].DistanceTo(new Point3D(x, y, ZValue));
            var distanceSum = distanceFrom + distanceTo;

            Point3D result;
            if (actualPointIndex + 1 >= trajectoryPoints.Count)
                result = trajectoryPoints[actualPointIndex];
            else
                result = new Point3D(
                    trajectoryPoints[actualPointIndex].X * (distanceTo / distanceSum)
                    + trajectoryPoints[actualPointIndex + 1].X * (distanceFrom / distanceSum),
                    trajectoryPoints[actualPointIndex].Y * (distanceTo / distanceSum)
                    + trajectoryPoints[actualPointIndex + 1].Y * (distanceFrom / distanceSum),
                    trajectoryPoints[actualPointIndex].Z);

            if (distanceTo < AverageDistance * ApproximateDistanceFactor)
                actualPointIndex++;
            if (actualPointIndex >= trajectoryPoints.Count)
            {
                TrajectoryAchieved = true;
                return trajectoryPoints[trajectoryPoints.Count - 1];
            }
            return result;
        }

        /// <summary>
        /// Get target position as a approximation between control points [the function is deprecated]
        /// </summary>
        /// <param name="x">Cart position in X-Coordinate</param>
        /// <param name="y">Cart position in Y-Coordinate</param>
        /// <returns>Target position</returns>
        public Point3D GetTargetApproximatePosition(double x, double y)
        {
            if (!TrajectoryEnabled)
                return new Point3D();
            if (TrajectoryAchieved)
                return trajectoryPoints[trajectoryPoints.Count - 1];
            for (int i = actualPointIndex + 1; i < trajectoryPoints.Count; i++)
            {
                if (trajectoryPoints[i].DistanceTo(new Point3D(x, y, ZValue)) < AverageDistance * ApproximateDistanceFactor)
                {
                    actualPointIndex = i;
                    break;
                }
            }
            if (actualPointIndex >= trajectoryPoints.Count - 1)
            {
                if (trajectoryPoints[actualPointIndex].DistanceTo(new Point3D(x, y, ZValue)) < distanceEps)
                {
                    TrajectoryAchieved = true;
                    return trajectoryPoints[trajectoryPoints.Count - 1];
                }
            }
            return trajectoryPoints[actualPointIndex];
        }
        #endregion
    }

    /// <summary>
    /// Enumeration for tracking accuracy types
    /// </summary>
    public enum AccuracyType
    {
        Ultra,
        High,
        Medium,
        Low
    }
}
