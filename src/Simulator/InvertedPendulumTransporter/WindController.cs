using System;
using System.Windows.Media.Media3D;

namespace InvertedPendulumTransporter
{
    public class WindController
    {
        public WindType WindType { get; set; }
        public double WindPower { get; set; }
        public Vector3D WindDirection { get; private set; }

        private const double DefaultWindPower = 0.0;
        private const WindType DefaultWindType = WindType.RandomPeak;
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
            WindDirection = GeneratePeakWindDirection();
            startWindDirection = WindDirection;
            endWindDirection = GeneratePeakWindDirection();
        }

        public double GetHorizontalXWindPower()
        {
            return WindDirection.X * WindPower;
        }

        public double GetHorizontalYWindPower()
        {
            return WindDirection.Y * WindPower;
        }

        public double GetVerticalWindPower()
        {
            return WindDirection.Z * WindPower;
        }

        public Vector3D GeneratePeakWindDirection()
        {
            var result = new Vector3D(random.NextDouble() * 2 - 1, random.NextDouble() * 2 - 1, random.NextDouble() * 2 - 1);
            return Normalize(result);
        }

        private Vector3D GenerateSwitchWindDirection()
        {
            var result = new Vector3D(-Math.Sign(WindDirection.X) * random.NextDouble(), 
                -Math.Sign(WindDirection.X) * random.NextDouble(), -Math.Sign(WindDirection.X) * random.NextDouble());
            return Normalize(result);
            
        }

        private Vector3D Normalize(Vector3D vector)
        {
            return vector * (1 / Math.Sqrt(vector.LengthSquared));
        }

        public void UpdateWindForce()
        {
            switch (WindType)
            {
                case WindType.RandomPeak:
                    tickCounter++;
                    if(tickCounter >= peakResetValue)
                    {
                        tickCounter = 0;
                        WindDirection = GeneratePeakWindDirection();
                    }
                    break;
                case WindType.RandomSwitch:
                    tickCounter++;
                    if (tickCounter >= switchResetValue)
                    {
                        tickCounter = 0;
                        WindDirection = GenerateSwitchWindDirection();
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
                    WindDirection = startWindDirection * (smoothResetValue - tickCounter) / 100.0 + endWindDirection * tickCounter / 100.0;
                    WindDirection = Normalize(WindDirection);
                    break;
                default:
                    break;
            }
        }

        public void Reset()
        {
            tickCounter = 0;
            WindType = DefaultWindType;
        }
    }

    public enum WindType
    {
        RandomPeak,
        RandomSwitch,
        RandomSmooth
    }
}
