using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace VeVantZeData.Collector
{
    class InfluxDBWriter
    {
        private readonly string _bucket;
        private readonly string _org;
        private readonly Playthrough _playthrough;
        private readonly InfluxDBClient _client;

        internal InfluxDBWriter(VeVantZeDataConfig config, Playthrough playthrough)
        {
            var token = Environment.GetEnvironmentVariable("VEVANTZEDATA_INFLUXDB_TOKEN");

            if (token == null)
                Plugin.Log.LogError("Environment variable VEVANTZEDATA_INFLUXDB_TOKEN not found. Can't write to InfluxDB.");
            else
            {
                _playthrough = playthrough;
                _bucket = config.InfluxDBBucket;
                _org = config.InfluxDBOrg;
                _client = InfluxDBClientFactory.Create(config.InfluxDBAddress, token.ToCharArray());
                Plugin.Log.LogDebug("influx db client created.");
                _client.HealthAsync().ContinueWith(t => Plugin.Log.LogDebug(t.Result)).Wait();
            }
        }

        internal void Write(Data data)
        {
            if (_client != null)
            {
                var points = new List<PointData>();
                points.Add(PointFrom(data.GlobalPops, data.GameTime, "[GLOBAL]"));
                points.AddRange(data.DistrictPops.Select(kvp => PointFrom(kvp.Value, data.GameTime, kvp.Key)));

                using (var writeApi = _client.GetWriteApi())
                {
                    writeApi.WritePoints(_bucket, _org, points);
                }

                Plugin.Log.LogDebug($"{points.Count} points written to influxdb");
            }
        }

        private PointData PointFrom(Pops pops, GameTime gameTime, string districName)
        {
            return PointData
                    .Measurement("pops")
                    .Tag("game", "timberborn")
                    .Tag("playthrough_id", _playthrough.ID.ToString())
                    .Tag("faction", Playthrough.FactionName)
                    .Tag("map", Playthrough.MapName)
                    .Tag("district", districName)
                    .Field("cycle", gameTime.Cycle)
                    .Field("cycle_day", gameTime.CycleDay)
                    .Field("adults", pops.Adults)
                    .Field("kits", pops.Children)
                    .Timestamp(gameTime.GameTimeStamp, WritePrecision.Ms);
        }
    }
}