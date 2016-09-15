using HelixToolkit.Wpf;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System;

namespace InvertedPendulumTransporter
{
    public class Pendulum
    {
        public ModelVisual3D pendulumModel;
        public SphereVisual3D massModel;
        public PipeVisual3D rodModel;
        public double rodDiameter = 0.1;
        public double rodLength = 6.1;
        public double rodLengthFactor = 10.0;
        public double platformHeight = 1.5;
        public Point3D cartLinkPoint;
        public Point3D massLinkPoint;

        public Pendulum()
        {
            cartLinkPoint = new Point3D(0.0, 0.0, platformHeight);
            massLinkPoint = new Point3D(0.0, 0.0, platformHeight + rodLength);

            pendulumModel = new ModelVisual3D();

            massModel = new SphereVisual3D();
            massModel.Radius = 0.5;
            massModel.Fill = Brushes.Gold;
            massModel.Center = massLinkPoint;
            pendulumModel.Children.Add(massModel);

            rodModel = new PipeVisual3D();
            rodModel.Diameter = rodDiameter;
            rodModel.Fill = Brushes.Gold;
            rodModel.Point1 = cartLinkPoint;
            rodModel.Point2 = massLinkPoint;
            pendulumModel.Children.Add(rodModel);
        }

        public void UpdateState(SystemState systemState, double? length = null)
        {
            if (length.HasValue)
                rodLength = length.Value * rodLengthFactor;
            var x = systemState.StateX.Position;
            var y = systemState.StateY.Position;
            var z = cartLinkPoint.Z;
            var alpha = systemState.StateX.Angle;
            var beta = systemState.StateY.Angle;

            cartLinkPoint = new Point3D(x, y, z);

            if (Math.Abs(beta) >= Math.PI / 2)
            {
                massLinkPoint = new Point3D(
                    x + rodLength * Math.Cos(alpha),
                    y + rodLength * Math.Sin(alpha),
                    z);
            }
            else if (Math.Abs(alpha) >= Math.PI / 2)
            {
                massLinkPoint = new Point3D(
                    x + rodLength * Math.Cos(beta),
                    y + rodLength * Math.Sin(beta),
                    z);
            }
            else
            {
                var alphaFactor = 1 / (Math.Cos(alpha) * Math.Cos(alpha));
                var betaFactor = 1 / (Math.Cos(beta) * Math.Cos(beta));
                var dz = rodLength / (Math.Sqrt(alphaFactor + betaFactor - 1));
                var dx = dz * Math.Sqrt(alphaFactor - 1) * Math.Sign(alpha);
                var dy = dz * Math.Sqrt(betaFactor - 1) * Math.Sign(beta);

                massLinkPoint = new Point3D(
                    x + dx,
                    y + dy,
                    z + dz);

            }
            rodModel.Point1 = cartLinkPoint;
            rodModel.Point2 = massLinkPoint;
            massModel.Center = massLinkPoint;
        }
    }
}
