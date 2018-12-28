namespace ProductBacklog.Client.ViewModel
{
    interface IDistanceCalculationService
    {
        double Distance { get; }

        double SampleDistance { get; }

        void Refresh(double velocitySample, long timeOfSample);

        void Reset();
    }
}
