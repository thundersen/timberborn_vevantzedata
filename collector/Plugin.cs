using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using UnityEngine;
using Prometheus;


namespace VeVantZeData.Collector
{
    [BepInPlugin("com.thundersen.vevantzedata.timberborn.collector", "Ve Vant Ze Data Timberborn!", "0.0.0.1")]
    [BepInProcess("Timberborn.exe")]
    public class Plugin : BaseUnityPlugin
    {

        private static readonly Counter TickTock = Metrics.CreateCounter("sampleapp_ticks_total", "Just keeps on ticking");

        private const float intervalSeconds = 10.0f;
        private float secondsSinceLastInterval;
        internal static ManualLogSource Log;
        private Collector _collector;

        private void Awake()
        {
            Log = base.Logger;

            var server = new MetricServer(hostname: "localhost", port: 1234);
            server.Start();

            TickTock.Inc();
            TickTock.Inc();
            TickTock.Inc();

            Logger.LogInfo($"Counter value is {TickTock.Value}");

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            Logger.LogInfo($"Plugin com.thundersen.vevantzedata.timberborn.collector is loaded!");

            _collector = new Collector();
        }

        void Update()
        {
            secondsSinceLastInterval += Time.deltaTime;
            if (secondsSinceLastInterval > intervalSeconds)
            {
                _collector.Collect();
                secondsSinceLastInterval = 0.0f;
            }
        }
    }
}
