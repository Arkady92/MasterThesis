using System;
using System.Linq;
using System.Windows.Media.Media3D;
using InvertedPendulumTransporterPhysics.Common;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using HelixToolkit.Wpf;

namespace InvertedPendulumTransporter.Models
{
    /// <summary>
    /// Simulation area model class
    /// </summary>
    public class SimulationArea : ISimulationArea
    {
        #region Private Members
        private ModelVisual3D floor;
        private ModelVisual3D walls;
        private const double DefaultSimulationAreaSize = 150.0;
        #endregion

        #region Public Members
        #region ISimulationArea Interface
        public double Size { get; private set; }
        public ModelVisual3D Model { get; private set; }
        #endregion
        #endregion

        #region Private Methods

        #endregion

        #region Public Methods
        /// <summary>
        /// Class constructor
        /// </summary>
        public SimulationArea()
        {
            Initialize();
        }

        public void Initialize()
        {
            Size = DefaultSimulationAreaSize;
            Model = new ModelVisual3D();

            floor = new ModelVisual3D();
            floor.Children.Add(new GridLinesVisual3D() { Center = new Point3D(0.0, 0.0, 0.0), Width = Size, Length = Size, MinorDistance = 2, MajorDistance = 10, Thickness = 0.03, Fill = Brushes.Green });
            floor.Children.Add(new RectangleVisual3D() { Origin = new Point3D(0.0, 0.0, -0.01), Normal = new Vector3D(0, 0, 1), Width = Size, Length = Size, Fill = Brushes.Tomato });
            Model.Children.Add(floor);

            walls = new ModelVisual3D();
            var wallHeight = Size / 30.0;
            walls.Children.Add(new GridLinesVisual3D() { Center = new Point3D(0.0, Size / 2, wallHeight / 2.0), Normal = new Vector3D(0.0, 1.0, 0), Width = wallHeight, Length = Size, Fill = Brushes.Green });
            walls.Children.Add(new GridLinesVisual3D() { Center = new Point3D(0.0, -Size / 2, wallHeight / 2.0), Normal = new Vector3D(0.0, 1.0, 0), Width = wallHeight, Length = Size, Fill = Brushes.Green });
            walls.Children.Add(new GridLinesVisual3D() { Center = new Point3D(Size / 2, 0.0, wallHeight / 2.0), Normal = new Vector3D(1.0, 0.0, 0), Width = Size, Length = wallHeight, Fill = Brushes.Green });
            walls.Children.Add(new GridLinesVisual3D() { Center = new Point3D(-Size / 2, 0.0, wallHeight / 2.0), Normal = new Vector3D(1.0, 0.0, 0), Width = Size, Length = wallHeight, Fill = Brushes.Green });
            Model.Children.Add(walls);
        }

        public void UpdateState(SystemState systemState)
        {
        }

        public void SetupHighLevelGraphics()
        {
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/Resources/Floor.jpg"));
            floor.Children.OfType<RectangleVisual3D>().First().Fill = imageBrush;
            foreach (var wall in walls.Children.OfType<GridLinesVisual3D>())
                wall.Visible = false;
        }

        public void SetupLowLevelGraphics()
        {
            floor.Children.OfType<RectangleVisual3D>().First().Fill = Brushes.Tomato;
            foreach (var wall in walls.Children.OfType<GridLinesVisual3D>())
                wall.Visible = true;
        }
        #endregion
    }
}
