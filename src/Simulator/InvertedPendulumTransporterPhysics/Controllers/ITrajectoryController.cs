using System.Windows.Media.Media3D;

namespace InvertedPendulumTransporterPhysics.Controllers
{
    public interface ITrajectoryController : IController
    {
        bool TrajectoryAchieved { get; }
        bool TrajectoryEnabled { get; }
        bool TrajectoryAccuracy { get; }

        void Clear();
        void SetAccuracy(AccuracyType accuracy);
        Point3DCollection LoadTrajectory();
        void SaveTrajectory();
        Point3D GetTargetStartPosition();
        Point3D GetTargetAccuratePosition(double x, double y, out bool nextCheckPoint);
    }
}
