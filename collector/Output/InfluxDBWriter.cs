using BepInEx;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace VeVantZeData.Collector.Output
{
    class InfluxDBWriter : IMetricsOutput
    {
        private readonly string _bucket;
        private readonly string _org;
        private readonly Playthrough _playthrough;
        private readonly InfluxDBClient _client;

        internal InfluxDBWriter(VeVantZeDataConfig config, Playthrough playthrough)
        {
            _playthrough = playthrough;
            _bucket = config.InfluxDBBucket;
            _org = config.InfluxDBOrg;

            if (!config.HasSetInfluxToken())
            {
                Plugin.Log.LogError($"Please set the InfluxDB token in the mod config under {Paths.ConfigPath}");
            }
            else
            {
                _client = InfluxDBClientFactory.Create(config.InfluxDBAddress, config.InfluxDBToken);
                _client.SetLogLevel(config.InfluxDBClientLogLevel);
                Plugin.Log.LogDebug("influx db client created.");
                _client.HealthAsync().ContinueWith(t => Plugin.Log.LogDebug(t.Result)).Wait();
            }
        }

        public void Write(Data data)
        {
            if (_client != null)
            {
                var sw = Stopwatch.StartNew();
                var points = new List<PointData>();
                points.AddRange(data.DistrictPops.Select(kvp => PointFrom(kvp.Value, data.GameTime.GameTimeStamp, kvp.Key)));
                points.AddRange(data.DistrictStocks.Select(kvp => PointFrom(kvp.Value, data.GameTime.GameTimeStamp, kvp.Key)));
                points.AddRange(data.DistrictDaysOfStocks.Select(kvp => PointFrom(kvp.Value, data.GameTime.GameTimeStamp, kvp.Key)));

                using (var writeApi = _client.GetWriteApi())
                {
                    writeApi.WritePoints(_bucket, _org, points);
                }
                sw.Stop();
                Plugin.Log.LogDebug($"{points.Count} points written to influxdb ({sw.ElapsedMilliseconds}ms)");
            }
        }

        private PointData PointFrom(Pops pops, DateTime gameTime, string district)
        {
            return StandardPointFrom("pops", gameTime, district)
                .Field("adults", pops.Adults)
                .Field("kits", pops.Children);
        }

        private PointData PointFrom(Goods goods, DateTime gameTime, string district)
        {
            var point = StandardPointFrom("stocks", gameTime, district);

            foreach (var good in goods.Counts.Keys)
            {
                point = point.Field(good, goods.Counts[good]);
            }

            return point;
        }

        private PointData PointFrom(DaysOfStocks daysOfStocks, DateTime gameTime, string district)
        {
            return StandardPointFrom("days_of_stocks", gameTime, district)
                .Field("water", daysOfStocks.Water)
                .Field("food", daysOfStocks.Food);
        }

        private PointData StandardPointFrom(string measurement, DateTime gameTime, string district)
        {
            return PointData
                    .Measurement(measurement)
                    .Tag("game", "timberborn")
                    .Tag("playthrough_id", _playthrough.ID.ToString())
                    .Tag("faction", _playthrough.FactionName)
                    .Tag("map", _playthrough.MapName)
                    .Tag("settlement", _playthrough.SettlementName)
                    .Tag("district", district)
                    .Timestamp(gameTime, WritePrecision.Ms);
        }
    }
}