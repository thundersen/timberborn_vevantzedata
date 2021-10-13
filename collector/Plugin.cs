using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using System.Linq;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;
using UnityEngine;
using Timberborn.GameDistricts;
using VeVantZeData.Collector.Collection;
using VeVantZeData.Collector.GameAdapters;

namespace VeVantZeData.Collector
{
    [BepInPlugin("com.thundersen.vevantzedata.timberborn.collector", "Ve Vant Ze Data Timberborn!", "0.0.0.1")]
    [BepInProcess("Timberborn.exe")]
    public class Plugin : BaseUnityPlugin
    {
        private const float intervalSeconds = 10.0f;
        private float secondsSinceLastInterval;
        private TimeAdapter _timeAdapter;
        private GoodsAdapter _goodsAdapter;
        private static DistrictsAdapter _districtsAdapter;
        private static MetricsCollector _collector;
        private static Writer _writer;
        private static InfluxDBWriter _influxDBWriter;
        private static EntityInitializationListener _entityInitializationListener;
        private VeVantZeDataConfig _config;

        internal static ManualLogSource Log { get; private set; }

        private void Awake()
        {
            Log = base.Logger;

            _config = new VeVantZeDataConfig(Config);

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

            if (_config.InfluxDBEnabled)
                _influxDBWriter.Write(data);
        }

        private void OnGameStart()
        {
            _districtsAdapter = new DistrictsAdapter();
            _timeAdapter = new TimeAdapter(TimberbornGame.WeatherService, TimberbornGame.DayNightCycle);
            _goodsAdapter = new GoodsAdapter(_districtsAdapter, TimberbornGame.GoodSpecs, () => TimberbornGame.ResourceCountingService);
            _collector = new MetricsCollector(_districtsAdapter, _timeAdapter, _goodsAdapter);

            _writer = new Writer(TimberbornGame.Playthrough);

            if (_config.InfluxDBEnabled)
                _influxDBWriter = new InfluxDBWriter(_config, TimberbornGame.Playthrough);

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

            _districtsAdapter.AddDistrictCenter(dc);
        }
    }
}
