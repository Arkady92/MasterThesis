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

            if (Math.Abs(beta) < Double.Epsilon)
            {
                massLinkPoint = new Point3D(
                    x + rodLength * Math.Sin(alpha),
                    y,
                    z + rodLength * Math.Cos(alpha));
            }
            else if (Math.Abs(alpha) < Double.Epsilon)
            {
                massLinkPoint = new Point3D(
                    x,
                    y + rodLength * Math.Sin(beta),
                    z + rodLength * Math.Cos(beta));
            }
            else
            {
                //TODO

                //massLinkPoint = new Point3D(
                //    x + rodLength * Math.Sin(alpha) * Math.Cos(beta),
                //    y + rodLength * Math.Sin(alpha) * Math.Sin(beta),
                //    z + rodLength * Math.Cos(alpha));

                //massLinkPoint = new Point3D(
                //    x + rodLength * Math.Sin(alpha),
                //    y + rodLength * Math.Cos(alpha) * Math.Sin(beta),
                //    z + rodLength * Math.Cos(alpha) * Math.Cos(beta));

                //var r = rodLength;
                //var dx = r * Math.Sin(alpha);
                //var dy = r * Math.Sin(beta);
                //var dxy = Math.Sqrt(dx * dx + dy * dy);
                //var dxyr = Math.Sqrt(r * r + dxy * dxy);
                //// dxyNew : r <-> dxy : dxyr
                //var dxyNew = r * dxy / dxyr;
                //// dzNew : dxyNew <-> r : dxy
                //var dzNew = dxyNew * r / dxy;
                //var dxNew = dxyNew * dx / dxy;
                //var dyNew = dxyNew * dy / dxy;

                var dx = rodLength * Math.Sin(alpha);
                var dy = rodLength * Math.Sin(beta);
                var calc = Math.Cos(alpha) * Math.Cos(alpha) - Math.Sin(beta) * Math.Sin(beta);
                var dz = calc > 0.0 ? rodLength * Math.Sqrt(calc) : 0.0;

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
