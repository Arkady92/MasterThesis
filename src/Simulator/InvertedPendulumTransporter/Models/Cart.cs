using HelixToolkit.Wpf;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using InvertedPendulumTransporterPhysics.Common;

namespace InvertedPendulumTransporter.Models
{
    public class Cart : ICart
    {
        public ModelVisual3D Model { get; private set; }
        public double PlatformSize { get; private set; }

        private const double DefaultPlatformSize = 5.0;

        public double platformHeightFactor = 0.1;
        public double wheelRadius = 0.5;
        
        public Cart()
        {
            PlatformSize = DefaultPlatformSize;
            Model = new ModelVisual3D();
            var platform = new CubeVisual3D();
            platform.Fill = Brushes.Purple;
            platform.SideLength = PlatformSize;
            var platformTransformGroup = new Transform3DGroup();
            platformTransformGroup.Children.Add(new ScaleTransform3D(new Vector3D(1.0, 1.0, platformHeightFactor)));
            platformTransformGroup.Children.Add(new TranslateTransform3D(0.0, 0.0, 
                wheelRadius * 2  + PlatformSize * platformHeightFactor / 2));
            platform.Transform = platformTransformGroup;
            Model.Children.Add(platform);
            for (int i = 0; i < 4; i++)
            {
                var wheel = new SphereVisual3D();
                wheel.Radius = wheelRadius;
                var wheelPosition = PlatformSize / 2 - wheelRadius;
                var wheelTransformGroup = new Transform3DGroup();
                wheelTransformGroup.Children.Add(new TranslateTransform3D(-wheelPosition * (i % 2 * 2 - 1),
                            wheelPosition * (i % 2 * 2 - 1) * (i >= 2 ? 1.0 : -1.0), 0.5));
                wheel.Transform = wheelTransformGroup;
                Model.Children.Add(wheel);
            }
            var rim = new TubeVisual3D();
            rim.Fill = Brushes.White;
            rim.Diameters = new DoubleCollection { 1.5, 0.0 };
            rim.Path = new Point3DCollection{ new Point3D(0, 0, 1.5), new Point3D(0, 0, 1.75) };
            Model.Children.Add(rim);
        }

        public void UpdateState(SystemState systemState)
        {
            var cartTG = new Transform3DGroup();
            cartTG.Children.Add(new TranslateTransform3D(systemState.StateX.Position, systemState.StateY.Position, 0.0));
            Model.Transform = cartTG;
        }
    }
}
