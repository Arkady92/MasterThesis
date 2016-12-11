namespace InvertedPendulumTransporter.Models
{
    /// <summary>
    /// Interface dedicated to simulation area model
    /// </summary>
    public interface ISimulationArea : IModel
    {
        /// <summary>
        /// Simulation area size
        /// </summary>
        double Size { get; }
    }
}
