using HelixToolkit.Wpf;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using InvertedPendulumTransporterPhysics.Common;
using System;
using System.Windows.Media.Imaging;

namespace InvertedPendulumTransporter.Models
{
    public class Cart : ICart
    {
        public ModelVisual3D Model { get; private set; }
        public double PlatformSize { get; private set; }

        private const double DefaultPlatformSize = 5.0;

        public double platformHeightFactor = 0.1;
        public double wheelRadius = 0.5;
        private SphereVisual3D[] wheels;
        private Point3D lastPoint;
        private Vector3D offsetXRotationAxis;
        private Vector3D offsetYRotationAxis;
        private bool highGradeTexturesEnabled;
        private CubeVisual3D platform;
        private TubeVisual3D rim;

        public Cart()
        {
            Initialize();
        }

        private void Initialize()
        {
            wheels = new SphereVisual3D[4];
            lastPoint = new Point3D();
            offsetXRotationAxis = new Vector3D(0.0, 1.0, 0.0);
            offsetYRotationAxis = new Vector3D(-1.0, 0.0, 0.0);
            highGradeTexturesEnabled = false;

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

            //if (highGradeTexturesEnabled)
            //{
            //    foreach (var wheel in wheels)
            //    {
            //        var wheelTG = new Transform3DGroup();
            //        var offsetX = (systemState.StateX.Position - lastPoint.X) * 180 / (Math.PI * wheelRadius);
            //        var offsetY = (systemState.StateY.Position - lastPoint.Y) * 180 / (Math.PI * wheelRadius);
            //        wheelTG.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(offsetXRotationAxis, offsetX)));
            //        wheelTG.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(offsetYRotationAxis, offsetY)));
            //        wheelTG.Children.Add(wheel.Transform);
            //        wheel.Transform = wheelTG;
            //    }
            //}

            //lastPoint = new Point3D(systemState.StateX.Position, systemState.StateY.Position, 0.0);
        }

        public void SetupHighGradeTextures()
        {
            highGradeTexturesEnabled = true;
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

        public void SetupLowGradeTextures()
        {
            highGradeTexturesEnabled = false;
            foreach (var wheel in wheels)
            {
                wheel.Material = Materials.Blue;
            }
            platform.Fill = Brushes.Purple;
            rim.Visible = true;
        }
    }
}
