using BepInEx.Configuration;

namespace VeVantZeData.Collector
{
    class VeVantZeDataConfig
    {
        internal bool InfluxDBEnabled { get; private set; }
        internal string InfluxDBAddress { get; private set; }
        internal string InfluxDBOrg { get; private set; }
        internal string InfluxDBBucket { get; private set; }

        internal VeVantZeDataConfig(ConfigFile config)
        {
            InfluxDBEnabled = config.Bind("writers:influxdb", "enabled", false, "Activate the InfluxDB writer").Value;
            InfluxDBAddress = config.Bind("writers:influxdb", "address", "http://localhost:8086").Value;
            InfluxDBOrg = config.Bind("writers:influxdb", "org", "thundersen").Value;
            InfluxDBBucket = config.Bind("writers:influxdb", "bucket", "vevantzedata").Value;
        }
    }
}