namespace InvertedPendulumTransporterPhysics.Controllers
{
    public interface IVoltageController
    {
        ControlType ControlType { get; set; }
        double GetVoltage();
        void SetTime(double time);
        void SetControlError(double angleError, double positionError);
        void Reset(double timeDelta);

    }
}
