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
        private float minimum;
        private float maximum;
        private float average;
        private bool canStart = false;
        private readonly List<float> statisticSamples = new List<float>();

        [Dependency]
        public IClientStateController ClientStateController { get; set; }

        [Dependency]
        public IClientService ClientService { get; set; }

        public ObservableCollection<SensorInfo> AvailableSensors {get; set; }
        public ObservableCollection<KeyValuePair<int, float>> Samples { get; set; }

        public ICommand StartClient { get; set; }
        public ICommand ShutdownClient { get; set; }
        public ICommand SendMessage { get; set; }
        public ICommand ActivateSensorCommand { get; set; }
        public ICommand ClearSamples { get; set; }

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

        public float Minimum
        {
            get => minimum;
            set => SetField(ref minimum, value, nameof(Minimum));
        }

        public float Maximum
        {
            get => maximum;
            set => SetField(ref maximum, value, nameof(Maximum));
        }

        public float Average
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
            ClearSamples = new DelegateCommand<object>(ExecuteClearSamples);
            AvailableSensors = new ObservableCollection<SensorInfo>();
            Samples = new ObservableCollection<KeyValuePair<int, float>>();
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

        public void ExecuteClearSamples(object args)
        {
            Samples.Clear();
            canStart = false;
            statisticSamples.Clear();
        }

        public void OnLoaded()
        {
            ClientStateController.Attach(this);
            ClientService.OnMessageHandler += OnClientServiceMessageHandler;
        }

        public void OnUnloaded()
        {
        }

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
                        var x = float.Parse(data[1]);
                        var y = float.Parse(data[2]);
                        var z = float.Parse(data[3]);
                        var t = (int)(long.Parse(data[4]));// / 1000000000);

                        if (!canStart || statisticSamples.Count <= 10)
                        {
                            statisticSamples.Add(x);
                            if (statisticSamples.Count != 10)
                                break;
                            GatherStatistic();
                            canStart = true;
                        }

                        System.Diagnostics.Debug.WriteLine($"X = {x}");

                        x += Average;
                        Application.Current.Dispatcher.Invoke(() =>
                            Samples.Add(new KeyValuePair<int, float>(t, x)));

                        System.Diagnostics.Debug.WriteLine($"X = {x} T = {data[4]} {t}");
                    }
                }
            }
        }

        private void GatherStatistic()
        {
            statisticSamples.Sort();
            Minimum = statisticSamples.First();
            Maximum = statisticSamples.Last();
            Average = statisticSamples.Average();
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
