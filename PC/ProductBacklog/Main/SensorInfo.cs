using System;

namespace ProductBacklog.Main
{
    class SensorInfo
    {
        public string SensorName { get; private set; }
        public string SensorType { get; private set; }

        public SensorInfo(string sensorData)
        {
            var fields = sensorData.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries);
            SensorName = fields[0].Replace("Invensense ", "");
            SensorType = fields[1];
        }
    }
}
