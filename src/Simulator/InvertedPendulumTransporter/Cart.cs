using HelixToolkit.Wpf;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace InvertedPendulumTransporter
{
    public class Cart
    {
        public ModelVisual3D cartModel;
        public double platformSize = 5.0;
        public double platformHeightFactor = 0.1;
        public double wheelRadius = 0.5;
        
        public Cart()
        {
            cartModel = new ModelVisual3D();
            var platform = new CubeVisual3D();
            platform.Fill = Brushes.Purple;
            platform.SideLength = platformSize;
            var platformTransformGroup = new Transform3DGroup();
            platformTransformGroup.Children.Add(new ScaleTransform3D(new Vector3D(1.0, 1.0, platformHeightFactor)));
            platformTransformGroup.Children.Add(new TranslateTransform3D(0.0, 0.0, 
                wheelRadius * 2  + platformSize * platformHeightFactor / 2));
            platform.Transform = platformTransformGroup;
            cartModel.Children.Add(platform);
            for (int i = 0; i < 4; i++)
            {
                var wheel = new SphereVisual3D();
                wheel.Radius = wheelRadius;
                var wheelPosition = platformSize / 2 - wheelRadius;
                var wheelTransformGroup = new Transform3DGroup();
                wheelTransformGroup.Children.Add(new TranslateTransform3D(-wheelPosition * (i % 2 * 2 - 1),
                            wheelPosition * (i % 2 * 2 - 1) * (i >= 2 ? 1.0 : -1.0), 0.5));
                wheel.Transform = wheelTransformGroup;
                cartModel.Children.Add(wheel);
            }
        }

        public void UpdateState(SystemState systemState)
        {
            var cartTG = new Transform3DGroup();
            cartTG.Children.Add(new TranslateTransform3D(systemState.StateX.Position, systemState.StateY.Position, 0.0));
            cartModel.Transform = cartTG;
        }
    }
}
