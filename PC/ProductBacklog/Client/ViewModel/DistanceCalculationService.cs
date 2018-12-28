using System;

namespace ProductBacklog.Client.ViewModel
{
    class DistanceCalculationService : IDistanceCalculationService
    {
        private double? sampleDistance;
        private double distance;
        private long? time;

        public double Distance => distance;

        public double SampleDistance => sampleDistance.GetValueOrDefault();

        public void Refresh(double velocitySample, long timeOfSample)
        {
            timeOfSample /= 1000000000;
            if (timeOfSample == 0)
                timeOfSample = 1;
            if (time.HasValue == false)
            {
                time = timeOfSample;
                return;
            }

            var deltaTime = (int)(timeOfSample - time.GetValueOrDefault());
            if ((decimal)velocitySample == 0)
                return;

            if (deltaTime <= 0)
            {
                deltaTime = 1;
            }

            sampleDistance = velocitySample * deltaTime;
            distance = distance + Math.Abs(sampleDistance.GetValueOrDefault());
            time = timeOfSample;

            if (Math.Abs(velocitySample) <= 0)
            {
                Reset();
            }
        }

        public void Reset()
        {
            time = null;
            sampleDistance = null;
        }
    }
}
