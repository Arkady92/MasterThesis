using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace InvertedPendulumTransporterPhysics.Controllers
{
    /// <summary>
    /// Interface dedicated to trajectory controller
    /// </summary>
    public interface ITrajectoryController : IController
    {
        /// <summary>
        /// Check if trajectory end was achieved
        /// </summary>
        bool TrajectoryAchieved { get; }
        
        /// <summary>
        /// Check if trajectory mode is enabled
        /// </summary>
        bool TrajectoryEnabled { get; }

        /// <summary>
        /// Average distance between control points
        /// </summary>
        double AverageDistance { get; }

        /// <summary>
        /// Clear trajectory
        /// </summary>
        void Clear();

        /// <summary>
        /// Set trajectory tracking accuracy
        /// </summary>
        /// <param name="accuracy">Accuracy type</param>
        void SetAccuracy(AccuracyType accuracy);

        /// <summary>
        /// Load trajectory from file
        /// </summary>
        /// <param name="fileName">File full path (if null additionaly open file browser)</param>
        /// <returns>Control points collection with doubled points for trajectory visualization</returns>
        Point3DCollection LoadTrajectory(string fileName = null);

        /// <summary>
        /// Save trajectory to file
        /// </summary>
        /// <param name="trajectory">List of control points</param>
        /// <returns>File full path</returns>
        string SaveTrajectory(List<Point3D> trajectory);

        /// <summary>
        /// Get trajectory beginning position
        /// </summary>
        /// <returns></returns>
        Point3D GetTargetStartPosition();

        /// <summary>
        /// Get actual target control point
        /// </summary>
        /// <param name="x">Cart position in X-coordinate</param>
        /// <param name="y">Cart position in Y-coordinate</param>
        /// <param name="nextCheckPoint">Info about target point change</param>
        /// <returns>Target control point</returns>
        Point3D GetTargetPosition(double x, double y, out bool nextCheckPoint);
    }
}
