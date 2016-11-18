using System;
using System.Windows.Media.Media3D;

namespace InvertedPendulumTransporterPhysics.Controllers
{
    public class WindController : IWindController
    {
        public WindType WindType { get; set; }
        public double WindPower { get; set; }
        public double WindChangeSpeed { get; set; }
        public double MaxWindPower { get { return 1.0; } }
        public double MinWindPower { get { return 0.0; } }
        public double DefaultWindPower { get { return 0.0; } }

        public const double DefaultWindChangeSpeed = 0.5;
        public const WindType DefaultWindType = WindType.RandomSmooth;
        private Vector3D windDirection;
        private int tickCounter = 0;
        private const int peakResetValue = 500;
        private const int switchResetValue = 500;
        private const int smoothResetValue = 500;
        private Random random;
        private Vector3D startWindDirection;
        private Vector3D endWindDirection;
        private double actualWindChangeSpeed;

        public WindController()
        {
            random = new Random(Guid.NewGuid().GetHashCode());
            WindType = DefaultWindType;
            WindPower = DefaultWindPower;
            WindChangeSpeed = DefaultWindChangeSpeed;
            actualWindChangeSpeed = DefaultWindChangeSpeed;
            windDirection = GeneratePeakWindDirection();
            startWindDirection = windDirection;
            endWindDirection = GeneratePeakWindDirection();
        }

        public double GetXCoordWindPower()
        {
            return windDirection.X * WindPower;
        }

        public double GetYCoordWindPower()
        {
            return windDirection.Y * WindPower;
        }

        public double GetZCoordWindPower()
        {
            return windDirection.Z * WindPower;
        }

        private Vector3D GeneratePeakWindDirection()
        {
            var result = new Vector3D(random.NextDouble() * 2 - 1, random.NextDouble() * 2 - 1, random.NextDouble() * 2 - 1);
            return Normalize(result);
        }

        private Vector3D GenerateSwitchWindDirection()
        {
            var result = new Vector3D(-Math.Sign(windDirection.X) * random.NextDouble(), 
                -Math.Sign(windDirection.X) * random.NextDouble(), -Math.Sign(windDirection.X) * random.NextDouble());
            return Normalize(result);
            
        }

        private Vector3D Normalize(Vector3D vector)
        {
            return vector * (1 / Math.Sqrt(vector.LengthSquared));
        }

        public Vector3D UpdateWindForce()
        {
            switch (WindType)
            {
                case WindType.RandomPeak:
                    tickCounter++;
                    if(tickCounter >= peakResetValue * (1.1 - WindChangeSpeed))
                    {
                        tickCounter = 0;
                        windDirection = GeneratePeakWindDirection();
                    }
                    break;
                case WindType.RandomSwitch:
                    tickCounter++;
                    if (tickCounter >= switchResetValue * (1.1 - WindChangeSpeed))
                    {
                        tickCounter = 0;
                        windDirection = GenerateSwitchWindDirection();
                    }
                    break;
                case WindType.RandomSmooth:
                    tickCounter++;
                    if (Math.Abs(WindChangeSpeed) < double.Epsilon)
                        return windDirection;
                    if(Math.Abs(actualWindChangeSpeed - WindChangeSpeed) > double.Epsilon)
                    {
                        actualWindChangeSpeed = WindChangeSpeed;
                        tickCounter = (int)(tickCounter * (1.1 - actualWindChangeSpeed));
                    }
                    var resetValue = (int)(smoothResetValue * (1.1 - actualWindChangeSpeed));
                    if (tickCounter >= resetValue)
                    {
                        tickCounter = 0;
                        startWindDirection = endWindDirection;
                        endWindDirection = GenerateSwitchWindDirection();
                    }
                    windDirection = startWindDirection * (resetValue - tickCounter) / resetValue 
                        + endWindDirection * tickCounter / resetValue;
                    windDirection = Normalize(windDirection);
                    break;
                default:
                    break;
            }
            return windDirection;
        }

        public void Reset()
        {
            tickCounter = 0;
        }
    }

    public enum WindType
    {
        RandomPeak,
        RandomSwitch,
        RandomSmooth
    }
}
