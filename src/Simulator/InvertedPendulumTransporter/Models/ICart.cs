namespace InvertedPendulumTransporter.Models
{
    /// <summary>
    /// Interface dedicated to cart model
    /// </summary>
    public interface ICart : IModel
    {
        /// <summary>
        /// Cart platform size
        /// </summary>
        double PlatformSize { get; }
    }
}
