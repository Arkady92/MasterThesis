﻿namespace InvertedPendulumTransporter
{
    public class SystemParameters
    {
        public double PendulumMass { get; set; }
        public double CartMass { get; set; }
        public double PendulumLength { get; set; }
        public double Voltage { get; set; }
        public double Gamma1 { get; set; }
        public double Gamma2 { get; set; }
        public const double G = 9.83;

        public SystemParameters()
        {
            PendulumMass = 2.31; // 0.231
            CartMass = 0.792;
            PendulumLength = 0.610;
            Gamma1 = 1.72;
            Gamma2 = 7.68;
            Voltage = 0;
        }
    }
}