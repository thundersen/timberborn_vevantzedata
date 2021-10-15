using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using Timberborn.SingletonSystem;
using UnityEngine;
using VeVantZeData.Collector.Collection;
using VeVantZeData.Collector.GameAdapters;
using VeVantZeData.Collector.Output;

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
        private IMetricsOutput _output;
        private static EntityListener _entityListener;
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
            _output.Write(data);
        }

        private void OnGameStart()
        {
            _districtsAdapter = new DistrictsAdapter();
            DistrictCenterListener.DistrictsAdapter = _districtsAdapter;
            _timeAdapter = new TimeAdapter(TimberbornGame.WeatherService, TimberbornGame.DayNightCycle);
            _goodsAdapter = new GoodsAdapter(_districtsAdapter, TimberbornGame.GoodSpecs, () => TimberbornGame.ResourceCountingService);
            _collector = new MetricsCollector(_districtsAdapter, _timeAdapter, _goodsAdapter);

            _output = MetricsOutput.Create(_config, TimberbornGame.Playthrough);

            Log.LogDebug("Reset after game start.");
        }

        internal static void SetEventBus(EventBus eventBus)
        {
            if (_entityListener != null)
                _entityListener.TearDown();

            _entityListener = EntityListener.Builder.WithEventBus(eventBus)
                    .WithCreationActions(DistrictCenterListener.CaptureCreatedDistrictCenter)
                    .WithDestructionActions(DistrictCenterListener.CaptureDestroyedDistrictCenter)
                    .Build();
        }

    }
}
