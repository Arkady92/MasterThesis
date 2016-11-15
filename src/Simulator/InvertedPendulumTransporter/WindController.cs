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
        private Random random;

        public WindController()
        {
            random = new Random(Guid.NewGuid().GetHashCode());
            WindType = DefaultWindType;
            WindPower = DefaultWindPower;
            GeneratePeakWindDirection();
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

        public void GeneratePeakWindDirection()
        {
            WindDirection = new Vector3D(random.NextDouble() * 2 - 1, random.NextDouble() * 2 - 1, random.NextDouble() * 2 - 1);
            WindDirection = WindDirection * (1 / Math.Sqrt(WindDirection.LengthSquared));
        }

        private void GenerateSwitchWindDirection()
        {
            WindDirection = new Vector3D(-Math.Sign(WindDirection.X) * random.NextDouble(), 
                -Math.Sign(WindDirection.X) * random.NextDouble(), -Math.Sign(WindDirection.X) * random.NextDouble());
            WindDirection = WindDirection * (1 / Math.Sqrt(WindDirection.LengthSquared));
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
                        GeneratePeakWindDirection();
                    }
                    break;
                case WindType.RandomSwitch:
                    tickCounter++;
                    if (tickCounter >= switchResetValue)
                    {
                        tickCounter = 0;
                        GenerateSwitchWindDirection();
                    }
                    break;
                case WindType.RandomSmooth:
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
