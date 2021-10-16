using System;
using BepInEx.Configuration;

namespace VeVantZeData.Collector
{
    class VeVantZeDataConfig
    {
        internal bool InfluxDBEnabled { get; private set; }
        internal string InfluxDBAddress { get; private set; }
        internal string InfluxDBOrg { get; private set; }
        internal string InfluxDBBucket { get; private set; }
        internal InfluxDB.Client.Core.LogLevel InfluxDBClientLogLevel { get; private set; }
        internal bool LogWriterEnabled { get; private set; }
        internal bool CsvWriterEnabled { get; private set; }

        internal VeVantZeDataConfig(ConfigFile config)
        {
            InfluxDBEnabled = config.Bind("writers:influxdb", "enabled", false, "Activate writing metrics to InfluxDB").Value;
            InfluxDBAddress = config.Bind("writers:influxdb", "address", "http://localhost:8086").Value;
            InfluxDBOrg = config.Bind("writers:influxdb", "org", "thundersen").Value;
            InfluxDBBucket = config.Bind("writers:influxdb", "bucket", "vevantzedata").Value;
            InfluxDBClientLogLevel = ReadInfluxLogLevel(config);

            LogWriterEnabled = config.Bind("writers:log", "enabled", false, "Activate writing an overview of collected metrics to LogOutput.log").Value;

            CsvWriterEnabled = config.Bind("writers:csv", "enabled", true, "Activate writing metrics to CSV files").Value;
        }

        private InfluxDB.Client.Core.LogLevel ReadInfluxLogLevel(ConfigFile config)
        {
            var allLogLevels = string.Join(", ", Enum.GetNames(typeof(InfluxDB.Client.Core.LogLevel)));
            var defaultLogLevel = InfluxDB.Client.Core.LogLevel.None.ToString();
            var valueAsString = config.Bind("writers:influxdb", "client_log_level", defaultLogLevel, allLogLevels).Value;
            return Enum.Parse<InfluxDB.Client.Core.LogLevel>(valueAsString);
        }
    }
}