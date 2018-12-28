using System;

namespace ProductBacklog.Client.ViewModel
{
    interface IVelocityCalculationService
    {
        double Velocity { get; }

        double Acceleration { get; }

        void Refresh(double accSample, long timeOfSample);

        void Reset();
    }
}
