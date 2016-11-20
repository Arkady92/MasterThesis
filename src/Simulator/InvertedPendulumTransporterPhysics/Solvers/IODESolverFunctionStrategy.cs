namespace InvertedPendulumTransporterPhysics.Solvers
{
    public interface IODESolverFunctionStrategy
    {
        void ODESolverFunction(double[] y, double x, double[] dy, object obj);
    }
}
