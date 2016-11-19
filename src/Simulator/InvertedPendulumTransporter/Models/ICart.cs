namespace InvertedPendulumTransporter.Models
{
    public interface ICart : IModel
    {
        double PlatformSize { get; }

        void SetupHighGradeTextures();
        void SetupLowGradeTextures();
    }
}
