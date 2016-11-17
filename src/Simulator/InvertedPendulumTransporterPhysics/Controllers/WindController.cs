using System;
using System.Windows.Media.Media3D;

namespace InvertedPendulumTransporterPhysics.Controllers
{
    public class WindController
    {
        public WindType WindType { get; set; }
        public double WindPower { get; set; }
        public double MaxWindPower { get { return 10.0; } }
        public double MinWindPower { get { return 0.0; } }
        public double DefaultWindPower { get { return 0.0; } }
        public const WindType DefaultWindType = WindType.RandomSmooth;

        private Vector3D windDirection;
        private int tickCounter = 0;
        private const int peakResetValue = 10;
        private const int switchResetValue = 100;
        private const int smoothResetValue = 100;
        private Random random;
        private Vector3D startWindDirection;
        private Vector3D endWindDirection;

        public WindController()
        {
            random = new Random(Guid.NewGuid().GetHashCode());
            WindType = DefaultWindType;
            WindPower = DefaultWindPower;
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

        public Vector3D GeneratePeakWindDirection()
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
                    if(tickCounter >= peakResetValue)
                    {
                        tickCounter = 0;
                        windDirection = GeneratePeakWindDirection();
                    }
                    break;
                case WindType.RandomSwitch:
                    tickCounter++;
                    if (tickCounter >= switchResetValue)
                    {
                        tickCounter = 0;
                        windDirection = GenerateSwitchWindDirection();
                    }
                    break;
                case WindType.RandomSmooth:
                    tickCounter++;
                    if (tickCounter >= smoothResetValue)
                    {
                        tickCounter = 0;
                        startWindDirection = endWindDirection;
                        endWindDirection = GenerateSwitchWindDirection();
                    }
                    windDirection = startWindDirection * (smoothResetValue - tickCounter) / 100.0 + endWindDirection * tickCounter / 100.0;
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
