using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using System.Linq;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;
using UnityEngine;
using Timberborn.GameDistricts;

namespace VeVantZeData.Collector
{
    [BepInPlugin("com.thundersen.vevantzedata.timberborn.collector", "Ve Vant Ze Data Timberborn!", "0.0.0.1")]
    [BepInProcess("Timberborn.exe")]
    public class Plugin : BaseUnityPlugin
    {
        private const float intervalSeconds = 10.0f;
        private float secondsSinceLastInterval;
        private static Collector _collector;
        private static Writer _writer;
        private static InfluxDBWriter _influxDBWriter;
        private static EntityInitializationListener _entityInitializationListener;

        internal static ManualLogSource Log { get; private set; }

        private void Awake()
        {
            Log = base.Logger;

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            TimberbornGame.AddGameStartCallback(OnGameStart);

            Logger.LogInfo($"Plugin com.thundersen.vevantzedata.timberborn.collector is loaded!");
        }

        void Update()
        {
            secondsSinceLastInterval += Time.deltaTime;
            if (secondsSinceLastInterval > intervalSeconds)
            {
                CollectIfInitialized();
                secondsSinceLastInterval = 0.0f;
            }
        }

        void CollectIfInitialized()
        {
            if (!TimberbornGame.IsRunning())
                return;

            var data = _collector.Collect();
            _writer.Write(data);
            _influxDBWriter.Write(data);
        }

        private static void OnGameStart()
        {
            _collector = new Collector(TimberbornGame.GlobalPopulation, TimberbornGame.WeatherService, TimberbornGame.DayNightCycle);
            _writer = new Writer(TimberbornGame.Playthrough);
            _influxDBWriter = new InfluxDBWriter(TimberbornGame.Playthrough);
            Log.LogDebug("Reset after game start.");
        }

        internal static void SetEventBus(EventBus eventBus)
        {
            if (_entityInitializationListener != null)
                _entityInitializationListener.TearDown();

            _entityInitializationListener = new EntityInitializationListener(eventBus, AddDistrictCenterToCollector);
        }

        private static void AddDistrictCenterToCollector(EntityComponent ec)
        {
            if (!ec.HasDistrictCenter())
                return;

            var dc = (DistrictCenter)ec.RegisteredComponents.First(c => c.GetType() == typeof(DistrictCenter));

            Log.LogDebug($"adding dc to collector {(dc).DistrictName}");

            _collector.AddDistrictCenter(dc);
        }
    }
}
