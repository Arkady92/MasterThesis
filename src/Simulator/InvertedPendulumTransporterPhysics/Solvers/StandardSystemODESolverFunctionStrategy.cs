﻿using InvertedPendulumTransporterPhysics.Common;
using System;

namespace InvertedPendulumTransporterPhysics.Solvers
{
    public class StandardSystemODESolverFunctionStrategy : IODESolverFunctionStrategy
    {
        public void ODESolverFunction(double[] y, double x, double[] dy, object obj)
        {
            // y: x, theta, dx, dtheta
            // dy: dx, dtheta, d2x, d2theta
            var data = obj as SolverParameters;
            dy[0] = y[2];
            dy[1] = y[3];
            dy[2] = (-data.PendulumMass * SolverParameters.G / data.CartMass) * y[1] + (-data.Gamma2 / data.CartMass) * y[2]
                + (data.Gamma1 / data.CartMass * data.Voltage);
            dy[3] = ((data.CartMass + data.PendulumMass) * SolverParameters.G / data.CartMass / data.PendulumLength) * y[1]
                + (data.Gamma2 / data.CartMass / data.PendulumLength) * y[2]
                + (-data.Gamma1 / data.CartMass / data.PendulumLength * data.Voltage);
        }
    }
}