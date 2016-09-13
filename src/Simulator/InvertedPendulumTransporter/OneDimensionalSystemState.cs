namespace InvertedPendulumTransporter
{
    public class OneDimensionalSystemState
    {
        public OneDimensionalSystemState()
        {
            InitializeDefault();
        }

        public OneDimensionalSystemState(double position, double angle, double velocity, double angularVelocity)
        {
            Position = position;
            Angle = angle;
            Velocity = velocity;
            AngularVelocity = angularVelocity;
        }

        public void InitializeDefault()
        {
            Position = 0.0;
            Angle = 0.0;
            Velocity = 0.0;
            AngularVelocity = 0.0;
        }

        public double Position { get; set; }
        public double Angle { get; set; }
        public double Velocity { get; set; }
        public double AngularVelocity { get; set; }

        public double[] ToStateArray()
        {
            return new double[] { Position, Angle, Velocity, AngularVelocity };
        }
    }
}
