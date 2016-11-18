using HelixToolkit.Wpf;
using InvertedPendulumTransporterPhysics.Common;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System;
using InvertedPendulumTransporter.Models;

namespace InvertedPendulumTransporter.Controls
{
    /// <summary>
    /// Interaction logic for Scene.xaml
    /// </summary>
    public partial class SceneControl : UserControl
    {
        private Point3DCollection cartTrajectoryPoints;
        private LinesVisual3D cartTrajectoryLines;
        private Point3DCollection pendulumTrajectoryPoints;
        private LinesVisual3D pendulumTrajectoryLines;
        private Point3DCollection targetTrajectoryPoints;
        private PointsVisual3D targetTrajectoryCheckPoints;
        private LinesVisual3D targetTrajectoryLines;

        public IPendulum pendulum;
        public ICart cart;
        public const double SimulationAreaSize = 150.0;
        private int frame;
        private const int FrameLineCutNumber = 200;
        public const int FrameLineOptimizeNumber = 3;
        private const double WindDirectionResizeFactor = 5.0;
        private const double CartTrajectoryHeight = 0.1;
        private Point3D sceneCenter;
        private bool renderTargetTrajectory;
        private bool renderPendulumTrajectory;
        private bool renderCartTrajectory;

        public SceneControl()
        {
            InitializeComponent();
            InitializeObjects();
            InitializeScene();
        }

        private void InitializeObjects()
        {
            cartTrajectoryPoints = new Point3DCollection();
            pendulumTrajectoryPoints = new Point3DCollection();
            targetTrajectoryPoints = new Point3DCollection();
            sceneCenter = new Point3D();
            renderTargetTrajectory = true;
            renderPendulumTrajectory = true;
            renderCartTrajectory = true;
        }

        private void InitializeScene()
        {
            WindDirectionScene.Camera = SimulationScene.Camera;

            cartTrajectoryLines = new LinesVisual3D();
            cartTrajectoryLines.Thickness = 3;
            cartTrajectoryLines.Color = Colors.Blue;
            cartTrajectoryLines.Points = cartTrajectoryPoints;
            SimulationScene.Children.Add(cartTrajectoryLines);

            pendulumTrajectoryLines = new LinesVisual3D();
            pendulumTrajectoryLines.Thickness = 3;
            pendulumTrajectoryLines.Color = Colors.Gold;
            pendulumTrajectoryLines.Points = pendulumTrajectoryPoints;
            SimulationScene.Children.Add(pendulumTrajectoryLines);

            targetTrajectoryLines = new LinesVisual3D();
            targetTrajectoryLines.Thickness = 3;
            targetTrajectoryLines.Color = Colors.Red;
            targetTrajectoryLines.Points = targetTrajectoryPoints;

            targetTrajectoryCheckPoints = new PointsVisual3D();
            targetTrajectoryCheckPoints.Color = Colors.Purple;
            targetTrajectoryCheckPoints.Size = 5;
            targetTrajectoryCheckPoints.Points = targetTrajectoryPoints;


            var floor = new ModelVisual3D();
            floor.Children.Add(new GridLinesVisual3D() { Center = new Point3D(0.0, 0.0, 0.0), Width = SimulationAreaSize, Length = SimulationAreaSize, MinorDistance = 2, MajorDistance = 10, Thickness = 0.03, Fill = Brushes.Green });
            floor.Children.Add(new RectangleVisual3D() { Origin = new Point3D(0.0, 0.0, -0.01), Normal = new Vector3D(0, 0, 1), Width = SimulationAreaSize, Length = SimulationAreaSize, Fill = Brushes.Tomato });
            SimulationScene.Children.Add(floor);

            var walls = new ModelVisual3D();
            var wallHeight = SimulationAreaSize / 30.0;
            walls.Children.Add(new GridLinesVisual3D() { Center = new Point3D(0.0, SimulationAreaSize / 2, wallHeight / 2.0), Normal = new Vector3D(0.0, 1.0, 0), Width = wallHeight, Length = SimulationAreaSize, Fill = Brushes.Green });
            walls.Children.Add(new GridLinesVisual3D() { Center = new Point3D(0.0, -SimulationAreaSize / 2, wallHeight / 2.0), Normal = new Vector3D(0.0, 1.0, 0), Width = wallHeight, Length = SimulationAreaSize, Fill = Brushes.Green });
            walls.Children.Add(new GridLinesVisual3D() { Center = new Point3D(SimulationAreaSize / 2, 0.0, wallHeight / 2.0), Normal = new Vector3D(1.0, 0.0, 0), Width = SimulationAreaSize, Length = wallHeight, Fill = Brushes.Green });
            walls.Children.Add(new GridLinesVisual3D() { Center = new Point3D(-SimulationAreaSize / 2, 0.0, wallHeight / 2.0), Normal = new Vector3D(1.0, 0.0, 0), Width = SimulationAreaSize, Length = wallHeight, Fill = Brushes.Green });
            SimulationScene.Children.Add(walls);

            cart = new Cart();
            SimulationScene.Children.Add(cart.Model);

            pendulum = new Pendulum();
            SimulationScene.Children.Add(pendulum.Model);
        }

        public void UpdateState(SystemState systemState)
        {
            cart.UpdateState(systemState);
            pendulum.UpdateState(systemState);
        }

        public void UpdateCamera(SystemState systemState)
        {
            sceneCenter = new Point3D(systemState.StateX.Position, systemState.StateY.Position, pendulum.RodLength / 2);
            SimulationScene.Camera.LookAt(sceneCenter, 0.0);
        }

        public void UpdateFrame(SystemState systemState)
        {
            frame++;
            UpdateState(systemState);
            UpdateCamera(systemState);

            // render pendulum trajectory
            if (renderPendulumTrajectory)
                SimulationScene.Children.Remove(pendulumTrajectoryLines);

            if (frame >= FrameLineCutNumber)
            {
                pendulumTrajectoryPoints.RemoveAt(0);
                pendulumTrajectoryPoints.RemoveAt(0);
            }
            pendulumTrajectoryPoints.Add(pendulum.MassLinkPoint);
            pendulumTrajectoryPoints.Add(pendulum.MassLinkPoint);

            // render cart trajectory
            if (renderCartTrajectory)
                SimulationScene.Children.Remove(cartTrajectoryLines);

            var cartEndPoint = new Point3D(systemState.StateX.Position, systemState.StateY.Position, CartTrajectoryHeight);
            if (frame >= FrameLineOptimizeNumber)
            {
                var A = cartTrajectoryPoints[cartTrajectoryPoints.Count - 1];
                var B = cartTrajectoryPoints[cartTrajectoryPoints.Count - 3];
                var C = cartEndPoint;
                var area = A.X * (B.Y - C.Y) + B.X * (C.Y - A.Y) + C.X * (A.Y - B.Y);
                if (Math.Abs(area) < double.Epsilon)
                {
                    cartTrajectoryPoints.RemoveAt(cartTrajectoryPoints.Count - 1);
                    cartTrajectoryPoints.RemoveAt(cartTrajectoryPoints.Count - 1);
                }
            }
            cartTrajectoryPoints.Add(cartEndPoint);
            cartTrajectoryPoints.Add(cartEndPoint);

            if (renderPendulumTrajectory)
                SimulationScene.Children.Add(pendulumTrajectoryLines);
            if (renderCartTrajectory)
                SimulationScene.Children.Add(cartTrajectoryLines);
        }

        public void ResetSimulation(SystemState systemState)
        {
            frame = 0;
            UpdateState(systemState);
            UpdateCamera(systemState);
            cartTrajectoryPoints.Clear();
            pendulumTrajectoryPoints.Clear();
            cartTrajectoryPoints.Add(new Point3D(systemState.StateX.Position, systemState.StateY.Position, CartTrajectoryHeight));
            pendulumTrajectoryPoints.Add(pendulum.MassLinkPoint);
            WindArrow.Visible = false;
        }

        public void UpdateWindDirection(Vector3D windDirection, double windPower)
        {
            if (Math.Abs(windPower) > double.Epsilon)
                WindArrow.Visible = true;
            else
                WindArrow.Visible = false;
            WindArrow.Point1 = (sceneCenter.ToVector3D() + windDirection * -WindDirectionResizeFactor).ToPoint3D();
            WindArrow.Point2 = (sceneCenter.ToVector3D() + windDirection * WindDirectionResizeFactor).ToPoint3D();
        }

        public void ClearTrajectory()
        {
            if (SimulationScene.Children.Contains(targetTrajectoryLines))
                SimulationScene.Children.Remove(targetTrajectoryLines);
            if (SimulationScene.Children.Contains(targetTrajectoryCheckPoints))
                SimulationScene.Children.Remove(targetTrajectoryCheckPoints);
        }

        public void UpdateTrajectory(Point3DCollection trajectory)
        {
            ClearTrajectory();
            targetTrajectoryPoints = trajectory;
            targetTrajectoryLines.Points = trajectory;
            targetTrajectoryCheckPoints.Points = trajectory;
            if (renderTargetTrajectory)
            {
                SimulationScene.Children.Add(targetTrajectoryLines);
                SimulationScene.Children.Add(targetTrajectoryCheckPoints);
            }
        }

        public void ShowTargetTrajectory(bool isChecked)
        {
            if (isChecked && !renderTargetTrajectory)
            {
                renderTargetTrajectory = true;
                SimulationScene.Children.Add(targetTrajectoryLines);
                SimulationScene.Children.Add(targetTrajectoryCheckPoints);
            }
            if (!isChecked && renderTargetTrajectory)
            {
                renderTargetTrajectory = false;
                SimulationScene.Children.Remove(targetTrajectoryLines);
                SimulationScene.Children.Remove(targetTrajectoryCheckPoints);
            }
        }

        public void ShowCartTrajectory(bool isChecked)
        {
            if (isChecked && !renderCartTrajectory)
            {
                renderCartTrajectory = true;
                SimulationScene.Children.Add(cartTrajectoryLines);
            }
            if (!isChecked && renderCartTrajectory)
            {
                renderCartTrajectory = false;
                SimulationScene.Children.Remove(cartTrajectoryLines);
            }
        }

        public void ShowPendulumTrajectory(bool isChecked)
        {
            if (isChecked && !renderPendulumTrajectory)
            {
                renderPendulumTrajectory = true;
                SimulationScene.Children.Add(pendulumTrajectoryLines);
            }
            if (!isChecked && renderPendulumTrajectory)
            {
                renderPendulumTrajectory = false;
                SimulationScene.Children.Remove(pendulumTrajectoryLines);
            }
        }
    }
}
