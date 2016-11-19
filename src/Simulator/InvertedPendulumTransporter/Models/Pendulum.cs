using HelixToolkit.Wpf;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System;
using InvertedPendulumTransporterPhysics.Common;
using System.Windows.Media.Imaging;

namespace InvertedPendulumTransporter.Models
{
    public class Pendulum : IPendulum
    {
        public ModelVisual3D Model { get; private set; }
        public Point3D MassLinkPoint { get; private set; }
        public Point3D CartLinkPoint { get; private set; }
        public double RodLength { get; private set; }

        private SphereVisual3D massModel;
        private PipeVisual3D rodModel;
        private const double defaultRodLength = 6.1;
        private double rodDiameter = 0.1;
        private double rodLengthFactor = 10.0;
        private double platformHeight = 1.5;

        public Pendulum()
        {
            RodLength = defaultRodLength;
            CartLinkPoint = new Point3D(0.0, 0.0, platformHeight);
            MassLinkPoint = new Point3D(0.0, 0.0, platformHeight + RodLength);

            Model = new ModelVisual3D();

            massModel = new SphereVisual3D();
            massModel.Radius = 0.5;
            massModel.Fill = Brushes.Gold;
            massModel.Center = MassLinkPoint;
            Model.Children.Add(massModel);

            rodModel = new PipeVisual3D();
            rodModel.Diameter = rodDiameter;
            rodModel.Fill = Brushes.Gold;
            rodModel.Point1 = CartLinkPoint;
            rodModel.Point2 = MassLinkPoint;
            Model.Children.Add(rodModel);
        }

        public void UpdateState(SystemState systemState)
        {
            RodLength = systemState.SolverParameters.PendulumLength * rodLengthFactor;
            var x = systemState.StateX.Position;
            var y = systemState.StateY.Position;
            var z = CartLinkPoint.Z;
            var alpha = systemState.StateX.Angle;
            var beta = systemState.StateY.Angle;

            CartLinkPoint = new Point3D(x, y, z);

            if (Math.Abs(beta) >= Math.PI / 2)
            {
                MassLinkPoint = new Point3D(
                    x + RodLength * Math.Cos(alpha),
                    y + RodLength * Math.Sin(alpha),
                    z);
            }
            else if (Math.Abs(alpha) >= Math.PI / 2)
            {
                MassLinkPoint = new Point3D(
                    x + RodLength * Math.Cos(beta),
                    y + RodLength * Math.Sin(beta),
                    z);
            }
            else
            {
                var alphaFactor = 1 / (Math.Cos(alpha) * Math.Cos(alpha));
                var betaFactor = 1 / (Math.Cos(beta) * Math.Cos(beta));
                var dz = RodLength / (Math.Sqrt(alphaFactor + betaFactor - 1));
                var dx = dz * Math.Sqrt(alphaFactor - 1) * Math.Sign(alpha);
                var dy = dz * Math.Sqrt(betaFactor - 1) * Math.Sign(beta);

                MassLinkPoint = new Point3D(
                    x + dx,
                    y + dy,
                    z + dz);

            }
            rodModel.Point1 = CartLinkPoint;
            rodModel.Point2 = MassLinkPoint;
            massModel.Center = MassLinkPoint;
        }

        public void SetupHighGradeTextures()
        {
            ImageBrush pendulumImageBrush = new ImageBrush();
            pendulumImageBrush.ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/Resources/Pendulum.jpg"));
            massModel.Fill = pendulumImageBrush;
            rodModel.Fill = pendulumImageBrush;
        }

        public void SetupLowGradeTextures()
        {
            massModel.Fill = Brushes.Gold;
            rodModel.Fill = Brushes.Gold;
        }
    }
}
