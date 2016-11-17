namespace InvertedPendulumTransporterPhysics.Common
{
    public class SolverParameters
    {
        public double PendulumMass { get; set; }
        public double CartMass { get; set; }
        public double PendulumLength { get; set; }
        public double Voltage { get; set; }
        public double Gamma1 { get; set; }
        public double Gamma2 { get; set; }
        public double HorizontalWindForce { get; set; }
        public double VerticalWindForce { get; set; }
        public const double G = 9.83;

        public SolverParameters()
        {
            PendulumMass = 0.231;
            CartMass = 0.792;
            PendulumLength = 0.610;
            Gamma1 = 1.72;
            Gamma2 = 7.68;
            Voltage = 0.0;
            HorizontalWindForce = 0.0;
            VerticalWindForce = 0.0;
        }
    }
}
