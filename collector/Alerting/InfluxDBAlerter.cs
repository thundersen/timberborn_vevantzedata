using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using InfluxDB.Client;
using InfluxDB.Client.Api.Client;
using InfluxDB.Client.Api.Domain;

namespace VeVantZeData.Collector.Alerting
{
    class InfluxDBAlerter : IAlerter
    {
        private Playthrough _playthrough;
        private string _bucket;
        private string _org;
        private InfluxDBClient _client;

        public InfluxDBAlerter(VeVantZeDataConfig config, Playthrough playthrough)
        {
            // var token = Environment.GetEnvironmentVariable("VEVANTZEDATA_INFLUXDB_TOKEN");

            // if (token == null)
            //     Plugin.Log.LogError("Environment variable VEVANTZEDATA_INFLUXDB_TOKEN not found. Can't write to InfluxDB.");
            // else
            // {
            _playthrough = playthrough;
            _bucket = config.InfluxDBBucket;
            _org = config.InfluxDBOrg;

            var options = new InfluxDBClientOptions.Builder()
                .Org(_org)
                .Bucket(_bucket)
                .Url(config.InfluxDBAddress)
                .AuthenticateToken(config.InfluxDBToken)
                .LogLevel(config.InfluxDBClientLogLevel)
                .TimeOut(TimeSpan.FromSeconds(60))
                .ReadWriteTimeOut(TimeSpan.FromSeconds(60))
                .Build();
            _client = InfluxDBClientFactory.Create(options);
            // _client = InfluxDBClientFactory.Create(config.InfluxDBAddress, token.ToCharArray());

            // _client.SetLogLevel(config.InfluxDBClientLogLevel);
            Plugin.Log.LogDebug($"influx db client created with token {config.InfluxDBToken}.");
            _client.HealthAsync().ContinueWith(t => Plugin.Log.LogDebug(t.Result)).Wait();
            // }
        }

        public void Init()
        {
            if (_client != null)
            {
                var tasks = _client.GetTasksApi();
                var flux = LoadFluxTemplate("test_alert")
                    .Replace("$ALERT_NAME$", $"Berries Test Alert for {_playthrough.SettlementName}")
                    .Replace("$SETTLEMENT$", _playthrough.SettlementName);

                var task = new TaskCreateRequest(
                    org: _org,
                    flux: flux);

                var ulf = "";



                tasks.CreateTaskAsync(task).ContinueWith(t =>
                {
                    Plugin.Log.LogDebug($"created task with id {t.Result.Id}:\n{t.Result}");
                    ulf = t.Result.Id;
                }).GetAwaiter().GetResult();

                Plugin.Log.LogDebug($"captured task id {ulf}");

                // var checks = _client.GetChecksApi();

                // var flux = LoadFluxTemplate("test_check")
                //     .Replace("$SETTLEMENT$", _playthrough.SettlementName);

                // var check = new ThresholdCheck(
                //     name: $"Berries Test Alert for {_playthrough.SettlementName}",
                //     orgID: "0e59a89b330c44c0", //TODO: add to config and print in provision.sh
                //     query: new DashboardQuery(text: flux),
                //     status: TaskStatusType.Active,
                //     every: "1m",
                //     offset: "5s",
                //     statusMessageTemplate: "Check: ${r._check_name} is: ${r._level} (${r.Berries}) for ${r.district} in ${r.settlement}",
                //     thresholds: new List<Threshold>()
                //         {
                //             new LesserThreshold(value: 20, level: CheckStatusLevel.CRIT ),
                //             new LesserThreshold(value: 100, level: CheckStatusLevel.WARN ),
                //             new GreaterThreshold(value: 99, level: CheckStatusLevel.OK )
                //         }
                //     );


                // var result = checks.CreateCheckAsync(check).GetAwaiter().GetResult();

                // Plugin.Log.LogDebug($"Created check with ID {result.Id}");
            }
        }

        private string LoadFluxTemplate(string templateName)
        {
            string resourceName = $"{GetType().Namespace}.{templateName}.flux.template";

            Plugin.Log.LogDebug($"loading template {resourceName}");

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                return stream != null ? new StreamReader(stream).ReadToEnd() : null;
            }
        }

        public void CleanUp()
        {
            throw new System.NotImplementedException();
        }


        public void UpdateAlertStatus()
        {
            throw new System.NotImplementedException();
        }
    }
}