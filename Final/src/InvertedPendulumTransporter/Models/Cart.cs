using HelixToolkit.Wpf;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using InvertedPendulumTransporterPhysics.Common;
using System;
using System.Windows.Media.Imaging;

namespace InvertedPendulumTransporter.Models
{
    /// <summary>
    /// Cart model class
    /// </summary>
    public class Cart : ICart
    {
        #region Private Members
        private SphereVisual3D[] wheels;
        private Vector3D offsetXRotationAxis;
        private Vector3D offsetYRotationAxis;
        private CubeVisual3D platform;
        private TubeVisual3D rim;
        private const double DefaultPlatformSize = 5.0;
        #endregion

        #region Public Members
        #region ICart Interface
        public ModelVisual3D Model { get; private set; }
        public double PlatformSize { get; private set; }
        #endregion

        /// <summary>
        /// Factor between walls height and platform size
        /// </summary>
        public double platformHeightFactor = 0.1;

        /// <summary>
        /// Wheel radius
        /// </summary>
        public double wheelRadius = 0.5;
        #endregion

        #region Private Methods

        #endregion

        #region Public Methods

        /// <summary>
        /// Class constructor
        /// </summary>
        public Cart()
        {
            Initialize();
        }

        #region ICart Interface
        public void Initialize()
        {
            wheels = new SphereVisual3D[4];
            offsetXRotationAxis = new Vector3D(0.0, 1.0, 0.0);
            offsetYRotationAxis = new Vector3D(-1.0, 0.0, 0.0);

            PlatformSize = DefaultPlatformSize;
            Model = new ModelVisual3D();
            platform = new CubeVisual3D();
            platform.Fill = Brushes.Purple;
            platform.SideLength = PlatformSize;
            var platformTransformGroup = new Transform3DGroup();
            platformTransformGroup.Children.Add(new ScaleTransform3D(new Vector3D(1.0, 1.0, platformHeightFactor)));
            platformTransformGroup.Children.Add(new TranslateTransform3D(0.0, 0.0,
                wheelRadius * 2 + PlatformSize * platformHeightFactor / 2));
            platform.Transform = platformTransformGroup;
            Model.Children.Add(platform);
            for (int i = 0; i < 4; i++)
            {
                var wheel = new SphereVisual3D();
                wheel.Radius = wheelRadius;
                wheel.Material = Materials.Blue;
                var wheelPosition = PlatformSize / 2 - wheelRadius;
                var wheelTransformGroup = new Transform3DGroup();
                wheelTransformGroup.Children.Add(new TranslateTransform3D(-wheelPosition * (i % 2 * 2 - 1),
                            wheelPosition * (i % 2 * 2 - 1) * (i >= 2 ? 1.0 : -1.0), 0.5));
                wheel.Transform = wheelTransformGroup;
                Model.Children.Add(wheel);
                wheels[i] = wheel;
            }
            rim = new TubeVisual3D();
            rim.Fill = Brushes.White;
            rim.Diameters = new DoubleCollection { 1.5, 0.0 };
            rim.Path = new Point3DCollection { new Point3D(0, 0, 1.5), new Point3D(0, 0, 1.75) };
            Model.Children.Add(rim);

        }

        public void UpdateState(SystemState systemState)
        {
            var cartTG = new Transform3DGroup();
            cartTG.Children.Add(new TranslateTransform3D(systemState.StateX.Position, systemState.StateY.Position, 0.0));
            Model.Transform = cartTG;
        }

        public void SetupHighLevelGraphics()
        {
            ImageBrush wheelImageBrush = new ImageBrush();
            wheelImageBrush.ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/Resources/Tire.jpg"));
            foreach (var wheel in wheels)
            {
                wheel.Material = null;
                wheel.Fill = wheelImageBrush;
            }
            ImageBrush platformImageBrush = new ImageBrush();
            platformImageBrush.ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/Resources/Platform.jpg"));
            platform.Fill = platformImageBrush;
            rim.Visible = false;
        }

        public void SetupLowLevelGraphics()
        {
            foreach (var wheel in wheels)
            {
                wheel.Material = Materials.Blue;
            }
            platform.Fill = Brushes.Purple;
            rim.Visible = true;
        }
        #endregion
        #endregion
    }
}
