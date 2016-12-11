using HelixToolkit.Wpf;
using InvertedPendulumTransporterPhysics.Common;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System;
using InvertedPendulumTransporter.Models;
using System.Collections.Generic;

namespace InvertedPendulumTransporter.Controls
{
    /// <summary>
    /// Interaction logic for Scene.xaml
    /// </summary>
    public partial class SceneControl : UserControl
    {
        #region Private Members
        private Point3DCollection cartTrajectoryPoints;
        private LinesVisual3D cartTrajectoryLines;
        private Point3DCollection pendulumTrajectoryPoints;
        private LinesVisual3D pendulumTrajectoryLines;
        private Point3DCollection targetTrajectoryPoints;
        private PointsVisual3D targetTrajectoryCheckPoints;
        private LinesVisual3D targetTrajectoryLines;
        private List<IModel> models;
        private Point3D sceneCenter;
        private bool renderTargetTrajectory;
        private bool renderPendulumTrajectory;
        private bool renderCartTrajectory;
        private int frame;
        private const int FrameLineCutNumber = 200;
        private const double WindDirectionResizeFactor = 5.0;
        private const double CartTrajectoryHeight = 0.1;
        private const int FrameLineOptimizeNumber = 3;
        #endregion

        #region Public Members
        /// <summary>
        /// Reference to pendulum model
        /// </summary>
        public IPendulum pendulum;

        /// <summary>
        /// Reference to cart model
        /// </summary>
        public ICart cart;

        /// <summary>
        /// Reference to simulation area model
        /// </summary>
        public ISimulationArea simulationArea;
        #endregion

        #region Private Methods

        /// <summary>
        /// Initialize all objects
        /// </summary>
        private void InitializeObjects()
        {
            cartTrajectoryPoints = new Point3DCollection();
            pendulumTrajectoryPoints = new Point3DCollection();
            targetTrajectoryPoints = new Point3DCollection();
            sceneCenter = new Point3D();
            renderTargetTrajectory = true;
            renderPendulumTrajectory = true;
            renderCartTrajectory = true;
            models = new List<IModel>();
        }

        /// <summary>
        /// Create base scene
        /// </summary>
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

            simulationArea = new SimulationArea();
            SimulationScene.Children.Add(simulationArea.Model);
            models.Add(simulationArea);

            cart = new Cart();
            SimulationScene.Children.Add(cart.Model);
            models.Add(cart);

            pendulum = new Pendulum();
            SimulationScene.Children.Add(pendulum.Model);
            models.Add(pendulum);
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Class constructor
        /// </summary>
        public SceneControl()
        {
            InitializeComponent();
            InitializeObjects();
            InitializeScene();
        }

        /// <summary>
        /// Setup high level graphics
        /// </summary>
        public void SetupHighLevelGraphics()
        {
            foreach (var model in models)
                model.SetupHighLevelGraphics();
        }

        /// <summary>
        /// Setup low level graphics
        /// </summary>
        public void SetupLowLevelGraphics()
        {
            foreach (var model in models)
                model.SetupLowLevelGraphics();
        }

        /// <summary>
        /// Update scene models
        /// </summary>
        /// <param name="systemState"></param>
        public void UpdateState(SystemState systemState)
        {
            foreach (var model in models)
                model.UpdateState(systemState);
        }

        /// <summary>
        /// Update camera object
        /// </summary>
        /// <param name="systemState">Actual system state</param>
        public void UpdateCamera(SystemState systemState)
        {
            sceneCenter = new Point3D(systemState.StateX.Position, systemState.StateY.Position, pendulum.RodLength / 2);
            SimulationScene.Camera.LookAt(sceneCenter, 0.0);
        }

        /// <summary>
        /// Update rendering frame
        /// </summary>
        /// <param name="systemState">Actual system staate</param>
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

        /// <summary>
        /// Reset scene to start configuration
        /// </summary>
        /// <param name="systemState">Actual system state</param>
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

        /// <summary>
        /// Update wind direction visual
        /// </summary>
        /// <param name="windDirection">Wind direction vector</param>
        /// <param name="windPower">Wind power value</param>
        public void UpdateWindDirection(Vector3D windDirection, double windPower)
        {
            if (Math.Abs(windPower) > double.Epsilon)
                WindArrow.Visible = true;
            else
                WindArrow.Visible = false;
            WindArrow.Point1 = (sceneCenter.ToVector3D() + windDirection * -WindDirectionResizeFactor).ToPoint3D();
            WindArrow.Point2 = (sceneCenter.ToVector3D() + windDirection * WindDirectionResizeFactor).ToPoint3D();
        }

        /// <summary>
        /// Clear trajectories
        /// </summary>
        public void ClearTrajectory()
        {
            if (SimulationScene.Children.Contains(targetTrajectoryLines))
                SimulationScene.Children.Remove(targetTrajectoryLines);
            if (SimulationScene.Children.Contains(targetTrajectoryCheckPoints))
                SimulationScene.Children.Remove(targetTrajectoryCheckPoints);
        }

        /// <summary>
        /// Update target trajectory
        /// </summary>
        /// <param name="trajectory">Trajectory</param>
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

        /// <summary>
        /// Change target trajectory visibility
        /// </summary>
        /// <param name="isChecked">Is trajectory visible</param>
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

        /// <summary>
        /// Change cart trajectory visibility
        /// </summary>
        /// <param name="isChecked">Is trajectory visible</param>
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

        /// <summary>
        /// Change pendulum trajectory visibility
        /// </summary>
        /// <param name="isChecked">Is trajectory visible</param>
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
        #endregion
    }
}
