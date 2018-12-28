using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using Labit.Composition;
using Labit.Interfaces;
using Microsoft.Practices.Unity;
using Prism.Commands;
using ProductBacklog.Client.Interfaces;
using System.Windows.Input;
using ProductBacklog.Main;

namespace ProductBacklog.Client.ViewModel
{
    [View("ClientDiagnosticMonitorView")]
    class ClientDiagnosticMonitor : BaseViewModel, IClientDiagnosticMonitor, ILoadable
    {
        private bool isServerOnline;
        private bool isParing;
        private double velocity;
        private double acceleration;
        private double distance;
        private double sampleDistance;
        private double minimum;
        private double maximum;
        private double average;

        [Dependency]
        public IClientStateController ClientStateController { get; set; }

        [Dependency]
        public IClientService ClientService { get; set; }

        [Dependency]
        public IVelocityCalculationService VelocityCalculationService { get; set; }

        [Dependency]
        public IDistanceCalculationService DistanceCalculationService { get; set; }

        public ObservableCollection<SensorInfo> AvailableSensors {get; set; }

        public ICommand StartClient { get; set; }
        public ICommand ShutdownClient { get; set; }
        public ICommand SendMessage { get; set; }
        public ICommand ActivateSensorCommand { get; set; }

        public bool IsServerOnline
        {
            get => isServerOnline;
            set => SetField(ref isServerOnline, value, nameof(IsServerOnline));
        }

        public bool IsParing
        {
            get => isParing;
            set => SetField(ref isParing, value, nameof(IsParing));
        }

        public string Message { get; set; }

        public double Velocity
        {
            get => velocity;
            set => SetField(ref velocity, value, nameof(Velocity));
        }

        public double Acceleration
        {
            get => acceleration;
            set => SetField(ref acceleration, value, nameof(Acceleration));
        }

        public double Distance
        {
            get => distance;
            set => SetField(ref distance, value, nameof(Distance));
        }

        public double SampleDistance
        {
            get => sampleDistance;
            set => SetField(ref sampleDistance, value, nameof(SampleDistance));
        }

        public double Minimum
        {
            get => minimum;
            set => SetField(ref minimum, value, nameof(Minimum));
        }

        public double Maximum
        {
            get => maximum;
            set => SetField(ref maximum, value, nameof(Maximum));
        }

        public double Average
        {
            get => average;
            set => SetField(ref average, value, nameof(Average));
        }

        public ClientDiagnosticMonitor()
        {
            StartClient = new DelegateCommand<object>(ExecuteStartClient);
            SendMessage = new DelegateCommand<object>(ExecuteSendMessage);
            ShutdownClient = new DelegateCommand<object>(ExecuteShutdownClient);
            ActivateSensorCommand = new DelegateCommand<object>(ExecuteActivateSensor);
            AvailableSensors = new ObservableCollection<SensorInfo>();
        }

        private void ExecuteStartClient(object args)
        {
            ClientService.Launch();
        }

        private void ExecuteSendMessage(object args)
        {
            ClientService.SendMessage(Message);
        }

        private void ExecuteShutdownClient(object args)
        {
            ClientService.ShutDown();
        }

        private void ExecuteActivateSensor(object args)
        {
            if (args is SensorInfo sensor)
            {
                ClientService.SendMessage($"SENSORS A {sensor.SensorType}");
            }
        }

        public void OnLoaded()
        {
            ClientStateController.Attach(this);
            ClientService.OnMessageHandler += OnClientServiceMessageHandler;
        }

        public void OnUnloaded()
        {
        }

        public Action<double> move;

        public void Movements(Action<double> move)
        {
            this.move = move;
        }

        private int counter = 0;
        private bool canStart;
        private bool statisticInProgress;
        private List<double> samples = new List<double>();
        private bool? isLeftMovement;

        private void OnClientServiceMessageHandler(string message)
        {
            var startIndex = message.IndexOf("<modes>", StringComparison.Ordinal);
            if (startIndex!= -1)
                ParseModesAndCreateHandlers(message);
            else
            {
                if (message.StartsWith("SD"))
                {
                    var lines = message.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                    {
                        var data = line.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                        var x = double.Parse(data[1]);
                        var y = double.Parse(data[2]);
                        var z = double.Parse(data[3]);
                        var t = long.Parse(data[4]);

                        if (!canStart || samples.Count <= 100)
                        {
                            samples.Add(x);
                            if (samples.Count != 100)
                                return;
                            GatherStatistic(x, y, z);
                            canStart = true;
                        }

                        x -= Average; // bring zero to middle of a noise
                        var noiseSpectrum = (Math.Abs(Minimum) + Math.Abs(Maximum)) / 2.0;

                        var absX = Math.Abs(x);
                        if (noiseSpectrum > absX && absX < noiseSpectrum)
                        {
                            if (VelocityCalculationService.Velocity != 0)
                            {
                                counter++;
                                // slow down till zero
                                if (counter > 5)
                                {
                                    VelocityCalculationService.Reset();
                                    isLeftMovement = null;
                                }
                                else
                                {
                                    if (VelocityCalculationService.Velocity > 0)
                                        VelocityCalculationService.Refresh(-Math.Abs(x), t);
                                    else if (VelocityCalculationService.Velocity < 0)
                                    {
                                        VelocityCalculationService.Refresh(Math.Abs(x), t);
                                    }
                                }
                            }
                        }
                        else
                        {
                            counter = 0;

                            if (!isLeftMovement.HasValue)
                            {
                                isLeftMovement = x > 0;
                            }

                            if (isLeftMovement.Value && x > 0 )
                                VelocityCalculationService.Refresh(x, t);
                            else if (!isLeftMovement.Value && x < 0)
                                VelocityCalculationService.Refresh(x, t);
                            else
                            {
                                return;
                            }
                        }

                        DistanceCalculationService.Refresh(VelocityCalculationService.Velocity, t);

                        Velocity = VelocityCalculationService.Velocity;
                        Acceleration = VelocityCalculationService.Acceleration;
                        SampleDistance = DistanceCalculationService.SampleDistance;
                        Distance = DistanceCalculationService.Distance;

                        if (VelocityCalculationService.Velocity != 0)
                            move(SampleDistance*5);
                    }
                }
            }
        }

        private void GatherStatistic(double x, double y, double z)
        {
            samples.Sort();
            Minimum = samples.First() - 0.05;
            Maximum = samples.Last() + 0.05;
            Average = samples.Sum() / (double)samples.Count;
        }

        private void ParseModesAndCreateHandlers(string message)
        {
            var startIndex = message.IndexOf("<modes>", StringComparison.Ordinal);
            if (startIndex != -1)
            {
                startIndex += "<modes>".Length;
                var endIndex = message.IndexOf("</modes>", StringComparison.Ordinal);
                var messageLength = endIndex - startIndex;
                var text = message.Substring(startIndex, messageLength);
                var lines = text.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    var sensorInfo = new SensorInfo(line);
                    Application.Current.Dispatcher.Invoke(() => AvailableSensors.Add(sensorInfo));
                }
            }
        }
    }
}
