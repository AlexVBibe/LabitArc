using System;

namespace ProductBacklog.Client.ViewModel
{
    class VelocityCalculationService : IVelocityCalculationService
    {
        private double? velocity;
        private long? time;
        private double? acceleration;

        public void Refresh(double accSample, long timeOfSample)
        {
            timeOfSample /= 1000000000;
            if (time.HasValue == false)
            {
                time = timeOfSample;
                velocity = accSample;
                acceleration = accSample;
                return;
            }

            var deltaTime = (timeOfSample - time.GetValueOrDefault());
            velocity = velocity + accSample * deltaTime;
            time = timeOfSample;
            acceleration = accSample;

            if (Math.Abs(velocity.GetValueOrDefault()) < 0.1)
            {
                Reset();
            }
        }

        public void Reset()
        {
            time = null;
            velocity = null;
            acceleration = null;
        }

        public double Velocity => velocity.GetValueOrDefault();

        public double Acceleration => acceleration.GetValueOrDefault();
    }
}
