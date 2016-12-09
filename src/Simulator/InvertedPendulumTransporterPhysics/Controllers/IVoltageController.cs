namespace InvertedPendulumTransporterPhysics.Controllers
{
    public interface IVoltageController : IController
    {
        ControlType ControlType { get; set; }
        double GetVoltage();
        void SetTime(double time);
        void SetControlError(double angleError, double positionError);
        void Reset(double timeDelta);
        void SetUserAngle(double userAngle);
    }
}
